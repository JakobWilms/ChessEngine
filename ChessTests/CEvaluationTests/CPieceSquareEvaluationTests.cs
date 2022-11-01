using Chess;
using NUnit.Framework;

namespace ChessTests.CEvaluationTests;

public class CPieceSquareEvaluationTests
{
    private CBoard Board;
    private MainEngine Engine = new MainEngine();

    [SetUp]
    public void Setup() => Board = FenReader.ImportFen(FenReader.StartingFen);

    [Test]
    public void StartPosition_Is0()
    {
        Assert.AreEqual(0, Engine.EvaluatePieceSquares(Board));
    }
    
    [Test]
    public void E2E4_Is40()
    {
        new CMove(CSquare.E2, CSquare.E4, Board).Make(Board);
        Assert.AreEqual(40, Engine.EvaluatePieceSquares(Board));
    }
    
    [Test]
    public void E2E4_B8C6_IsNeg10()
    {
        new CMove(CSquare.E2, CSquare.E4, Board).Make(Board);
        new CMove(CSquare.B8, CSquare.C6, Board).Make(Board);
        Assert.AreEqual(-10, Engine.EvaluatePieceSquares(Board));
    }
}