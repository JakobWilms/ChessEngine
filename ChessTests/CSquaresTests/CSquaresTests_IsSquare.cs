using Chess;
using NUnit.Framework;

namespace ChessTests.CSquaresTests;

public class CSquaresTestsIsSquare
{
    [Test]
    public void IsSquare_A1_NorthWest()
    {
        Assert.False(CSquares.IsSquare(CSquare.A1, CDoubleDir.Nw));
    }
}