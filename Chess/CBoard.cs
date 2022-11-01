using System.Text;
using static Chess.CAttackMap;
using static Chess.CSquares;
using static Chess.EnumPiece;
using static Chess.PieceType;

namespace Chess;

/// <summary>
/// Represents the current state of a chess board, using Bitboards, a <see cref="Zobrist"/>-Hash, and other necessary information
/// </summary>
public class CBoard
{
    /// <summary>
    /// The 6 Bitboards, represented as a <see cref="ulong"/> each, in the following order: White, Black, Pawns, Knights, Bishops, Rooks, Queens, Kings
    /// </summary>
    public ulong[] PieceBb { get; }

    /// <summary>
    /// The color of the player to move, either <see cref="ColorType.White"/> or <see cref="ColorType.Black"/>
    /// </summary>
    public ColorType ToMove { get; set; }

    /// <summary>
    /// Contains the following information about the state of this CBoard:
    /// 4 bits for the CastlingRights, 6 for the En-Passant-Target-Square, 6 for the current Half-Move-Counter
    /// </summary>
    public ushort Info { get; set; } // Castling 4, En Passant 6, Half Moves 6

    /// <summary>
    /// The number of full moves
    /// </summary>
    public ushort FullMoves { get; set; }

    /// <summary>
    /// The <see cref="Zobrist"/>-Instance for this board, to incrementally update the hash and access it
    /// </summary>
    public Zobrist Zobrist { get; private set; }

    /// <summary>
    /// Creates a new Instance of a CBoard, with an empty board, white to move, all castling possibilities allowed, no possible En Passant Capture, 0 half and full moves and a newly created <see cref="Zobrist"/>
    /// </summary>
    public CBoard()
    {
        PieceBb = new ulong[8];
        ToMove = ColorType.White;
        Info = 0xf000;
        FullMoves = 0;
        Zobrist = new Zobrist();
    }

    /// <summary>
    /// Creates a new CBoard from the <see cref="FenReader.StartingFen"/>
    /// </summary>
    /// <returns>A new CBoard with the starting position being loaded</returns>
    public static CBoard StartingBoard() => FenReader.ImportFen(FenReader.StartingFen);

    /// <summary>
    /// Copies this CBoard instance to a newly created one
    /// </summary>
    /// <returns>The copied CBoard</returns>
    public CBoard Copy()
    {
        CBoard board = new CBoard();
        for (int i = 0; i < 8; i++) board.PieceBb[i] = PieceBb[i];
        board.ToMove = ToMove;
        board.Info = Info;
        board.FullMoves = FullMoves;
        board.Zobrist = new Zobrist(GetHash());
        return board;
    }

    /// <summary>
    /// Returns the Hash of the <see cref="Zobrist"/>-Instance associated with this CBoard
    /// </summary>
    /// <returns></returns>
    public ulong GetHash() => Zobrist.Hash;

    /// <summary>
    /// The color of the player which is currently not to move
    /// </summary>
    /// <returns></returns>
    public ColorType NotToMove() => (ColorType)(1 - (byte)ToMove);

    /// <summary>
    /// Returns a Bitboard with all bits set where a given <see cref="PieceType"/> exists
    /// </summary>
    /// <param name="pieceType">The PieceType to look for</param>
    /// <returns>a Bitboard with all bits set where a given PieceType exists</returns>
    public ulong GetPieceSet(PieceType pieceType) => PieceBb[(byte)GetEnumPiece(pieceType)] & PieceBb[(byte)GetColorPiece(pieceType)];

    /// <summary>
    /// Returns a Bitboard with all bits set where any piece exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where any piece exists</returns>
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

    /// <summary>
    /// Gets a Bitboard with all bits set where a pawn of a given <see cref="ColorType"/> exists
    /// </summary>
    /// <param name="colorType">The ColorType of the pawns to look for</param>
    /// <returns>A Bitboard with all bits set where a pawn of a given ColorType exists</returns>
    private ulong GetPawns(ColorType colorType) => GetPawns() & PieceBb[(int)colorType];

