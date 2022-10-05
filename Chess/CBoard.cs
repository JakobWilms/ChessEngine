using System;
using ChessEngine;
using static Chess.CAttackMap;
using static Chess.CSquares;
using static Chess.EnumPiece;
using static Chess.PieceType;

namespace Chess;

public class CBoard
{
    public ulong[] PieceBb { get; } // White, Black, Pawn, Knight, Bishop, Rook, Queen, King
    public ColorType ToMove { get; set; }
    public ushort Info { get; set; } // Castling 4, En Passant 6, Half Moves 6
    public ushort FullMoves { get; set; }

    public CBoard()
    {
        PieceBb = new ulong[8];
        ToMove = ColorType.White;
        Info = 0xf000;
        FullMoves = 0;
    }

    public ColorType NotToMove() => (ColorType)(1 - (byte)ToMove);
    public ulong GetPieceSet(PieceType pieceType) => PieceBb[PieceCode(pieceType)] & PieceBb[ColorCode(pieceType)];
    public ulong GetOccupied() => GetWhite() | GetBlack();
    public ulong GetWhitePawns() => PieceBb[(int)NPawn] & PieceBb[(int)NWhite];
    public ulong GetBlackPawns() => PieceBb[(int)NPawn] & PieceBb[(int)NBlack];
    public ulong GetWhiteKnights() => PieceBb[(int)NKnight] & PieceBb[(int)NWhite];
    public ulong GetBlackKnights() => PieceBb[(int)NKnight] & PieceBb[(int)NBlack];
    public ulong GetWhiteBishops() => PieceBb[(int)NBishop] & PieceBb[(int)NWhite];
    public ulong GetBlackBishops() => PieceBb[(int)NBishop] & PieceBb[(int)NBlack];
    public ulong GetWhiteRooks() => PieceBb[(int)NRook] & PieceBb[(int)NWhite];
    public ulong GetBlackRooks() => PieceBb[(int)NRook] & PieceBb[(int)NBlack];
    public ulong GetWhiteQueens() => PieceBb[(int)NQueen] & PieceBb[(int)NWhite];
    public ulong GetBlackQueens() => PieceBb[(int)NQueen] & PieceBb[(int)NBlack];
    public ulong GetWhiteKings() => PieceBb[(int)NKing] & PieceBb[(int)NWhite];
    public ulong GetBlackKings() => PieceBb[(int)NKing] & PieceBb[(int)NBlack];

    public ulong GetPawns(ColorType colorType) => GetPawns() & PieceBb[(int)colorType];
    public ulong GetKnights(ColorType colorType) => GetKnights() & PieceBb[(int)colorType];
    public ulong GetBishops(ColorType colorType) => GetBishops() & PieceBb[(int)colorType];
    public ulong GetRooks(ColorType colorType) => GetRooks() & PieceBb[(int)colorType];
    public ulong GetQueens(ColorType colorType) => GetQueens() & PieceBb[(int)colorType];
    public ulong GetKings(ColorType colorType) => GetKings() & PieceBb[(int)colorType];

    public ulong GetWhite() => PieceBb[(byte)NWhite];
    public ulong GetBlack() => PieceBb[(byte)NBlack];
    public ulong GetPawns() => PieceBb[(byte)NPawn];
    public ulong GetKnights() => PieceBb[(byte)NKnight];
    public ulong GetBishops() => PieceBb[(byte)NBishop];
    public ulong GetRooks() => PieceBb[(byte)NRook];
    public ulong GetQueens() => PieceBb[(byte)NQueen];
    public ulong GetKings() => PieceBb[(byte)NKing];

    public bool GetPiece(byte square) => (GetOccupied() & (One << square)) != 0;
    
    public bool GetPiece(CSquare square) =>
        GetPiece((byte)square);

    public bool GetSquare(EnumPiece bitBoard, CSquare square) =>
        Convert.ToBoolean((PieceBb[(byte)bitBoard] & (One << (byte)square)) >> (byte)square);

    public void SetSquare(EnumPiece bitBoard, CSquare square, bool value)
    {
        PieceBb[(byte)bitBoard] &= ~(One << (byte)square);
        if (value) PieceBb[(byte)bitBoard] |= One << (byte)square;
    }

    public void UnsetSquare(PieceType type, CSquare square)
    {
        SetSquare(GetEnumPiece(type), square, false);
        SetSquare(GetColorPiece(type), square, false);
    }

    public void SetSquare(PieceType type, CSquare square)
    {
        SetSquare(GetEnumPiece(type), square, true);
        SetSquare(GetColorPiece(type), square, true);
    }

    public void SetSquare(PieceType type, int square)
    {
        SetSquare(type, (CSquare)square);
    }

    public void SwapToMove() => ToMove = (ColorType)(1 - (byte)ToMove);

    public bool GetWhiteCastleKing() => Convert.ToBoolean((Info & 0x8000) >> 15);
    public bool GetWhiteCastleQueen() => Convert.ToBoolean((Info & 0x4000) >> 14);
    public bool GetBlackCastleKing() => Convert.ToBoolean((Info & 0x2000) >> 13);
    public bool GetBlackCastleQueen() => Convert.ToBoolean((Info & 0x1000) >> 12);

    public CSquare GetEnPassantTargetSquare() => (CSquare)((Info & 0xfc0) >> 6);
    public CSquare GetEp() => GetEnPassantTargetSquare();
    public byte GetHalfMoves() => (byte)(Info & 0x3f);

    public void SetWhiteCastleKing(bool castle)
    {
        Info &= 0x7fff;
        if (castle) Info |= 1 << 15;
        //if (!castle) throw new Exception();
    }

