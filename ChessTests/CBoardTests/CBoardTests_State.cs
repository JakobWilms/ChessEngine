using NUnit.Framework;
using static ChessTests.CBoardTests.CBoardTestsSetUp;

namespace ChessTests.CBoardTests;

public class CBoardTestsState
{
    [Test]
    public void WhitePiecesColor_Correct()
    {
        Assert.AreEqual(0xffff, Board.GetWhite());
    }
    
    [Test]
    public void BlackPiecesColor_Correct()
    {
        Assert.AreEqual(0x_ffff_0000_0000_0000, Board.GetBlack());
    }
    
    [Test]
    public void Occupied_Correct()
    {
        Assert.AreEqual(0xffff00000000ffff, Board.GetOccupied());
    }
}