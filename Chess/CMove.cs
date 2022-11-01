using System.Text;
using static Chess.CSquare;
using static Chess.EnumPiece;
using static Chess.MoveFlag;
using static Chess.PieceType;

namespace Chess;

public class CMove
{
    private readonly CastlingType[] _castlingTypes;
    private readonly CastlingType[] _previousTypes;

    private readonly ushort _move; // Flags 4, From 6, To 6
    private readonly byte _pieces; // FromPiece 4, ToPiece 4
    public ushort Score;
    public ushort PrevInfo { get; private set; }

    private CMove(byte from, byte to, byte flags, byte fromPiece, byte toPiece)
    {
        _move = (ushort)(((flags & 0xf) << 12) | ((from & 0x3f) << 6) | (to & 0x3f));
        _pieces = (byte)(((fromPiece & 0xf) << 4) | (toPiece & 0xf));
        _castlingTypes = new CastlingType[4];
        _previousTypes = new CastlingType[4];
    }

    public CMove(CSquare from, CSquare to, MoveFlag flag, PieceType fromPiece, PieceType toPiece) : this((byte)from,
        (byte)to, (byte)flag, (byte)fromPiece, (byte)toPiece)
    {
    }

    public CMove(CSquare from, CSquare to, CBoard board, MoveFlag flag = QueenPromotion)
    {
        CMove?[] moves = CMoveGeneration.MoveGen(board);
        for (var index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move == null) continue;
            if (move.GetFromSquare() != from || move.GetToSquare() != to) continue;
            _move = move._move;
            if (move.IsPromotion())
            {
                _move &= 0xfff;
                _move |= (ushort)(((byte)flag & 0xf) << 12);
            }

            _pieces = move._pieces;
            break;
        }

