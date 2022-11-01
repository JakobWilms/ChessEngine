using System;
using Chess;
using NUnit.Framework;

namespace ChessTests.CMoveGenerationTests;

public class CMoveGenerationTestsSort
{
    [Test]
    public void SortMoveGeneration()
    {
        CMove?[] moves = CMoveGeneration.Sort(CMoveGeneration.MoveGen(CBoard.StartingBoard()), null);
        Console.WriteLine();
    }
}