using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Chess.Book;

namespace Chess;

public class CDisplay
{
    public static CDisplay Instance = null!;
    private static byte _fromPos = 64;

    public MainWindow Window { get; }
    private readonly List<byte> _possibleMoves;
    private readonly CResources _resources;
    private readonly List<CMove> _moveList;
    private readonly List<string>[] _moveOutputList;
    private CMove _cached;
    public COpeningBook Book { get; }
    public CBoard Board { get; private set; }

    public CDisplay(MainWindow window, string path, CBoard board)
    {
        Window = window;
        Board = board;
        _resources = new CResources(path);
        _moveList = new List<CMove>();
        _moveOutputList = new List<string>[3];
        _cached = null!;
        Window.BKnightPromotion.Source = _resources.GetTexture(PieceType.BlackKnight, true, false);
        Window.BBishopPromotion.Source = _resources.GetTexture(PieceType.BlackBishop, true, false);
        Window.BRookPromotion.Source = _resources.GetTexture(PieceType.BlackRook, true, false);
        Window.BQueenPromotion.Source = _resources.GetTexture(PieceType.BlackQueen, true, false);
        Window.WKnightPromotion.Source = _resources.GetTexture(PieceType.WhiteKnight, true, false);
        Window.WBishopPromotion.Source = _resources.GetTexture(PieceType.WhiteBishop, true, false);
        Window.WRookPromotion.Source = _resources.GetTexture(PieceType.WhiteRook, true, false);
        Window.WQueenPromotion.Source = _resources.GetTexture(PieceType.WhiteQueen, true, false);
        Window.BKnightPromotion.Tapped += OnPromotionSelection;
        Window.BBishopPromotion.Tapped += OnPromotionSelection;
        Window.BRookPromotion.Tapped += OnPromotionSelection;
        Window.BQueenPromotion.Tapped += OnPromotionSelection;
        Window.WKnightPromotion.Tapped += OnPromotionSelection;
        Window.WBishopPromotion.Tapped += OnPromotionSelection;
        Window.WRookPromotion.Tapped += OnPromotionSelection;
        Window.WQueenPromotion.Tapped += OnPromotionSelection;
        for (int i = 0; i < 3; i++) _moveOutputList[i] = new List<string>();
        Window.MoveListBlackScroll.ScrollChanged += OnMoveListScrollBarChanged;
        Window.MoveListWhiteScroll.ScrollChanged += OnMoveListScrollBarChanged;
        Window.MoveListFullMovesScroll.ScrollChanged += OnMoveListScrollBarChanged;
        Window.PreviousMoveButton.Click += OnPreviousButtonClick;
        Window.FenButton.Click += OnFenButtonClick;
        Window.ResetButton.Click += OnResetButtonClick;
        _possibleMoves = new List<byte>();
        Book = new COpeningBook("/home/jakob/RiderProjects/Chess/openingdb/book.cbook");
        UpdateOpeningBlock();
    }

    public void VisibleMove(CMove move)
    {
        move.Make(Board);
        UpdateMoveList(move);
        UpdatePossibleMoves(64);
        Display();
    }

    private void UpdateOpeningBlock()
    {
        CBookEntry? entry = Book.BookEntry(Board);

        Window.OpeningBlock.Text = entry == null ? "" : new OpeningDisplay(entry, Board).ToString(Board);
    }

    private void UpdateFenBox() => Window.FenBox.Text = FenReader.ExportFen(Board);

    private void UpdateMoveList(CMove move)
    {
        ushort count = CMoveGeneration.MoveGenCount(Board);
        bool check = Board.InCheck(Board.ToMove);
        _moveList.Add(move);
        if (_moveList.Count % 2 == 0)
        {
            _moveOutputList[2].Add(move.ToDisplayMove() + (count == 0 ? check ? "#" : "+" : ""));
        }
        else
        {
            _moveOutputList[0].Add($"{_moveOutputList[0].Count + 1}.)");
            _moveOutputList[1].Add(move.ToDisplayMove() + (count == 0 ? check ? "#" : "+" : ""));
        }

        UpdateMoveList();
    }

    private void UpdateMoveList()
    {
        Window.MoveListFullMoves.Text = string.Join('\n', _moveOutputList[0]);
        Window.MoveListWhite.Text = string.Join('\n', _moveOutputList[1]);
        Window.MoveListBlack.Text = string.Join('\n', _moveOutputList[2]);
    }

    private void OnResetButtonClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        for (int i = 0; i < 3; i++) _moveOutputList[i] = new List<string>();
        _moveList.Clear();
        _cached = null!;
        _fromPos = 64;
        Board = CBoard.StartingBoard();
        UpdatePossibleMoves(64);
        Display();
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
        Window.MoveListFullMovesScroll.Offset = scrollViewer.Offset;
        Window.MoveListWhiteScroll.Offset = scrollViewer.Offset;
        Window.MoveListBlackScroll.Offset = scrollViewer.Offset;
    }

    private void OnFenButtonClick(object? sender, RoutedEventArgs routedEventArgs)
    {
        Board = FenReader.ImportFen(Window.FenBox.Text);
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
        Window.WPromotionGrid.IsVisible = false;
        Window.BPromotionGrid.IsVisible = false;
    }

    private void OnPanelTapped(object? sender, RoutedEventArgs args)
    {
        byte pos = (byte)(Grid.GetColumn(sender as Control) + 8 * (7 - Grid.GetRow(sender as Control)));
        if (_fromPos != 64 && _fromPos != pos && Board.GetOccupied(_fromPos))
        {
            CMove?[] moves = CMoveGeneration.MoveGen(Board);
            foreach (var move in moves)
            {
                if (move == null) break;
                if (move.GetFrom() != _fromPos || move.GetTo() != pos) continue;
                if (move.IsPromotion())
                {
                    if (Board.ToMove == ColorType.White) Window.WPromotionGrid.IsVisible = true;
                    else Window.BPromotionGrid.IsVisible = true;
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
        CMove?[] moves = CMoveGeneration.MoveGen(Board);
        List<CMove> pieceMoves = new List<CMove>();
        foreach (var move in moves)
        {
            if (move == null) break;
            if (move.GetFrom() == _fromPos) pieceMoves.Add(move);
        }

        foreach (var move in pieceMoves) _possibleMoves.Add(move.GetTo());
        _possibleMoves.Add(pos);
        Display();
    }

    public void Display()
    {
        UpdateOpeningBlock();
        Window.Board.Children.Clear();
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
            Window.Board.Children.Add(image);
        }

        UpdateFenBox();
        Engine.TryEngineMove(Book.BookEntry(Board));
    }
}