    /// <summary>
    /// Gets a Bitboard with all bits set where a knight of a given <see cref="ColorType"/> exists
    /// </summary>
    /// <param name="colorType">The ColorType of the knights to look for</param>
    /// <returns>A Bitboard with all bits set where a knight of a given ColorType exists</returns>
    private ulong GetKnights(ColorType colorType) => GetKnights() & PieceBb[(int)colorType];

    /// <summary>
    /// Gets a Bitboard with all bits set where a bishop of a given <see cref="ColorType"/> exists
    /// </summary>
    /// <param name="colorType">The ColorType of the bishops to look for</param>
    /// <returns>A Bitboard with all bits set where a bishop of a given ColorType exists</returns>
    private ulong GetBishops(ColorType colorType) => GetBishops() & PieceBb[(int)colorType];

    /// <summary>
    /// Gets a Bitboard with all bits set where a rook of a given <see cref="ColorType"/> exists
    /// </summary>
    /// <param name="colorType">The ColorType of the rooks to look for</param>
    /// <returns>A Bitboard with all bits set where a rook of a given ColorType exists</returns>
    private ulong GetRooks(ColorType colorType) => GetRooks() & PieceBb[(int)colorType];

    /// <summary>
    /// Gets a Bitboard with all bits set where a queen of a given <see cref="ColorType"/> exists
    /// </summary>
    /// <param name="colorType">The ColorType of the queens to look for</param>
    /// <returns>A Bitboard with all bits set where a queen of a given ColorType exists</returns>
    private ulong GetQueens(ColorType colorType) => GetQueens() & PieceBb[(int)colorType];

    /// <summary>
    /// Gets a Bitboard with all bits set where a king of a given <see cref="ColorType"/> exists
    /// </summary>
    /// <param name="colorType">The ColorType of the kings to look for</param>
    /// <returns>A Bitboard with all bits set where a king of a given ColorType exists</returns>
    private ulong GetKings(ColorType colorType) => GetKings() & PieceBb[(int)colorType];

    /// <summary>
    /// Gets a Bitboard with all bits set where a white piece exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a white piece exists</returns>
    public ulong GetWhite() => PieceBb[(byte)NWhite];

    /// <summary>
    /// Gets a Bitboard with all bits set where a black piece exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a black piece exists</returns>
    public ulong GetBlack() => PieceBb[(byte)NBlack];

    /// <summary>
    /// Gets a Bitboard with all bits set where a pawn exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a pawn exists</returns>
    private ulong GetPawns() => PieceBb[(byte)NPawn];

    /// <summary>
    /// Gets a Bitboard with all bits set where a knight exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a knight exists</returns>
    private ulong GetKnights() => PieceBb[(byte)NKnight];

    /// <summary>
    /// Gets a Bitboard with all bits set where a bishop exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a bishop exists</returns>
    private ulong GetBishops() => PieceBb[(byte)NBishop];

    /// <summary>
    /// Gets a Bitboard with all bits set where a rook exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a rook exists</returns>
    private ulong GetRooks() => PieceBb[(byte)NRook];

    /// <summary>
    /// Gets a Bitboard with all bits set where a queen exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a queen exists</returns>
    private ulong GetQueens() => PieceBb[(byte)NQueen];

    /// <summary>
    /// Gets a Bitboard with all bits set where a king exists
    /// </summary>
    /// <returns>A Bitboard with all bits set where a king exists</returns>
    private ulong GetKings() => PieceBb[(byte)NKing];

    /// <summary>
    /// Get whether there exists a piece on a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The byte representation of the square to check for</param>
    /// <returns>True if the square is occupied, otherwise false</returns>
    private bool GetPiece(byte square) => (GetOccupied() & (One << square)) != 0;

    /// <summary>
    /// Get whether there exists a piece on a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare to check for</param>
    /// <returns>True if the square is occupied, otherwise false</returns>
    public bool GetPiece(CSquare square) =>
        GetPiece((byte)square);

