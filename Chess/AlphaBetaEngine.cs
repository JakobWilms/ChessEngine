using System;

namespace Chess;

public abstract class AlphaBetaEngine : Engine
{
    protected AlphaBetaEngine(ColorType color) : base(color)
    {
    }

    protected abstract int Evaluate(CBoard board);

    protected override CMove? FindMove()
    {
        int alpha = int.MinValue, beta = int.MaxValue, bestScore = int.MinValue;
        CMove? bestMove = null;
        foreach (var move in CMoveGeneration.MoveGen(CDisplay.Instance.Board))
        {
            move.Make(CDisplay.Instance.Board);
            int score = AlphaBetaMax(int.MinValue, int.MaxValue, 5, CDisplay.Instance.Board);
            move.Unmake(CDisplay.Instance.Board);
            if (score < beta) beta = score;
            if (score <= alpha) score = alpha;
            if (bestScore < score)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int AlphaBetaMax(int alpha, int beta, int depthLeft, CBoard board)
    {
        if (depthLeft == 0) return Evaluate(board);
        foreach (var move in CMoveGeneration.MoveGen(board))
        {
            move.Make(board);
            int score = AlphaBetaMin(alpha, beta, depthLeft - 1, board);
            move.Unmake(board);
            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }

        return alpha;
    }
    
    private int AlphaBetaMin(int alpha, int beta, int depthLeft, CBoard board)
    {
        if (depthLeft == 0) return Evaluate(board);
        foreach (var move in CMoveGeneration.MoveGen(board))
        {
            move.Make(board);
            int score = AlphaBetaMax(alpha, beta, depthLeft - 1, board);
            move.Unmake(board);
            if (score <= alpha) return beta;
            if (score > beta) beta = score;
        }

        return alpha;
    }
}