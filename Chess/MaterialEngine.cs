using System;
using System.Collections.Generic;
using static Chess.CDisplay;

namespace Chess;

public class MaterialEngine : Engine
{
    
    private readonly Random _random;
    
    public MaterialEngine(ColorType color) : base(color)
    {
        _random = new Random();
    }

    protected override CMove? FindMove()
    {
        int bestScore = -0xfffffff;
        List<CMove> bestMoves = new List<CMove>();
        List<CMove> moves = CMoveGeneration.MoveGen(Instance.Board);
        foreach (var move in moves)
        {
            move.Make(Instance.Board);
            int score = -NegaMax(3, Instance.Board);
            move.Unmake(Instance.Board);
            if (score == bestScore)
                bestMoves.Add(move);
            else if (score > bestScore)
            {
                bestScore = score;
                bestMoves.Clear();
                bestMoves.Add(move);
            }
        }

        return bestMoves.Count == 0 ? null : bestMoves.Count == 1 ? bestMoves[0] : bestMoves[_random.Next(bestMoves.Count)];
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
        List<CMove> moves = CMoveGeneration.MoveGen(board);
        if (moves.Count == 0) return int.MinValue;
        foreach (var move in moves)
        {
            move.Make(board);
            int score = -NegaMax(depth - 1, board);
            move.Unmake(board);
            if (score > max) max = score;
        }

        return max;
    }
}