    /// <summary>
    /// Sets the value of the bitboard of an <see cref="EnumPiece"/> at a <see cref="CSquare"/>
    /// </summary>
    /// <param name="bitBoard">The EnumPiece whose bitboard will be affected</param>
    /// <param name="square">The square where to set the value</param>
    /// <param name="value">The value the square will have</param>
    public void SetSquare(EnumPiece bitBoard, CSquare square, bool value)
    {
        PieceBb[(byte)bitBoard] &= ~(One << (byte)square);
        if (value) PieceBb[(byte)bitBoard] |= One << (byte)square;
    }

    /// <summary>
    /// Sets the value at a <see cref="CSquare"/> of a <see cref="PieceType"/> to true
    /// </summary>
    /// <param name="type">The PieceType to set the value for</param>
    /// <param name="square">The CSquare to set the value at</param>
    private void SetSquare(PieceType type, CSquare square)
    {
        SetSquare(GetEnumPiece(type), square, true);
        SetSquare(GetColorPiece(type), square, true);
    }

    /// <summary>
    /// Sets the value at a <see cref="CSquare"/> of a <see cref="PieceType"/> to true
    /// </summary>
    /// <param name="type">The PieceType to set the value for</param>
    /// <param name="square">The integer representation of the CSquare to set the value at</param>
    public void SetSquare(PieceType type, int square)
    {
        SetSquare(type, (CSquare)square);
    }

    /// <summary>
    /// Swaps the color of the player who is to move:
    /// If white is to move, black will be to move, and if black is to move, white will be to move
    /// </summary>
    public void SwapToMove() => ToMove = (ColorType)(1 - (byte)ToMove);

    /// <summary>
    /// Gets whether white has the right to castle king-side
    /// </summary>
    /// <seealso cref="CUtils.GetWhiteCastleKing"/>
    /// <returns>True if white has the right to castle king-side, otherwise false</returns>
    public bool GetWhiteCastleKing() => CUtils.GetWhiteCastleKing(Info);

    /// <summary>
    /// Gets whether white has the right to castle queen-side
    /// </summary>
    /// <seealso cref="CUtils.GetWhiteCastleQueen"/>
    /// <returns>True if white has the right to castle queen-side, otherwise false</returns>
    public bool GetWhiteCastleQueen() => CUtils.GetWhiteCastleQueen(Info);

    /// <summary>
    /// Gets whether black has the right to castle king-side
    /// </summary>
    /// <seealso cref="CUtils.GetBlackCastleKing"/>
    /// <returns>True if black has the right to castle king-side, otherwise false</returns>
    public bool GetBlackCastleKing() => CUtils.GetBlackCastleKing(Info);

    /// <summary>
    /// Gets whether black has the right to castle queen-side
    /// </summary>
    /// <seealso cref="CUtils.GetBlackCastleQueen"/>
    /// <returns>True if black has the right to castle queen-side, otherwise false</returns>
    public bool GetBlackCastleQueen() => CUtils.GetBlackCastleQueen(Info);

    /// <summary>
    /// Gets all the <see cref="CastlingType"/>s which are possible regarding the castling rights of the players, not the position of the board
    /// </summary>
    /// <returns>An array containing the castling type, if allowed, or <see cref="CastlingType.Null"/>, if not, in the following order: White King, White Queen, Black King, Black Queen</returns>
    public CastlingType[] GetCastlingTypes() => CUtils.GetCastlingTypes(Info, new CastlingType[4]);

    /// <summary>
    /// Gets the current <see cref="CSquare"/> to which a pawn could capture en passant
    /// </summary>
    /// <returns>The CSquare to which a pawn could capture en passant</returns>
    private CSquare GetEnPassantTargetSquare() => (CSquare)((Info & 0xfc0) >> 6);

    /// <summary>
    /// Gets the current <see cref="CSquare"/> to which a pawn could capture en passant
    /// </summary>
    /// <returns>The CSquare to which a pawn could capture en passant</returns>
    public CSquare GetEp() => GetEnPassantTargetSquare();

    /// <summary>
    /// Gets the current Half Move Counter
    /// </summary>
    /// <returns>The number of half moves as a byte since the last capture or pawn move</returns>
    public byte GetHalfMoves() => (byte)(Info & 0x3f);

