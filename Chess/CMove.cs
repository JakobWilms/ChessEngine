using System;
using static Chess.CSquare;
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

    public string ToDisplayMove()
    {
        switch (GetFlags())
        {
            case MoveFlag.QuietMove:
            case MoveFlag.DoublePawnPush:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" + (CSquare)GetTo();
            case MoveFlag.KingCastle:
                return "0-0";
            case MoveFlag.QueenCastle:
                return "0-0-0";
            case MoveFlag.Capture:
            case MoveFlag.EpCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" + (CSquare)GetTo();
            case MoveFlag.KnightPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteKnight
                           : BlackKnight);
            case MoveFlag.BishopPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteBishop
                           : BlackBishop);
            case MoveFlag.RookPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteRook
                           : BlackRook);
            case MoveFlag.QueenPromotion:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteQueen
                           : BlackQueen);
            case MoveFlag.KnightPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteKnight
                           : BlackKnight);
            case MoveFlag.BishopPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteBishop
                           : BlackBishop);
            case MoveFlag.RookPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteRook
                           : BlackRook);
            case MoveFlag.QueenPromotionCapture:
                return UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" + (CSquare)GetTo() + "=" +
                       UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite
                           ? WhiteQueen
                           : BlackQueen);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public byte GetTo() => (byte)(_move & 0x3f);
    public byte GetFrom() => (byte)((_move >> 6) & 0x3f);
    public MoveFlag GetFlags() => (MoveFlag)((_move >> 12) & 0x0f);
    public byte GetToPiece() => (byte)(_pieces & 0xf);
    private PieceType GetToPieceType() => (PieceType)GetToPiece();
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
            case MoveFlag.QuietMove:
                MakeQuiet(board, GetFrom(), GetTo());
                break;
            case MoveFlag.DoublePawnPush:
                MakeDoublePawnPush(board, GetFrom(), GetTo());
                break;
            case MoveFlag.KingCastle:
            case MoveFlag.QueenCastle:
                MakeCastle(board);
                break;
            case MoveFlag.Capture:
                MakeCapture(board);
                break;
            case MoveFlag.EpCapture:
                MakeEpCapture(board);
                break;
            case MoveFlag.KnightPromotion:
            case MoveFlag.BishopPromotion:
            case MoveFlag.RookPromotion:
            case MoveFlag.QueenPromotion:
                MakePromotion(board);
                break;
            case MoveFlag.KnightPromotionCapture:
            case MoveFlag.BishopPromotionCapture:
            case MoveFlag.RookPromotionCapture:
            case MoveFlag.QueenPromotionCapture:
                MakePromotionCapture(board);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (board.ToMove == ColorType.Black) board.FullMoves++;
        board.SwapToMove();
        board.SetHalfMoves((byte)(board.GetHalfMoves() + 1));
    }

    public void Unmake(CBoard board)
    {
        board.Info = _prevInfo;
        switch (GetFlags())
        {
            case MoveFlag.QuietMove:
            case MoveFlag.DoublePawnPush:
                MakeQuiet(board, GetTo(), GetFrom());
                break;
            case MoveFlag.KingCastle:
            case MoveFlag.QueenCastle:
                UnmakeCastle(board);
                break;
            case MoveFlag.Capture:
                UnmakeCapture(board);
                break;
            case MoveFlag.EpCapture:
                UnmakeEpCapture(board);
                break;
            case MoveFlag.KnightPromotion:
            case MoveFlag.BishopPromotion:
            case MoveFlag.RookPromotion:
            case MoveFlag.QueenPromotion:
                UnmakePromotion(board);
                break;
            case MoveFlag.KnightPromotionCapture:
            case MoveFlag.BishopPromotionCapture:
            case MoveFlag.RookPromotionCapture:
            case MoveFlag.QueenPromotionCapture:
                UnmakePromotionCapture(board);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (board.ToMove == ColorType.White) board.FullMoves--;
        board.SwapToMove();
        board.SetHalfMoves((byte)(board.GetHalfMoves() - 1));
    }

    private void MakeCastle(CBoard board)
    {
        MakeQuiet(board, GetFrom(), GetTo());
        if (CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite)
        {
            if (GetFlags() == MoveFlag.KingCastle) MakeQuiet(board, H1, F1);
            else MakeQuiet(board, A1, D1);
        }
        else
        {
            if (GetFlags() == MoveFlag.KingCastle) MakeQuiet(board, H8, F8);
            else MakeQuiet(board, A8, D8);
        }
    }

    private void UnmakeCastle(CBoard board)
    {
        MakeQuiet(board, GetTo(), GetFrom());
        if (CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite)
            if (GetFlags() == MoveFlag.KingCastle) MakeQuiet(board, F1, H1);
            else MakeQuiet(board, D1, A1);
        else if (GetFlags() == MoveFlag.KingCastle) MakeQuiet(board, F8, H8);
        else MakeQuiet(board, D8, A8);
    }

    private void MakePromotion(CBoard board)
    {
        ulong from = CSquares.One << GetFrom();
        ulong to = CSquares.One << GetTo();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetFromPieceType())] ^= from;
        if (GetFlags() == MoveFlag.KnightPromotion)
            board.SetSquare(EnumPiece.NKnight, (CSquare)GetTo(), true);
        else if (GetFlags() == MoveFlag.BishopPromotion)
            board.SetSquare(EnumPiece.NBishop, (CSquare)GetTo(), true);
        else if (GetFlags() == MoveFlag.RookPromotion)
            board.SetSquare(EnumPiece.NRook, (CSquare)GetTo(), true);
        else if (GetFlags() == MoveFlag.QueenPromotion) board.SetSquare(EnumPiece.NQueen, (CSquare)GetTo(), true);
    }

    private void UnmakePromotion(CBoard board)
    {
        ulong from = CSquares.One << GetTo();
        ulong to = CSquares.One << GetFrom();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        if (GetFlags() == MoveFlag.KnightPromotion)
            board.SetSquare(EnumPiece.NKnight, (CSquare)GetTo(), false);
        else if (GetFlags() == MoveFlag.BishopPromotion)
            board.SetSquare(EnumPiece.NBishop, (CSquare)GetTo(), false);
        else if (GetFlags() == MoveFlag.RookPromotion)
            board.SetSquare(EnumPiece.NRook, (CSquare)GetTo(), false);
        else if (GetFlags() == MoveFlag.QueenPromotion) board.SetSquare(EnumPiece.NQueen, (CSquare)GetTo(), false);
        board.PieceBb[(byte)EnumPiece.NPawn] ^= to;
    }

    private void MakePromotionCapture(CBoard board)
    {
        ulong from = CSquares.One << GetFrom();
        ulong to = CSquares.One << GetTo();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetFromPieceType())] ^= from;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetToPieceType())] ^= to;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetToPieceType())] ^= to;
        if (GetFlags() == MoveFlag.KnightPromotionCapture)
            board.SetSquare(EnumPiece.NKnight, (CSquare)GetTo(), true);
        else if (GetFlags() == MoveFlag.BishopPromotionCapture)
            board.SetSquare(EnumPiece.NBishop, (CSquare)GetTo(), true);
        else if (GetFlags() == MoveFlag.RookPromotionCapture)
            board.SetSquare(EnumPiece.NRook, (CSquare)GetTo(), true);
        else if (GetFlags() == MoveFlag.QueenPromotionCapture)
            board.SetSquare(EnumPiece.NQueen, (CSquare)GetTo(), true);
    }

    private void UnmakePromotionCapture(CBoard board)
    {
        ulong from = CSquares.One << GetTo();
        ulong to = CSquares.One << GetFrom();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        if (GetFlags() == MoveFlag.KnightPromotion)
            board.SetSquare(EnumPiece.NKnight, (CSquare)GetTo(), false);
        else if (GetFlags() == MoveFlag.BishopPromotion)
            board.SetSquare(EnumPiece.NBishop, (CSquare)GetTo(), false);
        else if (GetFlags() == MoveFlag.RookPromotion)
            board.SetSquare(EnumPiece.NRook, (CSquare)GetTo(), false);
        else if (GetFlags() == MoveFlag.QueenPromotion) board.SetSquare(EnumPiece.NQueen, (CSquare)GetTo(), false);
        board.PieceBb[(byte)EnumPiece.NPawn] ^= to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetToPieceType())] ^= from;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetToPieceType())] ^= from;
    }

    private void MakeQuiet(CBoard board, CSquare from, CSquare to) => MakeQuiet(board, (byte)from, (byte)to);

    private void MakeQuiet(CBoard board, byte fromB, byte toB)
    {
        ulong from = CSquares.One << fromB;
        ulong to = CSquares.One << toB;
        ulong fromTo = from ^ to;
        PieceType type = board.GetPieceType(fromB);
        board.PieceBb[(byte)CBoard.GetEnumPiece(type)] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetColorPiece(type)] ^= fromTo;
        if (type == WhiteKing)
        {
            //Console.WriteLine(this);
            board.SetWhiteCastleKing(false);
            board.SetWhiteCastleQueen(false);
        }
        else if (type == BlackKing)
        {
            board.SetBlackCastleKing(false);
            board.SetBlackCastleQueen(false);
        }

        if (CBoard.GetEnumPiece(type) != EnumPiece.NRook) return;
        switch ((CSquare)fromB)
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

    private void MakeDoublePawnPush(CBoard board, byte from, byte to)
    {
        MakeQuiet(board, from, to);
        board.SetEp(CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite ? from + 8 : from - 8);
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
        if (CBoard.GetEnumPiece(GetToPieceType()) != EnumPiece.NRook) return;
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
        if (CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite)
        {
            board.PieceBb[(byte)EnumPiece.NPawn] ^= CSquares.One << (GetTo() - 8);
            board.PieceBb[(byte)EnumPiece.NBlack] ^= CSquares.One << (GetTo() - 8);
        }
        else
        {
            board.PieceBb[(byte)EnumPiece.NPawn] ^= CSquares.One << (GetTo() + 8);
            board.PieceBb[(byte)EnumPiece.NWhite] ^= CSquares.One << (GetTo() + 8);
        }
    }

    private void UnmakeEpCapture(CBoard board)
    {
        ulong from = CSquares.One << GetTo();
        ulong to = CSquares.One << GetFrom();
        ulong fromTo = from ^ to;
        board.PieceBb[(byte)CBoard.GetEnumPiece(GetFromPieceType())] ^= fromTo;
        board.PieceBb[(byte)CBoard.GetColorPiece(GetFromPieceType())] ^= fromTo;
        if (CBoard.GetColorPiece(GetFromPieceType()) == EnumPiece.NWhite)
        {
            board.PieceBb[(byte)EnumPiece.NPawn] ^= CSquares.One << (GetTo() - 8);
            board.PieceBb[(byte)EnumPiece.NBlack] ^= CSquares.One << (GetTo() - 8);
        }
        else
        {
            board.PieceBb[(byte)EnumPiece.NPawn] ^= CSquares.One << (GetTo() + 8);
            board.PieceBb[(byte)EnumPiece.NWhite] ^= CSquares.One << (GetTo() + 8);
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