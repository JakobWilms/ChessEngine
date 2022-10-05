using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Interactivity;
using Avalonia.Media;
using ChessEngine;

namespace Chess;

public class CDisplay
{
    public static CDisplay Instance = null!;
    private static byte _fromPos = 64;

    private readonly MainWindow _window;
    private readonly Panel[] _pieces;
    private readonly Panel[] _possibleMoves;
    private readonly int _boardSize;
    private readonly CResources _resources;
    private readonly List<CMove> _moveList;
    private readonly List<string>[] _moveOutputList;
    public CBoard Board { get; private set; }

    public CDisplay(MainWindow window, int boardSize, string path, CBoard board)
    {
        _window = window;
        _pieces = new Panel[64];
        _possibleMoves = new Panel[64];
        _boardSize = boardSize;
        Board = board;
        _resources = new CResources(path);
        _moveList = new List<CMove>();
        _moveOutputList = new List<string>[3];
        for (int i = 0; i < 3; i++) _moveOutputList[i] = new List<string>();
        _window.MoveListBlackScroll.ScrollChanged += OnMoveListScrollBarChanged;
        _window.MoveListWhiteScroll.ScrollChanged += OnMoveListScrollBarChanged;
        _window.MoveListFullMovesScroll.ScrollChanged += OnMoveListScrollBarChanged;
        _window.PreviousMoveButton.Click += OnPreviousButtonClick;

        CheckerBoard();
        for (int i = 0; i < 64; i++)
        {
            _pieces[i] = new Panel
            {
                Width = boardSize >> 3,
                Height = boardSize >> 3
            };
            _pieces[i].Tapped += OnPanelTapped;
            Grid.SetColumn(_pieces[i], i % 8);
            Grid.SetRow(_pieces[i], 7 - (i >> 3));
            _possibleMoves[i] = new Panel
            {
                Width = boardSize >> 3,
                Height = boardSize >> 3
            };
            Grid.SetColumn(_possibleMoves[i], i % 8);
            Grid.SetRow(_possibleMoves[i], 7 - (i >> 3));
        }

        for (int r8 = 56; r8 >= 0; r8 -= 8)
        for (int f = 0; f < 8; f++)
        {
            window.Pieces.Children.Add(_pieces[r8 + f]);
            window.PossibleMoves.Children.Add(_possibleMoves[r8 + f]);
        }

        _window.FenButton.Click += OnFenButtonClick;
    }

    public void VisibleMove(CMove move)
    {
        move.Make(Board);
        UpdateMoveList(move);
        Display();
    }

    private void UpdateFenBox() => _window.FenBox.Text = FenReader.ExportFen(Board);

    private void UpdateMoveList(CMove move)
    {

        _moveList.Add(move);
        if (_moveList.Count % 2 == 0)
        {
            _moveOutputList[2].Add(move.ToDisplayMove());
        }
        else
        {
            _moveOutputList[0].Add($"{_moveOutputList[0].Count + 1}.)");
            _moveOutputList[1].Add(move.ToDisplayMove());
        }

        UpdateMoveList();
    }
    
    private void UpdateMoveList()
    {
        _window.MoveListFullMoves.Text = string.Join('\n', _moveOutputList[0]);
        _window.MoveListWhite.Text = string.Join('\n', _moveOutputList[1]);
        _window.MoveListBlack.Text = string.Join('\n', _moveOutputList[2]);
    }

    private void OnPreviousButtonClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (_moveList.Count == 0) return;
        _moveList[^1].Unmake(Board);
        if (_moveList.Count % 2 == 0)
        {
            _moveOutputList[2].RemoveAt(_moveOutputList[2].Count - 1);
        }
        else
        {
            _moveOutputList[1].RemoveAt(_moveOutputList[1].Count - 1);
            _moveOutputList[0].RemoveAt(_moveOutputList[0].Count - 1);
            
        }
        _moveList.RemoveAt(_moveList.Count - 1);
        UpdateMoveList();
        Display();
    }

    private void OnMoveListScrollBarChanged(object? sender, RoutedEventArgs routedEventArgs)
    {
        if (sender is not ScrollViewer scrollViewer) return;
        _window.MoveListFullMovesScroll.Offset = scrollViewer.Offset;
        _window.MoveListWhiteScroll.Offset = scrollViewer.Offset;
        _window.MoveListBlackScroll.Offset = scrollViewer.Offset;
    }

    private void OnFenButtonClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        Board = FenReader.ImportFen(_window.FenBox.Text);
        Display();
    }

    private void OnPanelTapped(object? sender, RoutedEventArgs args)
    {
        byte pos = (byte)(Grid.GetColumn(sender as Control) + 8 * (7 - Grid.GetRow(sender as Control)));
        if (_fromPos != 64 && _fromPos != pos && Board.GetOccupied(_fromPos))
        {
            List<CMove> moves = CMoveGeneration.MoveGen(Board);
            foreach (var move in moves)
            {
                if (move.GetFrom() != _fromPos || move.GetTo() != pos) continue;
                //move.Print();
                VisibleMove(move);
                foreach (var panel in _possibleMoves) panel.Children.Clear();
                _fromPos = 64;
                break;
            }
        }

        if (Board.GetColor(pos) == Board.ToMove)
        {
            UpdatePossibleMoves(pos);
        }
    }

    private void UpdatePossibleMoves(byte pos)
    {
        foreach (var panel in _possibleMoves) panel.Children.Clear();
        _fromPos = pos;
        List<CMove> moves = CMoveGeneration.MoveGen(Board);
        IEnumerable<CMove> pieceMoves = moves.Where(m => m.GetFrom() == _fromPos);
        foreach (var move in pieceMoves)
        {
            _possibleMoves[(move.GetTo() & ~0x7) + move.GetTo() % 8].Children
                .Add(new Image { Source = _resources.PossibleMove });
        }
    }

    public void Display()
    {
        for (byte i = 0; i < 64; i++)
        {
            _pieces[i].Children.Clear();
            _pieces[i].Children.Add(new Image { Source = _resources.Textures[Board.GetPieceType(i)] });
        }

        UpdateFenBox();
        Engine.TryEngineMove();
    }

    private void CheckerBoard()
    {
        var black = Color.FromRgb(181, 136, 99);
        var white = Color.FromRgb(240, 217, 181);
        for (byte i = 0; i < 64; i++)
        {
            var rectangle = new Rectangle
            {
                Fill = new SolidColorBrush((i >> 3) % 2 == i % 8 % 2
                    ? white
                    : black),
                Width = _boardSize >> 3,
                Height = _boardSize >> 3
            };
            Grid.SetColumn(rectangle, i % 8);
            Grid.SetRow(rectangle, i >> 3);
            _window.Board.Children.Add(rectangle);
        }
    }
}