    /// <summary>
    /// Sets whether white is allowed to castle king-side
    /// </summary>
    /// <param name="castle">A boolean indicating whether white should be allowed to castle king-side</param>
    public void SetWhiteCastleKing(bool castle)
    {
        Info &= 0x7fff;
        if (castle) Info |= 1 << 15;
    }

    /// <summary>
    /// Sets whether white is allowed to castle queen-side
    /// </summary>
    /// <param name="castle">A boolean indicating whether white should be allowed to castle queen-side</param>
    public void SetWhiteCastleQueen(bool castle)
    {
        Info &= 0xbfff;
        if (castle) Info |= 1 << 14;
    }

    /// <summary>
    /// Sets whether black is allowed to castle king-side
    /// </summary>
    /// <param name="castle">A boolean indicating whether black should be allowed to castle king-side</param>
    public void SetBlackCastleKing(bool castle)
    {
        Info &= 0xdfff;
        if (castle) Info |= 1 << 13;
    }

    /// <summary>
    /// Sets whether black is allowed to castle queen-side
    /// </summary>
    /// <param name="castle">A boolean indicating whether black should be allowed to castle queen-side</param>
    public void SetBlackCastleQueen(bool castle)
    {
        Info &= 0xefff;
        if (castle) Info |= 1 << 12;
    }

    /// <summary>
    /// Sets the castling rights contained in the array to false
    /// </summary>
    /// <param name="castlingTypes">An array containing the castling rights to unset in the following order, or <see cref="CastlingType.Null"/>, if not to unset: White King, White Queen, Black King, Black Queen</param>
    public void UnsetCastlingTypes(CastlingType[] castlingTypes)
    {
        if (castlingTypes[0] == CastlingType.WhiteCastleKing) SetWhiteCastleKing(false);
        if (castlingTypes[1] == CastlingType.WhiteCastleQueen) SetWhiteCastleQueen(false);
        if (castlingTypes[2] == CastlingType.BlackCastleKing) SetBlackCastleKing(false);
        if (castlingTypes[3] == CastlingType.BlackCastleQueen) SetBlackCastleQueen(false);
    }

    /// <summary>
    /// Sets the En Passant Target Square to a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare represented as an <see cref="int"/> to set to</param>
    public void SetEp(int square)
    {
        Info &= 0xf03f;
        Info |= (ushort)(square << 6);
    }

    /// <summary>
    /// Sets the En Passant Target Square to a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare to set to</param>
    public void SetEnPassantTargetSquare(CSquare square) => SetEp((byte)square);

    /// <summary>
    /// Sets the current Half Move Counter to a given number
    /// </summary>
    /// <param name="halfMoves">The number of half moves to set to, represented as a <see cref="byte"/></param>
    public void SetHalfMoves(byte halfMoves)
    {
        Info &= 0xffc0;
        Info |= (ushort)(halfMoves & 0x3f);
    }

    /// <summary>
    /// Determines whether there exists a piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare to determine at</param>
    /// <returns>True, if there exists a piece at the CSquare, otherwise false</returns>
    public bool GetOccupied(CSquare square) => GetOccupied((byte)square);

    /// <summary>
    /// Determines whether there exists a piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare represented as a byte to determine at</param>
    /// <returns>True, if there exists a piece at the CSquare, otherwise false</returns>
    public bool GetOccupied(byte square) => ((One << square) & GetOccupied()) != 0;

    /// <summary>
    /// Determines the <see cref="ColorType"/> of the piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare to determine at</param>
    /// <returns><see cref="ColorType.White"/>, if there is a white piece at the given square, else <see cref="ColorType.Black"/>, if there is a black piece, otherwise <see cref="ColorType.None"/></returns>
    public ColorType GetColor(CSquare square) => GetColor((byte)square);

