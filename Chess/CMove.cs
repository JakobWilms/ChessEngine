using System;
using static Chess.CSquare;
using static Chess.EnumPiece;
using static Chess.MoveFlag;
using static Chess.PieceType;

namespace Chess;

public class CMove
{
    private ushort _move; // Flags 4, From 6, To 6
    private byte _pieces; // FromPiece 4, ToPiece 4
    private ushort _prevInfo;

    private CMove(byte from, byte to, byte flags, byte fromPiece, byte toPiece)
    {
        _move = (ushort)(((flags & 0xf) << 12) | ((from & 0x3f) << 6) | (to & 0x3f));
        _pieces = (byte)(((fromPiece & 0xf) << 4) | (toPiece & 0xf));
    }

    public CMove(CSquare from, CSquare to, MoveFlag flag, PieceType fromPiece, PieceType toPiece) : this((byte)from,
        (byte)to, (byte)flag, (byte)fromPiece, (byte)toPiece)
    {
    }

    public void Print() => Console.WriteLine(ToString());

    public override string ToString() =>
        $"From:{(CSquare)GetFrom()}, To:{(CSquare)GetTo()}, Flag:{GetFlags()}, FromPiece:{GetFromPieceType()}, ToPiece:{GetToPieceType()}";

    public string ToShortString() =>
        $"{(CSquare)GetFrom()}{(CSquare)GetTo()}";

