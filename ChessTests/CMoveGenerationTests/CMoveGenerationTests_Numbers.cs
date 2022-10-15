using System.Collections.Generic;
using Chess;
using NUnit.Framework;

namespace ChessTests.CMoveGenerationTests;

public class CMoveGenerationTestsNumbers
{
    private CBoard Board;
    
    [SetUp]
    public void SetUp()
    {
        Board = FenReader.ImportFen(FenReader.StartingFen);
    }
    
    [Test]
    public void MoveGeneration_Depth1()
    {
        Assert.AreEqual(20, CMoveGeneration.MoveGen(Board).Count);
    }
    
    //[Test]
    public void MoveGeneration_Depth2_PreTest()
    {
        List<CMove> moves = CMoveGeneration.MoveGen(Board);
        moves[0].Make(Board);
        Assert.AreEqual(20, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGeneration_Depth2()
    {
        Assert.AreEqual(400, MoveGen_Root(2));
    }
    
    //[Test]
    public void MoveGeneration_Depth3_PreTest()
    {
        List<CMove> moves = CMoveGeneration.MoveGen(Board);
        TestContext.WriteLine(moves[6].ToString());
        moves[6].Make(Board);
        moves = CMoveGeneration.MoveGen(Board);
        TestContext.WriteLine(moves[0].ToString());
        moves[0].Make(Board);
        Assert.AreEqual(21, MoveGen_Root(1));
    }

    [Test]
    public void MoveGeneration_Depth3()
    {
        Assert.AreEqual(8902, MoveGen_Root(3));
    }
    
    //[Test]
    public void MoveGeneration_Depth4_PreTest()
    {
        new CMove(CSquare.B1, CSquare.C3, MoveFlag.QuietMove, PieceType.WhiteKnight, PieceType.Empty).Make(Board);
        new CMove(CSquare.E7, CSquare.E6, MoveFlag.QuietMove, PieceType.BlackPawn, PieceType.Empty).Make(Board);
        new CMove(CSquare.C3, CSquare.D5, MoveFlag.QuietMove, PieceType.WhiteKnight, PieceType.Empty).Make(Board);
        Assert.AreEqual(29, MoveGen_Root(1));
    }
    
    [Test]
    public void MoveGeneration_Depth4()
    {
        Assert.AreEqual(197281, MoveGen_Root(4));
    }
    
    //[Test]
    public void MoveGeneration_Depth5_PreTest()
    {
        new CMove(CSquare.A2, CSquare.A4, MoveFlag.DoublePawnPush, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        new CMove(CSquare.A7, CSquare.A6, MoveFlag.QuietMove, PieceType.BlackPawn, PieceType.Empty).Make(Board);
        new CMove(CSquare.A4, CSquare.A5, MoveFlag.QuietMove, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        CMove cMove = new CMove(CSquare.B7, CSquare.B5, MoveFlag.DoublePawnPush, PieceType.BlackPawn, PieceType.Empty);
        cMove.Make(Board);
        foreach (var move in CMoveGeneration.MoveGen(Board)) move.Print();
        Assert.AreEqual(22, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGeneration_Depth5()
    {
        Assert.AreEqual(4865609, MoveGen_Root(5));
    }

    [Test]
    public void MoveGeneration_Depth6()
    {
        Assert.AreEqual(119060324, MoveGen_Root(6));
    }
    
    [Test]
    public void MoveGen_Pos2_Depth1()
    {
        Board = FenReader.ImportFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        Assert.AreEqual(48, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGen_Pos2_Depth2()
    {
        Board = FenReader.ImportFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        Assert.AreEqual(2039, MoveGen_Root(2));
    }
    
    //[Test]
    public void MoveGen_Pos2_Depth3_PreTest()
    {
        Board = FenReader.ImportFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        new CMove(CSquare.E2, CSquare.A6, MoveFlag.Capture, PieceType.WhiteBishop, PieceType.BlackBishop).Make(Board);
        Assert.AreEqual(1907, MoveGen_Root(2));
    }
    
    [Test]
    public void MoveGen_Pos2_Depth3()
    {
        Board = FenReader.ImportFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        Assert.AreEqual(97862, MoveGen_Root(3));
    }
    
    [Test]
    public void MoveGen_Pos2_Depth4()
    {
        Board = FenReader.ImportFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        Assert.AreEqual(4085603, MoveGen_Root(4));
    }
    
    [Test]
    public void MoveGen_Pos2_Depth5()
    {
        Board = FenReader.ImportFen("r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R w KQkq - 0 1");
        Assert.AreEqual(193690690, MoveGen_Root(5));
    }

    [Test]
    public void MoveGen_Pos3_Depth1()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        Assert.AreEqual(14, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGen_Pos3_Depth2()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        Assert.AreEqual(191, MoveGen_Root(2));
    }
    
    [Test]
    public void MoveGen_Pos3_Depth3()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        Assert.AreEqual(2812, MoveGen_Root(3));
    }
    
    [Test]
    public void MoveGen_Pos3_Depth4()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        Assert.AreEqual(43238, MoveGen_Root(4));
    }

    [Test]
    public void MoveGen_Pos3_Depth5()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        Assert.AreEqual(674624, MoveGen_Root(5));
    }
    
    [Test]
    public void MoveGen_Pos3_Depth6()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        Assert.AreEqual(11030083, MoveGen_Root(6));
    }
    
    //[Test]
    public void MoveGen_Pos3_Depth7_PreTest()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        new CMove(CSquare.G2, CSquare.G3, MoveFlag.QuietMove, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        Assert.AreEqual(4190119, MoveGen_Root(6));
    }
    
    [Test]
    public void MoveGen_Pos3_Depth7()
    {
        Board = FenReader.ImportFen("8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8 w - - 0 1");
        Assert.AreEqual(178633661, MoveGen_Root(7));
    }
    
    [Test]
    public void MoveGen_Pos4_Depth1()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        Assert.AreEqual(6, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGen_Pos4_Depth2()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        Assert.AreEqual(264, MoveGen_Root(2));
    }
    
    //[Test]
    public void MoveGen_Pos4_Depth3_PreTest()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        new CMove(CSquare.C4, CSquare.C5, MoveFlag.QuietMove, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        Assert.AreEqual(1409, MoveGen_Root(2));
    }
    
    [Test]
    public void MoveGen_Pos4_Depth3()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        Assert.AreEqual(9467, MoveGen_Root(3));
    }
    
    //[Test]
    public void MoveGen_Pos4_Depth4_PreTest()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        new CMove(CSquare.F1, CSquare.F2, MoveFlag.QuietMove, PieceType.WhiteRook, PieceType.Empty).Make(Board);
        new CMove(CSquare.A8, CSquare.A7, MoveFlag.Capture, PieceType.BlackRook, PieceType.WhitePawn).Make(Board);
        new CMove(CSquare.D2, CSquare.D3, MoveFlag.QuietMove, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        foreach (var move in CMoveGeneration.MoveGen(Board)) move.Print();

        Assert.AreEqual(39, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGen_Pos4_Depth4()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        Assert.AreEqual(422333, MoveGen_Root(4));
    }
    
    [Test]
    public void MoveGen_Pos4_Depth5()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        Assert.AreEqual(15833292, MoveGen_Root(5));
    }
    
    [Test]
    public void MoveGen_Pos4_Depth6()
    {
        Board = FenReader.ImportFen("r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1 w kq - 0 1");
        Assert.AreEqual(706045033, MoveGen_Root(6));
    }
    
    [Test]
    public void MoveGen_Pos5_Depth1()
    {
        Board = FenReader.ImportFen("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8");
        Assert.AreEqual(44, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGen_Pos5_Depth2()
    {
        Board = FenReader.ImportFen("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8 ");
        Assert.AreEqual(1486, MoveGen_Root(2));
    }
    
    //[Test]
    public void MoveGen_Pos5_Depth3_PreTest()
    {
        Board = FenReader.ImportFen("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8 ");
        new CMove(CSquare.A2, CSquare.A3, MoveFlag.QuietMove, PieceType.WhitePawn, PieceType.Empty).Make(Board);
        Assert.AreEqual(1373, MoveGen_Root(2));
    }
    
    [Test]
    public void MoveGen_Pos5_Depth3()
    {
        Board = FenReader.ImportFen("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8 ");
        Assert.AreEqual(62379, MoveGen_Root(3));
    }
    
    [Test]
    public void MoveGen_Pos5_Depth4()
    {
        Board = FenReader.ImportFen("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8 ");
        Assert.AreEqual(2103487, MoveGen_Root(4));
    }
    
    [Test]
    public void MoveGen_Pos5_Depth5()
    {
        Board = FenReader.ImportFen("rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R w KQ - 1 8 ");
        Assert.AreEqual(89941194, MoveGen_Root(5));
    }
    
    [Test]
    public void MoveGen_Pos6_Depth1()
    {
        Board = FenReader.ImportFen("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
        Assert.AreEqual(46, CMoveGeneration.MoveGen(Board).Count);
    }
    
    [Test]
    public void MoveGen_Pos6_Depth2()
    {
        Board = FenReader.ImportFen("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
        Assert.AreEqual(2079, MoveGen_Root(2));
    }
    
    [Test]
    public void MoveGen_Pos6_Depth3()
    {
        Board = FenReader.ImportFen("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
        Assert.AreEqual(89890, MoveGen_Root(3));
    }
    
    [Test]
    public void MoveGen_Pos6_Depth4()
    {
        Board = FenReader.ImportFen("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
        Assert.AreEqual(3894594, MoveGen_Root(4));
    }
    
    [Test]
    public void MoveGen_Pos6_Depth5()
    {
        Board = FenReader.ImportFen("r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1 w - - 0 10");
        Assert.AreEqual(164075551, MoveGen_Root(5));
    }
    
    private long MoveGen_Root(int n)
    {
        List<CMove> moves = CMoveGeneration.MoveGen(Board);
        long moveCount = 0;
        foreach (var move in moves)
        {
            move.Make(Board);
            long count = MoveGeneration_DepthN(n - 2);
            TestContext.WriteLine($"{move}: {count}");
            moveCount += count;
            move.Unmake(Board);
        }

        return moveCount;
    }
    
    private long MoveGeneration_DepthN(int n)
    {
        if (n == 0) return CMoveGeneration.MoveGen(Board).Count;
        List<CMove> moves = CMoveGeneration.MoveGen(Board);
        long moveCount = 0;
        foreach (var move in moves)
        {
            move.Make(Board);
            moveCount += MoveGeneration_DepthN(n - 1);
            move.Unmake(Board);
        }

        return moveCount;
    }
}