    /// <summary>
    /// Determines the <see cref="ColorType"/> of the piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare represented as a byte to determine at</param>
    /// <returns><see cref="ColorType.White"/>, if there is a white piece at the given square, else <see cref="ColorType.Black"/>, if there is a black piece, otherwise <see cref="ColorType.None"/></returns>
    public ColorType GetColor(byte square) =>
        (GetWhite() & (One << square)) != 0 ? ColorType.White :
        (GetBlack() & (One << square)) != 0 ? ColorType.Black : ColorType.None;

    /// <summary>
    /// Determines the <see cref="PieceType"/> of the piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare represented as a byte to determine at</param>
    /// <returns><see cref="PieceType.Empty"/>, if there is no piece at the given square, else the PieceType of the piece at the square</returns>
    /// <exception cref="ApplicationException">If the given CSquare is neither empty nor occupied - Internal Board Representation Failure</exception>
    public PieceType GetPieceType(byte square)
    {
        ulong shift = One << square;
        if ((shift & (PieceBb[0] | PieceBb[1])) == 0) return Empty;
        if ((shift & PieceBb[2]) != 0) return (shift & PieceBb[0]) != 0 ? WhitePawn : BlackPawn;
        if ((shift & PieceBb[3]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteKnight : BlackKnight;
        if ((shift & PieceBb[4]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteBishop : BlackBishop;
        if ((shift & PieceBb[5]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteRook : BlackRook;
        if ((shift & PieceBb[6]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteQueen : BlackQueen;
        if ((shift & PieceBb[7]) != 0) return (shift & PieceBb[0]) != 0 ? WhiteKing : BlackKing;
        throw new ApplicationException();
    }

    /// <summary>
    /// Gets the <see cref="PieceType"/>s at each <see cref="CSquare"/> 
    /// </summary>
    /// <returns>A 64 long <see cref="SquarePieceTypePair"/>-Array, containing the PieceTypes for each CSquare</returns>
    public SquarePieceTypePair?[] GetPieceTypes()
    {
        SquarePieceTypePair?[] pairs = new SquarePieceTypePair[64];
        int pairIndex = 0;
        for (byte sq = 0; sq < 64; sq++)
        {
            PieceType type = GetPieceType(sq);
            if (type == Empty) continue;
            pairs[pairIndex] = new SquarePieceTypePair(sq, type);
            pairIndex++;
        }

        return pairs;
    }

    /// <summary>
    /// Determines the <see cref="EnumPiece"/> of the piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare to determine at</param>
    /// <returns>The EnumPiece at the CSquare</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the given CSquare is empty</exception>
    public EnumPiece GetEnumPiece(CSquare square) => GetEnumPiece((byte)square);

    /// <summary>
    /// Determines the <see cref="EnumPiece"/> of the piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare represented as a byte to determine at</param>
    /// <returns>The EnumPiece at the CSquare</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the given CSquare is empty</exception>
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

    /// <summary>
    /// Determines the <see cref="PieceType"/> of the piece at a given <see cref="CSquare"/>
    /// </summary>
    /// <param name="square">The CSquare to determine at</param>
    /// <returns><see cref="PieceType.Empty"/>, if there is no piece at the given square, else the PieceType of the piece at the square</returns>
    /// <exception cref="ApplicationException">If the given CSquare is neither empty nor occupied - Internal Board Representation Failure</exception>
    public PieceType GetPieceType(CSquare square) => GetPieceType((byte)square);

    /// <summary>
    /// Determines whether a given <see cref="CSquare"/> is attacked by a given side
    /// </summary>
    /// <param name="square">The CSquare to determine at</param>
    /// <param name="bySide">The <see cref="ColorType"/> of which to determine the attacks</param>
    /// <returns>True, if the CSquare is under attack by the given ColorType, otherwise false</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="bySide"/> equals <see cref="ColorType.None"/></exception>
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

    /// <summary>
    /// A constant used to determine the PopCount
    /// </summary>
    private const ulong K1 = 0x5555555555555555;
    
    /// <summary>
    /// A constant used to determine the PopCount
    /// </summary>
    private const ulong K2 = 0x3333333333333333;
    
    /// <summary>
    /// A constant used to determine the PopCount
    /// </summary>
    private const ulong K4 = 0x0f0f0f0f0f0f0f0f;
    
    /// <summary>
    /// A constant used to determine the PopCount
    /// </summary>
    private const ulong Kf = 0x0101010101010101;

    /// <summary>
    /// Determines the Population-Count of a given <see cref="PieceType"/>
    /// </summary>
    /// <param name="pieceType">The PieceType of which to determine the Population-Count</param>
    /// <returns>The number of pieces of the given PieceType on this CBoard</returns>
    private byte PopCount(PieceType pieceType) => PopCount(GetPieceSet(pieceType));

    /// <summary>
    /// Determines the Population-Count of a given <see cref="PieceType"/>
    /// </summary>
    /// <param name="i">The PieceType represented as an <see cref="int"/> of which to determine the Population-Count</param>
    /// <returns>The number of pieces of the given PieceType on this CBoard</returns>
    public byte PopCount(int i) => PopCount((PieceType)i);

    /// <summary>
    /// Determines the number of bits set to one in a give <see cref="ulong"/>
    /// </summary>
    /// <param name="u">The ulong of which to determine the number of bits set to one</param>
    /// <returns>The number of bits set to one as a byte</returns>
    private byte PopCount(ulong u)
    {
        if (u == 0) return 0;
        u -= ((u >> 1) & K1);
        u = (u & K2) + ((u >> 2) & K2);
        u = (u + (u >> 4)) & K4;
        u = (u * Kf) >> 56;
        return (byte)u;
    }

    /// <summary>
    /// Determines whether a given side is in check
    /// </summary>
    /// <param name="side">The <see cref="ColorType"/> of the side of which it should be determined</param>
    /// <returns>True, if the King of the given side is attacked, otherwise false</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="side"/> equals <see cref="ColorType.None"/></exception>
    public bool InCheck(ColorType side)
    {
        if (side == ColorType.None) throw new ArgumentOutOfRangeException();
        return Attacked((CSquare)CUtils.BitScanForwardIsolatedLs1B(GetKings() & PieceBb[(byte)side]), 1 - side);
    }

    /// <summary>
    /// Builds a readable String-Representation of this CBoard
    /// </summary>
    /// <returns>The String-Representation of this CBoard</returns>
    public string ToDisplayString()
    {
        StringBuilder builder = new StringBuilder();
        for (int r8 = 56; r8 >= 0; r8 -= 8)
        {
            for (int i = 0; i <= 32; i++) builder.Append(i % 4 == 0 ? '+' : '-');
            builder.Append('\n');
            for (int f = 0; f < 8; f++)
            {
                builder.Append("| ");
                builder.Append(FenReader.FenCharFromPieceType(GetPieceType((byte)(r8 + f)))).Append(' ');
            }

            builder.Append('|');
            builder.Append('\n');
        }

        for (int i = 0; i <= 32; i++) builder.Append(i % 4 == 0 ? '+' : '-');
        return builder.ToString();
    }

    /// <summary>
    /// Converts a given <see cref="PieceType"/> to an <see cref="EnumPiece"/>, holding the PieceType of the PieceType
    /// </summary>
    /// <param name="pieceType">The PieceType to convert</param>
    /// <returns>The EnumPiece to which the PieceType belongs</returns>
    /// <exception cref="ArgumentOutOfRangeException">If <paramref name="pieceType"/> equals <see cref="PieceType.Empty"/></exception>
    public static EnumPiece GetEnumPiece(PieceType pieceType) =>
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

    /// <summary>
    /// Converts a given <see cref="PieceType"/> to an <see cref="EnumPiece"/>, holding the color of the PieceType
    /// </summary>
    /// <param name="pieceType">The PieceType to convert</param>
    /// <returns>The EnumPiece to which the PieceType belongs</returns>
    public static EnumPiece GetColorPiece(PieceType pieceType) =>
        pieceType == Empty
            ? throw new ArgumentOutOfRangeException()
            : pieceType is WhitePawn or WhiteKnight or WhiteBishop or WhiteRook or WhiteQueen or WhiteKing
                ? NWhite
                : NBlack;
}