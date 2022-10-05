using System;
using System.Collections.Generic;
using static Chess.CDoubleDir;
using static Chess.CSquare;
using static Chess.CSquares;
using static Chess.PieceType;

namespace Chess;

public static class CMoveGeneration
{
    public static List<CMove> MoveGen(CBoard board)
    {
        List<CMove> moves = new List<CMove>();
        short moveCount = 0;
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
                    Console.WriteLine("King Castle Push");
                    moveCount += MoveGenPush(E1, G1, MoveFlag.KingCastle, WhiteKing, Empty, moves, board);
                }

            if (board.GetWhiteCastleQueen())
                if (!board.GetPiece(B1) && !board.GetPiece(C1) && !board.GetPiece(D1) &&
                    !board.Attacked(C1, board.NotToMove()) &&
                    !board.Attacked(D1, board.NotToMove()) &&
                    !board.Attacked(E1, board.NotToMove()))
                    moveCount += MoveGenPush(E1, C1, MoveFlag.QueenCastle, WhiteKing, Empty, moves,
                        board);
        }
        else
        {
            if (board.GetBlackCastleKing())
                if (!board.GetPiece(F8) && !board.GetPiece(G8) &&
                    !board.Attacked(E8, board.NotToMove()) &&
                    !board.Attacked(F8, board.NotToMove()) &&
                    !board.Attacked(G8, board.NotToMove()))
                    moveCount += MoveGenPush(E8, G8, MoveFlag.KingCastle, BlackKing, Empty, moves, board);

            if (board.GetBlackCastleQueen())
                if (!board.GetPiece(B8) && !board.GetPiece(C8) && !board.GetPiece(D8) &&
                    !board.Attacked(C8, board.NotToMove()) &&
                    !board.Attacked(D8, board.NotToMove()) &&
                    !board.Attacked(E8, board.NotToMove()))
                    moveCount += MoveGenPush(E8, C8, MoveFlag.QueenCastle, BlackKing, Empty, moves,
                        board);
        }

        for (byte sq = 0; sq < 64; sq++)
        {
            if (board.GetColor(sq) != board.ToMove) continue;
            //Console.WriteLine($"{board.GetPieceType(sq)} {sq}");
            if (board.GetPieceType(sq) is WhitePawn or BlackPawn) // Pawn Move
            {
                moveCount += MoveGenPawnMove((CSquare)sq, moves, board);
                moveCount += MoveGenPawnCapture((CSquare)sq, moves, board);
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
                            moveCount += MoveGenPush((CSquare)sq, (CSquare)pos, MoveFlag.QuietMove,
                                board.GetPieceType(sq), Empty, moves, board);
                        else if (board.GetColor(pos) == board.NotToMove())
                        {
                            moveCount += MoveGenPush((CSquare)sq, (CSquare)pos, MoveFlag.Capture,
                                board.GetPieceType(sq), board.GetPieceType(pos), moves, board);
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

    private static short MoveGenPawnMove(CSquare from, List<CMove> moves, CBoard board)
    {
        short moveCount = 0;
        if (board.ToMove == ColorType.White)
        {
            if (board.GetPieceType(AddDir(from, CDir.Nort)) == Empty)
            {
                moveCount += MoveGenPush(from, AddDir(from, CDir.Nort), MoveFlag.QuietMove, WhitePawn, Empty, moves,
                    board);
                if (RowOf(from) == 1 && board.GetPieceType(from + 16) == Empty)
                    moveCount += MoveGenPush(from, (CSquare)((byte)from + 16), MoveFlag.DoublePawnPush, WhitePawn,
                        Empty, moves, board);
            }
        }
        else if (board.GetPieceType(AddDir(from, CDir.Sout)) == Empty)
        {
            moveCount += MoveGenPush(from, AddDir(from, CDir.Sout), MoveFlag.QuietMove, BlackPawn, Empty, moves, board);
            if (RowOf(from) == 6 && board.GetPieceType(from - 16) == Empty)
                moveCount += MoveGenPush(from, (CSquare)((byte)from - 16), MoveFlag.DoublePawnPush, BlackPawn, Empty,
                    moves, board);
        }

        return moveCount;
    }

    private static short MoveGenPawnCapture(CSquare from, List<CMove> moves, CBoard board)
    {
        short moveCount = 0;
        if (board.ToMove == ColorType.White)
        {
            CSquare nw = AddDir(from, CDir.NoWe);
            CSquare ne = AddDir(from, CDir.NoEa);
            if (IsSquare(from, CDir.NoWe) &&
                (board.GetEp() == nw && board.GetEp() != A1 || board.GetColor(nw) == board.NotToMove()))
                moveCount += MoveGenPush(from, nw, MoveFlag.Capture, WhitePawn,
                    board.GetPieceType(nw), moves, board);
            if (IsSquare(from, CDir.NoEa) &&
                (board.GetEp() == ne && board.GetEp() != A1 || board.GetColor(ne) == board.NotToMove()))
                moveCount += MoveGenPush(from, ne, MoveFlag.Capture, WhitePawn,
                    board.GetPieceType(ne), moves, board);
        }
        else
        {
            CSquare sw = AddDir(from, CDir.SoWe);
            CSquare se = AddDir(from, CDir.SoEa);
            if (IsSquare(from, CDir.NoWe) &&
                (board.GetEp() == sw && board.GetEp() != A1 || board.GetColor(sw) == board.NotToMove()))
                moveCount += MoveGenPush(from, sw, MoveFlag.Capture, BlackPawn,
                    board.GetPieceType(sw), moves, board);
            if (IsSquare(from, CDir.NoEa) &&
                (board.GetEp() == se && board.GetEp() != A1 || board.GetColor(se) == board.NotToMove()))
                moveCount += MoveGenPush(from, se, MoveFlag.Capture, BlackPawn,
                    board.GetPieceType(se), moves, board);
        }

        return moveCount;
    }

    private static short MoveGenPush(CSquare from, CSquare to, MoveFlag flags, PieceType fromPiece, PieceType toPiece,
        List<CMove> moves, CBoard board)
    {
        //Console.WriteLine($"Push: {fromPiece.ToString()}");
        short moveCount = 0;
        CMove move = new CMove(from, to, flags, fromPiece, toPiece);
        if (fromPiece is WhitePawn or BlackPawn && to == board.GetEp() && board.GetEp() != A1) // En passant Capture
        {
            move.SetToPiece(fromPiece == WhitePawn ? BlackPawn : WhitePawn);
            move.SetFlags(MoveFlag.EpCapture);
        }

        if (flags != MoveFlag.KingCastle && flags != MoveFlag.QueenCastle)
        {
            move.Make(board); // Verify Legal Move
            if (CBoard.GetColorPiece(fromPiece) == EnumPiece.NWhite)
            {
                if (board.InCheck(ColorType.White))
                {
                    move.Unmake(board);
                    return 0;
                }
            }
            else if (board.InCheck(ColorType.Black))
            {
                move.Unmake(board);
                return 0;
            }

            move.Unmake(board);
        }

        if (fromPiece is BlackPawn or WhitePawn && RowOf(to) is 0 or 7) // Pawn Promotion
        {
            int add = toPiece == Empty ? 8 : 12;
            for (int i = 0; i < 4; i++) moves.Add(new CMove(from, to, (MoveFlag)(add + i), fromPiece, toPiece));
            moveCount += 3;
        }
        else moves.Add(move);

        moveCount++;
        return moveCount;
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