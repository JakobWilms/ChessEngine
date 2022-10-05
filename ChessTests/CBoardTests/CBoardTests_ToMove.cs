using Chess;
using NUnit.Framework;

namespace ChessTests.CBoardTests;

public class CBoardTestsToMove
{
    
    [Test]
    public void NotToMove_SwapToMove_WhiteToMove()
    {
        CBoard board = new CBoard();
        
        board.SwapToMove();
        
        Assert.AreEqual(ColorType.White, board.NotToMove());
    }

    [Test]
    public void NotToMove_WhiteToMove()
    {
        CBoard board = new CBoard();

        board.ToMove = ColorType.White;
        
        Assert.AreEqual(ColorType.Black, board.NotToMove());
    }
    
    [Test]
    public void NewCBoard_BitboardsAre0()
    {
        CBoard board = new CBoard();

        foreach (ulong actual in board.PieceBb)
        {
            Assert.AreEqual(0, actual);
        }
    }
}