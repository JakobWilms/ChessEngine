using Chess;
using NUnit.Framework;

namespace ChessTests.CBoardTests;

public class CBoardTestsAttacked
{

    private CBoard Board;
    
    [SetUp]
    public void SetUp() => Board = FenReader.ImportFen(FenReader.StartingFen);
    
    [Test]
    public void StartPosition_KingNotAttacked()
    {
        Assert.False(Board.Attacked(CSquare.E1, ColorType.Black));
        Assert.False(Board.Attacked(CSquare.E8, ColorType.White));
    }
    
    [Test]
    public void FirstMove_KingNotAttacked()
    {
        new CMove(CSquare.E2, CSquare.E4, MoveFlag.DoublePawnPush, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        Assert.False(Board.Attacked(CSquare.E1, ColorType.Black));
        Assert.False(Board.Attacked(CSquare.E8, ColorType.White));
    }
    
    [Test]
    public void StartPosition_NoCheck()
    {
        Assert.False(Board.InCheck(ColorType.White));
        Assert.False(Board.InCheck(ColorType.Black));
    }
    
    [Test]
    public void FirstMove_NoCheck()
    {
        new CMove(CSquare.E2, CSquare.E4, MoveFlag.DoublePawnPush, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        Assert.False(Board.InCheck(ColorType.White));
        Assert.False(Board.InCheck(ColorType.Black));
    }
    
    [Test]
    public void SecondMove_NoCheck()
    {
        new CMove(CSquare.E2, CSquare.E4, MoveFlag.DoublePawnPush, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        new CMove(CSquare.D7, CSquare.D6, MoveFlag.QuietMove, PieceType.BlackPawn, PieceType.Empty).Make(Board);
        Assert.False(Board.InCheck(ColorType.White));
        Assert.False(Board.InCheck(ColorType.Black));
    }
}