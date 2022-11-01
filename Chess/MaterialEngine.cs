using Chess.Book;
using static Chess.CDisplay;

namespace Chess;

public class MaterialEngine : Engine
{

    protected override CMove? FindMove(CBookEntry? entry = null)
    {
        int bestScore = -0xfffffff;
        CMove?[] bestMoves = new CMove[128];
        ushort bestMoveIndex = 0;
        CMove?[] moves = CMoveGeneration.MoveGen(Instance.Board);
        for (var index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move == null) break;
            move.Make(Instance.Board);
            int score = -NegaMax(3, Instance.Board);
            move.Unmake(Instance.Board);
            if (score == bestScore)
            {
                bestMoves[bestMoveIndex] = move;
                bestMoveIndex++;
            }
            else if (score > bestScore)
            {
                bestMoves = new CMove[128];
                bestMoves[0] = move;
                bestMoveIndex = 1;
            }
        }

        return FromMoveArray(bestMoves);
    }

    private int Evaluate(CBoard board)
    {
        int evaluation = 0;
        byte[] popCount = new byte[12];
        for (byte i = 0; i < 12; i++) popCount[i] = board.PopCount(i);

        for (byte i = 0; i < 12; i++) // Raw Material Weight
            if (i % 2 == 0) evaluation += PieceValues[i] * popCount[i];
            else evaluation -= PieceValues[i] * popCount[i];

        return board.ToMove == ColorType.White ? evaluation : -evaluation;
    }

    private int NegaMax(int depth, CBoard board)
    {
        if (depth == 0) return Evaluate(board);
        int max = int.MinValue;
        CMove?[] moves = CMoveGeneration.MoveGen(board);
        if (moves.Length == 0) return int.MinValue;
        for (var index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move == null) break;
            move.Make(board);
            int score = -NegaMax(depth - 1, board);
            move.Unmake(board);
            if (score > max) max = score;
        }

        return max;
    }
}