    public string ToDisplayMove()
    {
        switch (GetFlags())
        {
            case QuietMove:
            case DoublePawnPush:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                       (CSquare)GetTo();
            case KingCastle:
                return "0-0";
            case QueenCastle:
                return "0-0-0";
            case Capture:
            case EpCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                       (CSquare)GetTo();
            case KnightPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteKnight
                           : BlackKnight);
            case BishopPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteBishop
                           : BlackBishop);
            case RookPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteRook
                           : BlackRook);
            case QueenPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteQueen
                           : BlackQueen);
            case KnightPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteKnight
                           : BlackKnight);
            case BishopPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteBishop
                           : BlackBishop);
            case RookPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteRook
                           : BlackRook);
            case QueenPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                       (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                           ? WhiteQueen
                           : BlackQueen);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public byte GetTo() => (byte)(_move & 0x3f);
    public CSquare GetToSquare() => (CSquare)GetTo();
    public byte GetFrom() => (byte)((_move >> 6) & 0x3f);
    public CSquare GetFromSquare() => (CSquare)GetFrom();
    public MoveFlag GetFlags() => (MoveFlag)((_move >> 12) & 0x0f);
    public byte GetToPiece() => (byte)(_pieces & 0xf);
    public PieceType GetToPieceType() => (PieceType)GetToPiece();
    public byte GetFromPiece() => (byte)((_pieces >> 4) & 0xf);
    public PieceType GetFromPieceType() => (PieceType)GetFromPiece();

    public void SetTo(byte to)
    {
        _move &= 0xffa0;
        _move |= (ushort)(to & 0x3f);
    }

    public void SetFrom(byte from)
    {
        _move &= 0xf030;
        _move |= (ushort)((from & 0x3f) << 6);
    }

    public void SetFlags(MoveFlag flag) => SetFlags((byte)flag);

    private void SetFlags(byte flags)
    {
        _move &= 0xfff;
        _move |= (ushort)((flags & 0xf) << 12);
    }

    public void SetToPiece(byte toPiece)
    {
        _pieces &= 0xf8;
        _pieces |= (byte)(toPiece & 0x7);
    }

    public void SetToPiece(PieceType pieceType) => SetToPiece((byte)pieceType);

    public void SetFromPiece(byte fromPiece)
    {
        _pieces &= 0x38;
        _pieces |= (byte)((fromPiece & 0x7) << 3);
    }

    public int GetIndex() => _move & 0x0fff;

    public void Make(CBoard board)
    {
        _prevInfo = board.Info;
        board.SetEnPassantTargetSquare(A1);
        switch (GetFlags())
        {
            case QuietMove:
                MakeQuiet(board, GetFrom(), GetTo(), GetFromPieceType());
                break;
            case DoublePawnPush:
                MakeDoublePawnPush(board, GetFrom(), GetTo());
                break;
            case KingCastle:
            case QueenCastle:
                MakeCastle(board);
                break;
            case Capture:
                MakeCapture(board);
                break;
            case EpCapture:
                MakeEpCapture(board);
                break;
            case KnightPromotion:
            case BishopPromotion:
            case RookPromotion:
            case QueenPromotion:
                MakePromotion(board);
                break;
            case KnightPromotionCapture:
            case BishopPromotionCapture:
            case RookPromotionCapture:
            case QueenPromotionCapture:
                MakePromotionCapture(board);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        UpdateCastlingRights(board);
        if (board.ToMove == ColorType.Black) board.FullMoves++;
        board.SwapToMove();
        if (GetFromPieceType() is WhitePawn or BlackPawn || IsCapture())
            board.SetHalfMoves(0);
        else board.SetHalfMoves((byte)(board.GetHalfMoves() + 1));
    }

    public void Unmake(CBoard board)
    {
        switch (GetFlags())
        {
            case QuietMove:
            case DoublePawnPush:
                MakeQuiet(board, GetTo(), GetFrom(), GetFromPieceType());
                break;
            case KingCastle:
            case QueenCastle:
                UnmakeCastle(board);
                break;
            case Capture:
                UnmakeCapture(board);
                break;
            case EpCapture:
                UnmakeEpCapture(board);
                break;
            case KnightPromotion:
            case BishopPromotion:
            case RookPromotion:
            case QueenPromotion:
                UnmakePromotion(board);
                break;
            case KnightPromotionCapture:
            case BishopPromotionCapture:
            case RookPromotionCapture:
            case QueenPromotionCapture:
                UnmakePromotionCapture(board);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        board.Info = _prevInfo;
        if (board.ToMove == ColorType.White) board.FullMoves--;
        board.SwapToMove();
    }

    private void UpdateCastlingRights(CBoard board)
    {
        if (GetFromPieceType() == WhiteKing)
        {
            board.SetWhiteCastleKing(false);
            board.SetWhiteCastleQueen(false);
        }
        else if (GetFromPieceType() == BlackKing)
        {
            board.SetBlackCastleKing(false);
            board.SetBlackCastleQueen(false);
        }
        
        switch ((CSquare)GetTo())
        {
            case A1:
                board.SetWhiteCastleQueen(false);
                break;
            case H1:
                board.SetWhiteCastleKing(false);
                break;
            case A8:
                board.SetBlackCastleQueen(false);
                break;
            case H8:
                board.SetBlackCastleKing(false);
                break;
        }

        if (CBoard.GetEnumPiece(GetFromPieceType()) != NRook) return;
        switch ((CSquare)GetFrom())
        {
            case A1:
                board.SetWhiteCastleQueen(false);
                break;
            case H1:
                board.SetWhiteCastleKing(false);
                break;
            case A8:
                board.SetBlackCastleQueen(false);
                break;
            case H8:
                board.SetBlackCastleKing(false);
                break;
        }
    }

    private void MakeCastle(CBoard board)
    {
        MakeQuiet(board, GetFrom(), GetTo(), GetFromPieceType());
        if (CBoard.GetColorPiece(GetFromPieceType()) == NWhite)
        {
            if (GetFlags() == KingCastle) MakeQuiet(board, H1, F1, WhiteRook);
            else MakeQuiet(board, A1, D1, WhiteRook);
        }
        else if (GetFlags() == KingCastle) MakeQuiet(board, H8, F8, BlackRook);
        else MakeQuiet(board, A8, D8, BlackRook);
    }

    private void UnmakeCastle(CBoard board)
    {
        MakeQuiet(board, GetTo(), GetFrom(), GetFromPieceType());
        if (CBoard.GetColorPiece(GetFromPieceType()) == NWhite)
            if (GetFlags() == KingCastle) MakeQuiet(board, F1, H1, WhiteRook);
            else MakeQuiet(board, D1, A1, WhiteRook);
        else if (GetFlags() == KingCastle) MakeQuiet(board, F8, H8, BlackRook);
        else MakeQuiet(board, D8, A8, BlackRook);
    }

    private void MakePromotion(CBoard board)
    {
        ulong to = CSquares.One << GetTo();
        MakeQuiet(board);
        board.PieceBb[(byte)NPawn] ^= to;
        board.PieceBb[(byte)GetFlags() - 5] ^= to;
    }

    private void UnmakePromotion(CBoard board)
    {
        ulong to = CSquares.One << GetTo();
        board.PieceBb[(byte)GetFlags() - 5] ^= to;
        board.PieceBb[(byte)NPawn] ^= to;
        UnmakeQuiet(board);
    }

    private void MakePromotionCapture(CBoard board)
    {
        ulong to = CSquares.One << GetTo();
        MakeCapture(board);
        board.PieceBb[(byte)NPawn] ^= to;
        board.PieceBb[(byte)GetFlags() - 9] ^= to;
    }

    private void UnmakePromotionCapture(CBoard board)
    {
        ulong to = CSquares.One << GetTo();
        board.PieceBb[(byte)GetFlags() - 9] ^= to;
        board.PieceBb[(byte)NPawn] ^= to;
        UnmakeCapture(board);
    }

    private void MakeQuiet(CBoard board) => MakeQuiet(board, GetFrom(), GetTo(), GetFromPieceType());
    private void UnmakeQuiet(CBoard board) => MakeQuiet(board, GetTo(), GetFrom(), GetFromPieceType());
    private void MakeQuiet(CBoard board, CSquare from, CSquare to, PieceType fromPieceType) => MakeQuiet(board, (byte)from, (byte)to, fromPieceType);

    private void MakeQuiet(CBoard board, byte fromB, byte toB, PieceType fromPieceType)
    {
        ulong from = CSquares.One << fromB;
        ulong to = CSquares.One << toB;
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(fromPieceType)] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetColorPiece(fromPieceType)] ^= fromTo;
    }

    private void MakeDoublePawnPush(CBoard board, byte from, byte to)
    {
        MakeQuiet(board, from, to, GetFromPieceType());
        board.SetEp(CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? from + 8 : from - 8);
    }

    private void MakeCapture(CBoard board)
    {
        ulong from = CSquares.One << GetFrom();
        ulong to = CSquares.One << GetTo();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetToPieceType())] ^= to;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetToPieceType())] ^= to;
    }

    private void UnmakeCapture(CBoard board)
    {
        ulong from = CSquares.One << GetTo();
        ulong to = CSquares.One << GetFrom();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetToPieceType())] ^= from;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetToPieceType())] ^= from;
    }

    private void MakeEpCapture(CBoard board)
    {
        ulong from = CSquares.One << GetFrom();
        ulong to = CSquares.One << GetTo();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        if (CBoard.GetColorPiece(GetFromPieceType()) == NWhite)
        {
            board.PieceBb[(byte)NPawn] ^= CSquares.One << (GetTo() - 8);
            board.PieceBb[(byte)NBlack] ^= CSquares.One << (GetTo() - 8);
        }
        else
        {
            board.PieceBb[(byte)NPawn] ^= CSquares.One << (GetTo() + 8);
            board.PieceBb[(byte)NWhite] ^= CSquares.One << (GetTo() + 8);
        }
    }

    private void UnmakeEpCapture(CBoard board)
    {
        ulong from = CSquares.One << GetTo();
        ulong to = CSquares.One << GetFrom();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        if (CBoard.GetColorPiece(GetFromPieceType()) == NWhite)
        {
            board.PieceBb[(byte)NPawn] ^= CSquares.One << (GetTo() - 8);
            board.PieceBb[(byte)NBlack] ^= CSquares.One << (GetTo() - 8);
        }
        else
        {
            board.PieceBb[(byte)NPawn] ^= CSquares.One << (GetTo() + 8);
            board.PieceBb[(byte)NWhite] ^= CSquares.One << (GetTo() + 8);
        }
    }

    public bool Equals(CMove other) => _move == other._move && _pieces == other._pieces;
    public bool NotEquals(CMove other) => _move != other._move || _pieces != other._pieces;

    private static char UnicodeFromPieceType(PieceType pieceType) =>
        pieceType switch
        {
            WhitePawn => '\u265F',
            BlackPawn => '\u2659',
            WhiteKnight => '\u265E',
            BlackKnight => '\u2658',
            WhiteBishop => '\u265D',
            BlackBishop => '\u2657',
            WhiteRook => '\u265C',
            BlackRook => '\u2656',
            WhiteQueen => '\u265B',
            BlackQueen => '\u2655',
            WhiteKing => '\u265A',
            BlackKing => '\u2654',
            _ => throw new ArgumentOutOfRangeException()
        };

    public bool IsPromotion() => GetFlags() is KnightPromotion or BishopPromotion or RookPromotion or QueenPromotion
        or KnightPromotionCapture or BishopPromotionCapture or RookPromotionCapture or QueenPromotionCapture;

    public bool IsCapture() => GetFlags() is Capture or EpCapture or KnightPromotionCapture
        or BishopPromotionCapture or RookPromotionCapture or QueenPromotionCapture;
}

public enum MoveFlag : byte
{
    QuietMove,
    DoublePawnPush,
    KingCastle,
    QueenCastle,
    Capture,
    EpCapture,
    KnightPromotion = 8,
    BishopPromotion,
    RookPromotion,
    QueenPromotion,
    KnightPromotionCapture,
    BishopPromotionCapture,
    RookPromotionCapture,
    QueenPromotionCapture
}