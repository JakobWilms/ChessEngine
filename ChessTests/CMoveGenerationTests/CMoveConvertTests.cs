using System;
using Chess;
using NUnit.Framework;
using static Chess.CSquare;

namespace ChessTests.CMoveGenerationTests;

public class CMoveConvertTests
{
    private CBoard Board;

    [SetUp]
    public void SetUp() => Board = CBoard.StartingBoard();

    [Test]
    public void E2E4_ToSan()
    {
        CMove move = new CMove(E2, E4, Board);

        Assert.AreEqual("e4", move.ToSan(Board));
    }

    [Test]
    public void E2E4_E7E5_ToSan()
    {
        new CMove(E2, E4, Board).Make(Board);
        CMove move = new CMove(E7, E5, Board);

        Assert.AreEqual("e5", move.ToSan(Board));
    }

    [Test]
    public void PawnCapture_ToSan()
    {
        new CMove(E2, E4, Board).Make(Board);
        new CMove(A7, A6, Board).Make(Board);
        new CMove(D2, D4, Board).Make(Board);
        new CMove(E7, E6, Board).Make(Board);
        new CMove(D4, D5, Board).Make(Board);
        CMove move = new CMove(E6, D5, Board);
        Assert.AreEqual("exd5", move.ToSan(Board));
        move.Make(Board);
        move = new CMove(E4, D5, Board);
        Assert.AreEqual("exd5", move.ToSan(Board));
    }
    
    [Test]
    public void KnightMove_ToSan()
    {
        CMove move = new CMove(B1, C3, Board);
        Assert.AreEqual("Nc3", move.ToSan(Board));
    }

    [Test]
    public void KnightCapture()
    {
        Console.WriteLine(Board.PieceBb[3]);
        new CMove(E2, E4, Board).Make(Board);
        new CMove(E7, E5, Board).Make(Board);
        new CMove(G1, F3, Board).Make(Board);
        new CMove(B8, C6, Board).Make(Board);
        new CMove(B1, C3, Board).Make(Board);
        new CMove(F8, C5, Board).Make(Board);
        new CMove(F1, B5, Board).Make(Board);
        new CMove(D7, D6, Board).Make(Board);
        new CMove(D2, D4, Board).Make(Board);
        new CMove(E5, D4, Board).Make(Board);
        new CMove(F3, D4, Board).Make(Board);
        new CMove(C8, D7, Board).Make(Board);
        CMove move = new CMove(D4, C6, Board);
        move.Make(Board);
        Console.WriteLine(move.ToString());
        Console.WriteLine(Board.GetPieceType(C6));
        //foreach (var cMove in CMoveGeneration.MoveGen(Board)) cMove.Print();
        Console.WriteLine(Board.PieceBb[3]);
        move = new CMove(B7, C6, Board);
        Console.WriteLine(move.ToString());
    }
}