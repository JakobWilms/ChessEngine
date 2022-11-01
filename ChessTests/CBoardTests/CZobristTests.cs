using Chess;
using NUnit.Framework;

namespace ChessTests.CBoardTests;

public class CZobristTests
{
    [Test]
    public void NewBoard_SameHash()
    {
        CBoard board1 = CBoard.StartingBoard();
        CBoard board2 = CBoard.StartingBoard();
        
        Assert.AreEqual(board1.GetHash(), board2.GetHash());
    }
    
    [Test]
    public void OneMove_SameHash()
    {
        CBoard board1 = CBoard.StartingBoard();
        CBoard board2 = CBoard.StartingBoard();
        
        new CMove(CSquare.E2, CSquare.E4, board1).Make(board1);
        new CMove(CSquare.E2, CSquare.E4, board2).Make(board2);
        
        Assert.AreEqual(board1.GetHash(), board2.GetHash());
    }
}