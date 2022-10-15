using System;
using System.Collections.Generic;
using static Chess.CDisplay;

namespace Chess;

public abstract class AlphaBetaEngine : Engine
{
    
    private readonly Random _random;
    
    protected AlphaBetaEngine(ColorType color) : base(color)
    {
        _random = new Random();
    }

    protected abstract int Evaluate(CBoard board);

    protected override CMove? FindMove()
    {
        int alpha = int.MinValue /*, beta = int.MaxValue*/, bestScore = int.MinValue;
        List<CMove> bestMoves = new List<CMove>();
        List<CMove> moves = CMoveGeneration.MoveGen(Instance.Board);
        foreach (var move in moves)
        {
            move.Make(Instance.Board);
            int score = -AlphaBeta(int.MinValue, int.MaxValue, 3, Instance.Board);
            move.Unmake(Instance.Board);
            if (score > alpha) alpha = score;
            if (score == bestScore)
            {
                bestMoves.Add(move);
            }
            else if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(move);
            }
        }
        return bestMoves.Count == 0 ? null : bestMoves.Count == 1 ? bestMoves[0] : bestMoves[_random.Next(bestMoves.Count)];
    }
    
    private int AlphaBeta(int alpha, int beta, int depthLeft, CBoard board)
    {
        if (depthLeft == 0) return Evaluate(board);
        List<CMove> moves = CMoveGeneration.MoveGen(board);
        foreach (var move in moves)
        {
            move.Make(board);
            int score = -AlphaBeta(-beta, -alpha, depthLeft - 1, board);
            move.Unmake(board);
            if (score >= beta) return beta;
            if (score > alpha) alpha = score;
        }
        return alpha;
    }
}