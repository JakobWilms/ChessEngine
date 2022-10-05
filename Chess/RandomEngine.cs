using System;
using System.Collections.Generic;

namespace Chess;

public class RandomEngine : Engine
{

    private readonly Random _random;

    public RandomEngine(ColorType color) : base(color)
    {
        _random = new Random();
    }

    protected override CMove? FindMove()
    {
        List<CMove> moves = CMoveGeneration.MoveGen(CDisplay.Instance.Board);
        return moves.Count == 0 ? null : moves[_random.Next(moves.Count)];
    }
}