using Chess;
using NUnit.Framework;
using static Chess.CSquare;
using static Chess.MoveFlag;
using static Chess.PieceType;

namespace ChessTests.CEvaluationTests;

public class CMaterialEvaluationTests
{
    private CBoard Board;
    private MaterialAlphaBetaEngine Engine = new();

    [SetUp]
    public void SetUp() => Board = FenReader.ImportFen(FenReader.StartingFen);

    [Test]
    public void StartingPos_Is0()
    {
        Assert.AreEqual(0, Engine.Evaluate(Board));
    }

    [Test]
    public void PawnCapture_Is100()
    {
        new CMove(E2, E4, DoublePawnPush, WhitePawn, Empty).Make(Board);
        new CMove(D7, D5, DoublePawnPush, BlackPawn, Empty).Make(Board);
        new CMove(E4, D5, Capture, WhitePawn, BlackPawn).Make(Board);
        
        Assert.AreEqual(100, -Engine.Evaluate(Board));
    }
    
    [Test]
    public void Black_PawnCapture_Is100()
    {
        new CMove(A2, A3, Board).Make(Board);
        new CMove(D7, D5, Board).Make(Board);
        new CMove(E2, E4, Board).Make(Board);
        new CMove(D5, E4, Board).Make(Board);
        
        Assert.AreEqual(100, -Engine.Evaluate(Board));
    }
    
    [Test]
    public void QueenCapture_Is900()
    {
        new CMove(E2, E4, Board).Make(Board);
        new CMove(D7, D5, Board).Make(Board);
        new CMove(D1, G4, Board).Make(Board);
        new CMove(D8, D7, Board).Make(Board);
        new CMove(G4, D7, Board).Make(Board);
        
        Assert.AreEqual(900, -Engine.Evaluate(Board));
    }
}