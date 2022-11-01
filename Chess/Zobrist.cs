namespace Chess;

public class Zobrist
{
    public ulong Hash { get; private set; }
    
    public Zobrist(){}
    public Zobrist(ulong hash) => Hash = hash;

    public void Make(CMove move)
    {
        if (move.GetFromPieceType() == PieceType.Empty) throw new ArgumentException();
        Hash ^= ZobristHashes.Instance.PieceHash(move.GetFromPieceType(), move.GetFromSquare()) ^ // Remove FromPiece
                ZobristHashes.Instance.ToMoveHash(); // Change ToMove
        if (move.IsCastle())
            switch (move.GetFlags(), CBoard.GetColorPiece(move.GetFromPieceType()))
            {
                case (MoveFlag.KingCastle, EnumPiece.NWhite):
                    Hash ^= ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.H1) ^
                            ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.F1);
                    break;
                case (MoveFlag.QueenCastle, EnumPiece.NWhite):
                    Hash ^= ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.A1) ^
                            ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.D1);
                    break;
                case (MoveFlag.KingCastle, EnumPiece.NBlack):
                    Hash ^= ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.H8) ^
                            ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.F8);
                    break;
                case (MoveFlag.QueenCastle, EnumPiece.NBlack):
                    Hash ^= ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.A8) ^
                            ZobristHashes.Instance.PieceHash(PieceType.WhiteRook, CSquare.D8);
                    break;
            }

        if (move.IsCapture())
            Hash ^= ZobristHashes.Instance.PieceHash(move.GetToPieceType(), move.GetToSquare()); // Remove ToPiece
        if (move.IsPromotion())
            Hash ^= ZobristHashes.Instance.PieceHash(move.GetPromotionPieceType(), move.GetToSquare()); // Set NewSquare
        else
            Hash ^= ZobristHashes.Instance.PieceHash(move.GetFromPieceType(), move.GetToSquare()); // Set NewSquare
        var castlingTypes = move.UnsetCastlingTypes();
        for (var index = 0; index < castlingTypes.Length; index++)
        {
            var castlingType = castlingTypes[index];
            Hash ^= ZobristHashes.Instance.CastlingHash(castlingType);
        }

        CSquare prevEp = CUtils.GetEnPassantTargetSquare(move.PrevInfo);
        if (prevEp != CSquare.A1) Hash ^= ZobristHashes.Instance.EnPassantHash(CSquares.FileOf(prevEp)); // Remove OldEp
        CSquare newEp = move.NewEp();
        if (newEp != CSquare.A1) Hash ^= ZobristHashes.Instance.EnPassantHash(CSquares.FileOf(newEp)); // Set NewEp
    }

    public void Unmake(CMove move) => Make(move);

    public void Import(CBoard board)
    {
        SquarePieceTypePair?[] pairs = board.GetPieceTypes();
        Hash = ZobristHashes.Instance.PieceHash(pairs[0]!);
        for (var index = 1; index < pairs.Length; index++)
        {
            var pair = pairs[index];
            if (pair == null) break;
            Hash ^= ZobristHashes.Instance.PieceHash(pair);
        }

        if (board.ToMove == ColorType.Black) Hash ^= ZobristHashes.Instance.ToMoveHash();
        foreach (CastlingType type in board.GetCastlingTypes()) Hash ^= ZobristHashes.Instance.CastlingHash(type);
        CSquare ep = board.GetEp();
        if (ep != CSquare.A1) Hash ^= ZobristHashes.Instance.EnPassantHash(CSquares.FileOf(ep));
    }
}

public class ZobristHashes
{
    public static readonly ZobristHashes Instance = new(1070372);

    private readonly ulong[][] _pieces; // PieceType, Square
    private ulong _toMove;
    private readonly ulong[] _castling;
    private readonly ulong[] _enPassant;

    private ZobristHashes(ulong seed)
    {
        _pieces = new ulong[12][];
        _castling = new ulong[5];
        _enPassant = new ulong[8];
        Init(seed);
    }

    public ulong PieceHash(PieceType pieceType, byte square) => _pieces[(byte)pieceType][square];
    public ulong PieceHash(PieceType pieceType, CSquare square) => _pieces[(byte)pieceType][(byte)square];
    public ulong PieceHash(SquarePieceTypePair pair) => PieceHash(pair.Type, pair.Square);
    public ulong ToMoveHash() => _toMove;
    public ulong CastlingHash(CastlingType castlingType) => _castling[(byte)castlingType];
    public ulong CastlingHash(byte castlingType) => _castling[castlingType];
    public ulong EnPassantHash(File file) => _enPassant[(byte)file];

    private void Init(ulong seed)
    {
        CPrng cPrng = new CPrng(seed);
        for (int pt = 0; pt < 12; pt++)
        {
            _pieces[pt] = new ulong[64];
            for (int sq = 0; sq < 64; sq++) _pieces[pt][sq] = cPrng.Next();
        }

        _toMove = cPrng.Next();
        _castling[0] = 0UL;
        for (int i = 1; i < 5; i++) _castling[i] = cPrng.Next();
        for (int f = 0; f < 8; f++) _enPassant[f] = cPrng.Next();
    }
}

public class CPrng
{
    private ulong _u;

    public CPrng(ulong seed) => _u = seed;

    public ulong Next()
    {
        _u ^= _u >> 12;
        _u ^= _u << 25;
        _u ^= _u >> 27;
        return _u * 2685821657736338717L;
    }
}