using Chess.Book;
using static Chess.CDisplay;

namespace Chess;

public class CaptureEngine : Engine
{
    protected override CMove? FindMove(CBookEntry? entry = null)
    {
        int bestScore = -0xfffffff;
        CMove?[] bestMoves = new CMove[128];
        ushort bestMoveIndex = 0;
        CMove?[] moves = CMoveGeneration.MoveGen(Instance.Board);
        for (ushort index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move == null) break;
            move.Make(Instance.Board);
            int score = -Evaluate(Instance.Board);
            move.Unmake(Instance.Board);
            if (score == bestScore)
            {
                bestMoves[bestMoveIndex] = move;
                bestMoveIndex++;
            }
            else if (score > bestScore)
            {
                bestScore = score;
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

        for (byte i = 0; i < 12; i++)
            if (i % 2 == 0) evaluation += popCount[i];
            else evaluation -= popCount[i];

        return board.ToMove == ColorType.White ? evaluation : -evaluation;
    }
}