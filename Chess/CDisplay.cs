using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;

namespace Chess;

public class CDisplay
{
    public static CDisplay Instance = null!;
    private static byte _fromPos = 64;

    private readonly MainWindow _window;
    private readonly List<byte> _possibleMoves;
    private readonly CResources _resources;
    private readonly List<CMove> _moveList;
    private readonly List<string>[] _moveOutputList;
    private CMove _cached;
    public CBoard Board { get; private set; }

    public CDisplay(MainWindow window, string path, CBoard board)
    {
        _window = window;
        Board = board;
        _resources = new CResources(path);
        _moveList = new List<CMove>();
        _moveOutputList = new List<string>[3];
        _cached = null!;
        _window.BKnightPromotion.Source = _resources.GetTexture(PieceType.BlackKnight, true, false);
        _window.BBishopPromotion.Source = _resources.GetTexture(PieceType.BlackBishop, true, false);
        _window.BRookPromotion.Source = _resources.GetTexture(PieceType.BlackRook, true, false);
        _window.BQueenPromotion.Source = _resources.GetTexture(PieceType.BlackQueen, true, false);
        _window.WKnightPromotion.Source = _resources.GetTexture(PieceType.WhiteKnight, true, false);
        _window.WBishopPromotion.Source = _resources.GetTexture(PieceType.WhiteBishop, true, false);
        _window.WRookPromotion.Source = _resources.GetTexture(PieceType.WhiteRook, true, false);
        _window.WQueenPromotion.Source = _resources.GetTexture(PieceType.WhiteQueen, true, false);
        _window.BKnightPromotion.Tapped += OnPromotionSelection;
        _window.BBishopPromotion.Tapped += OnPromotionSelection;
        _window.BRookPromotion.Tapped += OnPromotionSelection;
        _window.BQueenPromotion.Tapped += OnPromotionSelection;
        _window.WKnightPromotion.Tapped += OnPromotionSelection;
        _window.WBishopPromotion.Tapped += OnPromotionSelection;
        _window.WRookPromotion.Tapped += OnPromotionSelection;
        _window.WQueenPromotion.Tapped += OnPromotionSelection;
        for (int i = 0; i < 3; i++) _moveOutputList[i] = new List<string>();
        _window.MoveListBlackScroll.ScrollChanged += OnMoveListScrollBarChanged;
        _window.MoveListWhiteScroll.ScrollChanged += OnMoveListScrollBarChanged;
        _window.MoveListFullMovesScroll.ScrollChanged += OnMoveListScrollBarChanged;
        _window.PreviousMoveButton.Click += OnPreviousButtonClick;
        _window.FenButton.Click += OnFenButtonClick;
        _possibleMoves = new List<byte>();
    }

    public void VisibleMove(CMove move)
    {
        move.Make(Board);
        UpdateMoveList(move);
        UpdatePossibleMoves(64);
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

    private void OnPromotionSelection(object? sender, RoutedEventArgs args)
    {
        Image image = sender as Image ?? throw new InvalidOperationException();
        CMove? move = image.Name switch
        {
            "BKnightPromotion" or "WKnightPromotion" => new CMove(_cached.GetFromSquare(), _cached.GetToSquare(),
                _cached.IsCapture() ? MoveFlag.KnightPromotionCapture : MoveFlag.KnightPromotion,
                _cached.GetFromPieceType(), _cached.GetToPieceType()),
            "BBishopPromotion" or "WBishopPromotion" => new CMove(_cached.GetFromSquare(), _cached.GetToSquare(),
                _cached.IsCapture() ? MoveFlag.BishopPromotionCapture : MoveFlag.BishopPromotion,
                _cached.GetFromPieceType(), _cached.GetToPieceType()),
            "BRookPromotion" or "WRookPromotion" => new CMove(_cached.GetFromSquare(), _cached.GetToSquare(),
                _cached.IsCapture() ? MoveFlag.RookPromotionCapture : MoveFlag.RookPromotion,
                _cached.GetFromPieceType(), _cached.GetToPieceType()),
            "BQueenPromotion" or "WQueenPromotion" => new CMove(_cached.GetFromSquare(), _cached.GetToSquare(),
                _cached.IsCapture() ? MoveFlag.QueenPromotionCapture : MoveFlag.QueenPromotion,
                _cached.GetFromPieceType(), _cached.GetToPieceType()),
            _ => null
        };
        if (move != null) VisibleMove(move);
        _window.WPromotionGrid.IsVisible = false;
        _window.BPromotionGrid.IsVisible = false;
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
                if (move.IsPromotion())
                {
                    if (Board.ToMove == ColorType.White) _window.WPromotionGrid.IsVisible = true;
                    else _window.BPromotionGrid.IsVisible = true;
                    _cached = move;
                    return;
                }

                VisibleMove(move);
                return;
            }
        }

        UpdatePossibleMoves(Board.GetColor(pos) == Board.ToMove ? pos : (byte)64);
    }

    private void UpdatePossibleMoves(byte pos)
    {
        _fromPos = pos;
        _possibleMoves.Clear();
        Display();
        if (pos == 64) return;
        List<CMove> moves = CMoveGeneration.MoveGen(Board);
        IEnumerable<CMove> pieceMoves = moves.Where(m => m.GetFrom() == _fromPos);
        foreach (var move in pieceMoves) _possibleMoves.Add(move.GetTo());

        Display();
    }

    public void Display()
    {
        _window.Board.Children.Clear();
        for (int r8 = 56; r8 >= 0; r8 -= 8)
        for (int f = 0; f < 8; f++)
        {
            Image image = new Image
            {
                Source = _resources.GetTexture(Board.GetPieceType((byte)(r8 + f)), (r8 >> 3) % 2 == f % 2,
                    _possibleMoves.Contains((byte)(r8 + f))),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            image.Tapped += OnPanelTapped;
            Grid.SetColumn(image, f);
            Grid.SetRow(image, 7 - (r8 >> 3));
            _window.Board.Children.Add(image);
        }

        UpdateFenBox();
        Engine.TryEngineMove();
    }
}