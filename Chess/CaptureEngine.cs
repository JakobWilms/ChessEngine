using System;
using System.Collections.Generic;
using static Chess.CDisplay;

namespace Chess;

public class CaptureEngine : Engine
{

    private readonly Random _random;
    
    public CaptureEngine(ColorType color) : base(color)
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
            int score = -Evaluate(Instance.Board);
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

        for (byte i = 0; i < 12; i++)
            if (i % 2 == 0) evaluation += popCount[i];
            else evaluation -= popCount[i];

        return board.ToMove == ColorType.White ? evaluation : -evaluation;
    }
}