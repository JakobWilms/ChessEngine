using Chess;
using NUnit.Framework;

namespace ChessTests.CBoardTests;

public class CBoardTestsManipulation
{
    private CBoard board;
    
    [SetUp]
    public void SetUp()
    {
        board = new CBoard();
    }
    
    [Test]
    public void SetSquare_GetOccupied()
    {
        board.SetSquare(EnumPiece.NBlack, CSquare.A3, true);
        
        Assert.True(board.GetOccupied(CSquare.A3));
    }
    
    [Test]
    public void SetSquare_GetColor()
    {
        board.SetSquare(EnumPiece.NWhite, CSquare.B2, true);
        
        Assert.AreEqual(ColorType.White, board.GetColor(CSquare.B2));
        Assert.AreEqual(ColorType.None, board.GetColor(CSquare.A6));
    }
    
    [Test]
    public void SetSquare_GetPieceType()
    {
        board.SetSquare(EnumPiece.NBlack, CSquare.C6, true);
        board.SetSquare(EnumPiece.NKnight, CSquare.C6, true);
        
        Assert.AreEqual(PieceType.BlackKnight, board.GetPieceType(CSquare.C6));
        Assert.AreEqual(PieceType.Empty, board.GetPieceType(CSquare.C5));
    }
    
    [Test]
    public void SetSquare_GetEnumPiece()
    {
        board.SetSquare(EnumPiece.NBlack, CSquare.C6, true);
        board.SetSquare(EnumPiece.NKnight, CSquare.C6, true);
        
        Assert.AreEqual(EnumPiece.NKnight, board.GetEnumPiece(CSquare.C6));
    }
}