        if (_pieces == 0) throw new ArgumentException();
        _castlingTypes = new CastlingType[4];
        _previousTypes = new CastlingType[4];
    }

    public CMove(byte from, byte to, CBoard board) : this((CSquare)from, (CSquare)to, board)
    {
    }

    public void Print() => Console.WriteLine(ToString());

    public override string ToString() =>
        $"From:{(CSquare)GetFrom()}, To:{(CSquare)GetTo()}, Flag:{GetFlags()}, FromPiece:{GetFromPieceType()}, ToPiece:{GetToPieceType()}";

    public string ToShortString() =>
        $"{(CSquare)GetFrom()}{(CSquare)GetTo()}";

    public string ToDisplayMove()
    {
        return GetFlags() switch
        {
            QuietMove => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                         (CSquare)GetTo(),
            DoublePawnPush => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                              (CSquare)GetTo(),
            KingCastle => "0-0",
            QueenCastle => "0-0-0",
            Capture => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                       (CSquare)GetTo(),
            EpCapture => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                         (CSquare)GetTo(),
            KnightPromotion => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                               (CSquare)GetTo() + "=" + UnicodeFromPieceType(
                                   CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? WhiteKnight : BlackKnight),
            BishopPromotion => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                               (CSquare)GetTo() + "=" + UnicodeFromPieceType(
                                   CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? WhiteBishop : BlackBishop),
            RookPromotion => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                             (CSquare)GetTo() + "=" +
                             UnicodeFromPieceType(CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                                 ? WhiteRook
                                 : BlackRook),
            QueenPromotion => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "-" +
                              (CSquare)GetTo() + "=" + UnicodeFromPieceType(
                                  CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? WhiteQueen : BlackQueen),
            KnightPromotionCapture => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                                      (CSquare)GetTo() + "=" + UnicodeFromPieceType(
                                          CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                                              ? WhiteKnight
                                              : BlackKnight),
            BishopPromotionCapture => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                                      (CSquare)GetTo() + "=" + UnicodeFromPieceType(
                                          CBoard.GetColorPiece(GetFromPieceType()) == NWhite
                                              ? WhiteBishop
                                              : BlackBishop),
            RookPromotionCapture => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                                    (CSquare)GetTo() + "=" + UnicodeFromPieceType(
                                        CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? WhiteRook : BlackRook),
            QueenPromotionCapture => UnicodeFromPieceType(GetFromPieceType()) + ((CSquare)GetFrom()).ToString() + "x" +
                                     (CSquare)GetTo() + "=" + UnicodeFromPieceType(
                                         CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? WhiteQueen : BlackQueen),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ToSan(CBoard board) => ToSan(CMoveGeneration.MoveGen(board));

    public string ToSan(CMove?[] possibleMoves)
    {
        if (GetFlags() == KingCastle) return "O-O";
        if (GetFlags() == QueenCastle) return "O-O-O";
        string pieceSymbol = StringFromPieceType(GetFromPieceType());
        string capture = IsCapture() ? "x" : String.Empty;
        string to = GetToSquare().ToString();
        string promotion = StringFromPromotion(GetFlags());
        string from;

        File file = CSquares.FileOf(GetFromSquare());
        Rank rank = CSquares.RankOf(GetFromSquare());

        if (GetFromPieceType() != BlackPawn && GetFromPieceType() != WhitePawn)
        {
            CMove?[] moves = new CMove[8];
            ushort moveIndex = 0;
            for (var index = 0; index < possibleMoves.Length; index++)
            {
                var move = possibleMoves[index];
                if (move == null) break;
                if (move.GetFromPieceType() != GetFromPieceType() || move.GetTo() != GetTo() ||
                    move.GetFrom() == GetFrom()) continue;
                moves[moveIndex] = move;
                moveIndex++;
            }

            if (moveIndex > 0)
            {
                bool sameFile = false;
                bool sameRank = false;
                for (var index = 0; index < moveIndex; index++)
                {
                    var move = moves[index];
                    if (move == null) break;
                    if (CSquares.FileOf(move.GetFromSquare()) == file)
                        sameFile = true;
                    else if (CSquares.RankOf(move.GetFromSquare()) == rank)
                        sameRank = true;
                }


                if (!sameFile) from = file.ToString();
                else if (!sameRank) from = rank.ToString().Replace("_", "");
                else from = GetFromSquare().ToString();
            }
            else from = String.Empty;
        }
        else if (IsCapture()) from = file.ToString();
        else from = String.Empty;

        return new StringBuilder().Append(from.ToLower()).Append(capture).Append(to.ToLower()).Append(promotion)
            .Insert(0, pieceSymbol)
            .ToString();
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

    public void Make(CBoard board)
    {
        PrevInfo = board.Info;
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
        if (GetFromPieceType() == Empty) throw new ArgumentException();
        board.Zobrist.Make(this);
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

        board.Info = PrevInfo;
        if (board.ToMove == ColorType.White) board.FullMoves--;
        board.SwapToMove();
        board.Zobrist.Unmake(this);
    }

    private void UpdateCastlingRights(CBoard board) => board.UnsetCastlingTypes(UnsetCastlingTypes());

    public CastlingType[] UnsetCastlingTypes()
    {
        for (int i = 0; i < 4; i++)
        {
            _castlingTypes[i] = CastlingType.Null;
            _previousTypes[i] = CastlingType.Null;
        }

        if (GetFromPieceType() == WhiteKing)
        {
            _castlingTypes[0] = CastlingType.WhiteCastleKing;
            _castlingTypes[1] = CastlingType.WhiteCastleQueen;
        }
        else if (GetFromPieceType() == BlackKing)
        {
            _castlingTypes[2] = CastlingType.BlackCastleKing;
            _castlingTypes[3] = CastlingType.BlackCastleQueen;
        }

        switch (GetToSquare(), GetFromSquare())
        {
            case (A1, _) or (_, A1):
                _castlingTypes[1] = CastlingType.WhiteCastleQueen;
                break;
            case (H1, _) or (_, H1):
                _castlingTypes[0] = CastlingType.WhiteCastleKing;
                break;
            case (A8, _) or (_, A8):
                _castlingTypes[3] = CastlingType.BlackCastleQueen;
                break;
            case (H8, _) or (_, H8):
                _castlingTypes[2] = CastlingType.BlackCastleKing;
                break;
        }

        CUtils.GetCastlingTypes(PrevInfo, _previousTypes);
        for (int i = 0; i < 4; i++) _castlingTypes[i] = _castlingTypes[i] == _previousTypes[i] ? _castlingTypes[i] : CastlingType.Null;
        return _castlingTypes;
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

    private void MakeQuiet(CBoard board, CSquare from, CSquare to, PieceType fromPieceType) =>
        MakeQuiet(board, (byte)from, (byte)to, fromPieceType);

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

    private static string StringFromPieceType(PieceType pieceType) =>
        pieceType switch
        {
            WhitePawn or BlackPawn => String.Empty,
            WhiteKnight or BlackKnight => "N",
            WhiteBishop or BlackBishop => "B",
            WhiteRook or BlackRook => "R",
            WhiteQueen or BlackQueen => "Q",
            WhiteKing or BlackKing => "K",
            _ => throw new ArgumentOutOfRangeException(nameof(pieceType), pieceType, null)
        };

    private static string StringFromPromotion(MoveFlag flag) =>
        flag switch
        {
            KnightPromotion or KnightPromotionCapture => "=N",
            BishopPromotion or BishopPromotionCapture => "=B",
            RookPromotion or RookPromotionCapture => "=R",
            QueenPromotion or QueenPromotionCapture => "=Q",
            _ => String.Empty
        };

    public bool IsPromotion() => IsPromotion(GetFlags());

    public bool IsCapture() => IsCapture(GetFlags());

    public bool IsCastle() => IsCastle(GetFlags());

    private static bool IsPromotion(MoveFlag flag) => flag is KnightPromotion or BishopPromotion or RookPromotion
        or QueenPromotion
        or KnightPromotionCapture or BishopPromotionCapture or RookPromotionCapture or QueenPromotionCapture;

    private static bool IsCapture(MoveFlag flag) => flag is Capture or EpCapture or KnightPromotionCapture
        or BishopPromotionCapture or RookPromotionCapture or QueenPromotionCapture;

    private static bool IsCastle(MoveFlag flag) => flag is KingCastle or QueenCastle;

    public PieceType GetPromotionPieceType()
    {
        if (!IsPromotion()) throw new ArgumentException();
        int whitePiece;
        if (IsCapture()) whitePiece = 2 + ((byte)GetFlags() - 12) * 2;
        else whitePiece = 2 + ((byte)GetFlags() - 8) * 2;
        return (PieceType)(CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? whitePiece : whitePiece + 1);
    }

    public CSquare NewEp() =>
        (CSquare)(GetFlags() == DoublePawnPush
            ? CBoard.GetColorPiece(GetFromPieceType()) == NWhite ? GetFrom() + 8 : GetFrom() - 8
            : 0);

    public override bool Equals(object? obj) => obj is CMove move && this == move;

    public override int GetHashCode() => HashCode.Combine(_move, _pieces);
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