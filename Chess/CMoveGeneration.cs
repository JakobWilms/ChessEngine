using static Chess.CDoubleDir;
using static Chess.CSquare;
using static Chess.CSquares;
using static Chess.PieceType;

namespace Chess;

public static class CMoveGeneration
{

    public static ushort MoveGenCount(CBoard board) => MoveGenCount(MoveGen(board));

    public static ushort MoveGenCount(CMove?[] moves)
    {
        ushort count = 0;
        for (var index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move == null) break;
            count++;
        }

        return count;
    }
    
    public static CMove?[] MoveGen(CBoard board)
    {
        CMove[] moves = new CMove[128];
        CMove[] movesToAdd = new CMove[4];
        ushort index = 0;
        //castling
        if (board.ToMove == ColorType.White)
        {
            //Console.WriteLine(
            //$"{board.GetPiece(F1)} {board.GetPiece(G1)} {board.Attacked(E1, board.NotToMove())} {board.Attacked(F1, board.NotToMove())} {board.Attacked(G1, board.NotToMove())}");
            //Console.WriteLine($"{board.GetWhiteCastleKing()}");
            if (board.GetWhiteCastleKing())
                if (!board.GetPiece(F1) && !board.GetPiece(G1) &&
                    !board.Attacked(E1, board.NotToMove()) &&
                    !board.Attacked(F1, board.NotToMove()) &&
                    !board.Attacked(G1, board.NotToMove()))
                {
                    //Console.WriteLine("King Castle Push");
                    index = MoveGenPush(E1, G1, MoveFlag.KingCastle, WhiteKing, Empty, moves, board, index, movesToAdd);
                }

            if (board.GetWhiteCastleQueen())
                if (!board.GetPiece(B1) && !board.GetPiece(C1) && !board.GetPiece(D1) &&
                    !board.Attacked(C1, board.NotToMove()) &&
                    !board.Attacked(D1, board.NotToMove()) &&
                    !board.Attacked(E1, board.NotToMove()))
                    index = MoveGenPush(E1, C1, MoveFlag.QueenCastle, WhiteKing, Empty, moves,
                        board, index, movesToAdd);
        }
        else
        {
            if (board.GetBlackCastleKing())
                if (!board.GetPiece(F8) && !board.GetPiece(G8) &&
                    !board.Attacked(E8, board.NotToMove()) &&
                    !board.Attacked(F8, board.NotToMove()) &&
                    !board.Attacked(G8, board.NotToMove()))
                    index = MoveGenPush(E8, G8, MoveFlag.KingCastle, BlackKing, Empty, moves, board, index, movesToAdd);

            if (board.GetBlackCastleQueen())
                if (!board.GetPiece(B8) && !board.GetPiece(C8) && !board.GetPiece(D8) &&
                    !board.Attacked(C8, board.NotToMove()) &&
                    !board.Attacked(D8, board.NotToMove()) &&
                    !board.Attacked(E8, board.NotToMove()))
                    index = MoveGenPush(E8, C8, MoveFlag.QueenCastle, BlackKing, Empty, moves,
                        board, index, movesToAdd);
        }

        for (byte sq = 0; sq < 64; sq++)
        {
            if (board.GetColor(sq) != board.ToMove) continue;
            //Console.WriteLine($"{board.GetPieceType(sq)} {sq}");
            if (board.GetPieceType(sq) is WhitePawn or BlackPawn) // Pawn Move
            {
                index = MoveGenPawnMove((CSquare)sq, moves, board, index, movesToAdd);
                index = MoveGenPawnCapture((CSquare)sq, moves, board, index, movesToAdd);
            }
            else
            {
                byte bPieceType = (byte)board.GetEnumPiece(sq);
                for (byte dir = 0; dir < Dirs[bPieceType - 3].Length; dir++)
                {
                    for (byte pos = sq;;)
                    {
                        if (!IsSquare((CSquare)pos, Dirs[bPieceType - 3][dir])) break;
                        pos = (byte)(pos + Vectors[bPieceType - 3][dir]);

                        if (board.GetPieceType(pos) == Empty)
                            index = MoveGenPush((CSquare)sq, (CSquare)pos, MoveFlag.QuietMove,
                                board.GetPieceType(sq), Empty, moves, board, index, movesToAdd);
                        else if (board.GetColor(pos) == board.NotToMove())
                        {
                            index = MoveGenPush((CSquare)sq, (CSquare)pos, MoveFlag.Capture,
                                board.GetPieceType(sq), board.GetPieceType(pos), moves, board, index, movesToAdd);
                            break;
                        }
                        else break;

                        if (!Slide(board.GetEnumPiece(sq))) break;
                    }
                }
            }
        }

        return moves;
    }

    private static ushort MoveGenPawnMove(CSquare from, CMove[] moves, CBoard board, ushort index, CMove?[] movesToAdd)
    {
        if (board.ToMove == ColorType.White)
        {
            if (board.GetPieceType(AddDir(from, CDir.North)) == Empty)
            {
                index = MoveGenPush(from, AddDir(from, CDir.North), MoveFlag.QuietMove, WhitePawn, Empty, moves,
                    board, index, movesToAdd);
                if (RowOf(from) == 1 && board.GetPieceType(from + 16) == Empty)
                    index = MoveGenPush(from, (CSquare)((byte)from + 16), MoveFlag.DoublePawnPush, WhitePawn,
                        Empty, moves, board, index, movesToAdd);
            }
        }
        else if (board.GetPieceType(AddDir(from, CDir.South)) == Empty)
        {
            index = MoveGenPush(from, AddDir(from, CDir.South), MoveFlag.QuietMove, BlackPawn, Empty, moves, board,
                index, movesToAdd);
            if (RowOf(from) == 6 && board.GetPieceType(from - 16) == Empty)
                index = MoveGenPush(from, (CSquare)((byte)from - 16), MoveFlag.DoublePawnPush, BlackPawn, Empty,
                    moves, board, index, movesToAdd);
        }

        return index;
    }

    private static ushort MoveGenPawnCapture(CSquare from, CMove[] moves, CBoard board, ushort index, CMove?[] movesToAdd)
    {
        if (board.ToMove == ColorType.White)
        {
            CSquare nw = AddDir(from, CDir.NoWe);
            CSquare ne = AddDir(from, CDir.NoEa);
            if (IsSquare(from, CDir.NoWe) &&
                (board.GetEp() == nw && board.GetEp() != A1 || board.GetColor(nw) == board.NotToMove()))
                index = MoveGenPush(from, nw, MoveFlag.Capture, WhitePawn,
                    board.GetPieceType(nw), moves, board, index, movesToAdd);
            if (IsSquare(from, CDir.NoEa) &&
                (board.GetEp() == ne && board.GetEp() != A1 || board.GetColor(ne) == board.NotToMove()))
                index = MoveGenPush(from, ne, MoveFlag.Capture, WhitePawn,
                    board.GetPieceType(ne), moves, board, index, movesToAdd);
        }
        else
        {
            CSquare sw = AddDir(from, CDir.SoWe);
            CSquare se = AddDir(from, CDir.SoEa);
            if (IsSquare(from, CDir.NoWe) &&
                (board.GetEp() == sw && board.GetEp() != A1 || board.GetColor(sw) == board.NotToMove()))
                index = MoveGenPush(from, sw, MoveFlag.Capture, BlackPawn,
                    board.GetPieceType(sw), moves, board, index, movesToAdd);
            if (IsSquare(from, CDir.NoEa) &&
                (board.GetEp() == se && board.GetEp() != A1 || board.GetColor(se) == board.NotToMove()))
                index = MoveGenPush(from, se, MoveFlag.Capture, BlackPawn,
                    board.GetPieceType(se), moves, board, index, movesToAdd);
        }

        return index;
    }

    private static ushort MoveGenPush(CSquare from, CSquare to, MoveFlag flags, PieceType fromPiece, PieceType toPiece,
        CMove[] moves, CBoard board, ushort index, CMove?[] movesToAdd)
    {
        if (fromPiece == Empty) throw new ArgumentException();
        if (index == 128) throw new IndexOutOfRangeException();
        //Console.WriteLine($"Push: {fromPiece.ToString()}");
        ushort myIndex = 0;
        if (fromPiece is WhitePawn or BlackPawn && to == board.GetEp() && board.GetEp() != A1) // En passant Capture
        {
            flags = MoveFlag.EpCapture;
            toPiece = fromPiece == WhitePawn ? BlackPawn : WhitePawn;
        }

        CMove move = new CMove(from, to, flags, fromPiece, toPiece);

        if (fromPiece is BlackPawn or WhitePawn && RowOf(to) is 0 or 7) // Pawn Promotion
        {
            int add = toPiece == Empty ? 8 : 12;
            for (int i = 0; i < 4; i++) movesToAdd[i] = new CMove(from, to, (MoveFlag)(add + i), fromPiece, toPiece);
            myIndex += 3;
        }
        else movesToAdd[0] = move;

        if (flags != MoveFlag.KingCastle && flags != MoveFlag.QueenCastle)
        {
            move.Make(board); // Verify Legal Move
            if (CBoard.GetColorPiece(fromPiece) == EnumPiece.NWhite)
            {
                if (board.InCheck(ColorType.White))
                {
                    move.Unmake(board);
                    return index;
                }
            }
            else if (board.InCheck(ColorType.Black))
            {
                move.Unmake(board);
                return index;
            }

            move.Unmake(board);
        }

        for (int i = 0; i <= myIndex; i++)
        {
            if (movesToAdd[i] == null) break;
            moves[index + i] = movesToAdd[i]!;
        }

        index = (ushort)(index + myIndex + 1);

        if (move.GetToPieceType() != Empty)
            move.Score = (ushort)(move.GetToPiece() + (12 - move.GetFromPiece()));
        for (int i = 0; i <= myIndex; i++) movesToAdd[i] = null;

        return index;
    }

    public static CMove?[] Sort(CMove?[] moves, CMove? bestMove)
    {
        int best = -1;
        Array.Sort(moves, (a, b) => (a, b) switch
        {
            (null, null) => 0,
            (null, not null) => /*b.Score*/1,
            (not null, null) => /*-a.Score*/-1,
            (not null, not null) => /*b.Score - a.Score*/a.Score > b.Score ? -1 : 1
        });
        if (bestMove == null) return moves;

        for (int i = 0; i < moves.Length; i++)
            if (moves[i] == null) break;
            else if (moves[i]!.GetFrom() == bestMove.GetFrom() && moves[i]!.GetTo() == bestMove.GetTo())
                best = i;
        if (best == -1) return moves;
        CMove?[] returnValue = new CMove[moves.Length + 1];
        returnValue[0] = bestMove;
        for (int i = 0, pos = 1; i < moves.Length; i++, pos++)
        {
            if (moves[i] == null) continue;
            if (!moves[i]!.Equals(bestMove))
                returnValue[pos] = moves[i];
            else pos--;
        }

        return moves;
    }

    private static bool Slide(EnumPiece piece) => piece is EnumPiece.NBishop or EnumPiece.NRook or EnumPiece.NQueen;

    private static readonly CDoubleDir[][] Dirs =
    {
        new[] { Nne, Nee, See, Sse, Ssw, Sww, Nww, Nnw },
        new[] { Ne, Se, Sw, Nw },
        new[] { N, E, S, W },
        new[] { N, Ne, E, Se, S, Sw, W, Nw },
        new[] { N, Ne, E, Se, S, Sw, W, Nw }
    };

    private static readonly sbyte[][] Vectors =
    {
        new sbyte[] { 17, 10, -6, -15, -17, -10, 6, 15 },
        new sbyte[] { 9, -7, -9, 7 },
        new sbyte[] { 8, 1, -8, -1 },
        new sbyte[] { 8, 9, 1, -7, -8, -9, -1, 7 },
        new sbyte[] { 8, 9, 1, -7, -8, -9, -1, 7 }
    };
}