    public void SetWhiteCastleQueen(bool castle)
    {
        Info &= 0xbfff;
        if (castle) Info |= 1 << 14;
    }

    public void SetBlackCastleKing(bool castle)
    {
        Info &= 0xdfff;
        if (castle) Info |= 1 << 13;
    }

    public void SetBlackCastleQueen(bool castle)
    {
        Info &= 0xefff;
        if (castle) Info |= 1 << 12;
    }

    public void SetEp(int square)
    {
        Info &= 0xf030;
        Info |= (ushort)(square << 6);
    }

    public void SetEnPassantTargetSquare(CSquare square) => SetEp((byte)square);

    public void SetHalfMoves(byte halfMoves)
    {
        Info &= 0xffa0;
        Info |= (ushort)(halfMoves & 0x3f);
    }

    public bool GetOccupied(CSquare square) => GetOccupied((byte)square);
    
    public bool GetOccupied(byte square) => ((One << square) & GetOccupied()) != 0;

    public ColorType GetColor(CSquare square) => GetColor((byte)square);
    
    public ColorType GetColor(byte square) =>
        (GetWhite() & (One << square)) != 0 ? ColorType.White :
        (GetBlack() & (One << square)) != 0 ? ColorType.Black : ColorType.None;

    public PieceType GetPieceType(byte square)
    {
        ulong shift = One << square;
        if ((shift & GetOccupied()) == 0) return Empty;
        if ((shift & PieceBb[2]) != 0) return (shift & PieceBb[0]) != 0 ? WhitePawn : BlackPawn;
        if ((shift & PieceBb[3]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteKnight : BlackKnight;
        if ((shift & PieceBb[4]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteBishop : BlackBishop;
        if ((shift & PieceBb[5]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteRook : BlackRook;
        if ((shift & PieceBb[6]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteQueen : BlackQueen;
        if ((shift & PieceBb[7]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteKing : BlackKing;
        throw new ArgumentOutOfRangeException();
    }

    public EnumPiece GetEnumPiece(CSquare square) => GetEnumPiece((byte)square);
    
    public EnumPiece GetEnumPiece(byte square)
    {
        if (!GetOccupied(square)) throw new ArgumentOutOfRangeException();
        ulong shift = One << square;
        if ((shift & PieceBb[2]) != 0) return NPawn;
        if ((shift & PieceBb[3]) != 0) return NKnight;
        if ((shift & PieceBb[4]) != 0) return NBishop;
        if ((shift & PieceBb[5]) != 0) return NRook;
        if ((shift & PieceBb[6]) != 0) return NQueen;
        if ((shift & PieceBb[7]) != 0) return NKing;
        throw new ArgumentOutOfRangeException();
    }

    public PieceType GetPieceType(CSquare square) => GetPieceType((byte)square);

    public bool Attacked(CSquare square, ColorType bySide)
    {
        if (bySide == ColorType.None) throw new ArgumentOutOfRangeException();
        if ((ArrPawnAttacks[1 - (byte)bySide][(int)square] & GetPawns(bySide)) != 0) return true;
        if ((ArrKnightAttacks[(int)square] & GetKnights(bySide)) != 0) return true;
        if ((ArrKingAttacks[(int)square] & GetKings(bySide)) != 0) return true;
        ulong bishopsQueens = GetQueens(bySide) | GetBishops(bySide);
        if ((CAttacks.BishopAttacks(GetOccupied(), square) & bishopsQueens) != 0) return true;
        ulong rooksQueens = GetQueens(bySide) | GetRooks(bySide);
        return (CAttacks.RookAttacks(GetOccupied(), square) & rooksQueens) != 0;
    }

    public bool InCheck(PieceType type) => InCheck((ColorType)ColorCode(type));

    public bool InCheck(ColorType side)
    {
        if (side == ColorType.None) throw new ArgumentOutOfRangeException();
        return Attacked((CSquare)CAttacks.BitScanForwardIsolatedLs1B(GetKings() & PieceBb[(byte)side]), 1 - side);
    }

    public  static EnumPiece GetEnumPiece(PieceType pieceType) =>
        pieceType switch
        {
            WhitePawn => NPawn,
            BlackPawn => NPawn,
            WhiteKnight => NKnight,
            BlackKnight => NKnight,
            WhiteBishop => NBishop,
            BlackBishop => NBishop,
            WhiteRook => NRook,
            BlackRook => NRook,
            WhiteQueen => NQueen,
            BlackQueen => NQueen,
            WhiteKing => NKing,
            BlackKing => NKing,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static EnumPiece GetColorPiece(PieceType pieceType) =>
        pieceType == Empty
            ? throw new ArgumentOutOfRangeException()
            : pieceType is WhitePawn or WhiteKnight or WhiteBishop or WhiteRook or WhiteQueen or WhiteKing
                ? NWhite
                : NBlack;

    public static byte PieceCode(PieceType pieceType) =>
        pieceType switch
        {
            WhitePawn => 2,
            BlackPawn => 2,
            WhiteKnight => 3,
            BlackKnight => 3,
            WhiteBishop => 4,
            BlackBishop => 4,
            WhiteRook => 5,
            BlackRook => 5,
            WhiteQueen => 6,
            BlackQueen => 6,
            WhiteKing => 7,
            BlackKing => 7,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static byte ColorCode(PieceType pieceType) =>
        pieceType == Empty
            ? throw new ArgumentOutOfRangeException()
            : pieceType is WhitePawn or WhiteKnight or WhiteBishop or WhiteRook or WhiteQueen or WhiteKing
                ? (byte)0
                : (byte)1;
}