using Chess;
using NUnit.Framework;

namespace ChessTests.CBoardTests;

public class CBoardTestsInfo
{

    private CBoard board;
    
    [SetUp]
    public void SetUp()
    {
        board = new CBoard();
    }
    
    [Test]
    public void SetHalfMoves_GetHalfMoves()
    {
        board.SetHalfMoves(5);
        
        Assert.AreEqual(5, board.GetHalfMoves());
    }

    [Test]
    public void SetEP_GetEP()
    {
        board.SetEnPassantTargetSquare(CSquare.A3);
        
        Assert.AreEqual(CSquare.A3, board.GetEp());
        
        board.SetEp((int)CSquare.B6);
        
        Assert.AreEqual(CSquare.B6, board.GetEp());
    }
    
    [Test]
    public void SetCastle_GetCastle()
    {
        board.SetWhiteCastleQueen(true);
        
        Assert.True(board.GetWhiteCastleQueen());
    }
}