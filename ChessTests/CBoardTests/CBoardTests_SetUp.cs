using Chess;
using NUnit.Framework;

namespace ChessTests.CBoardTests;

[SetUpFixture]
public class CBoardTestsSetUp
{
    public static CBoard Board;
    
    [OneTimeSetUp]
    public void SetUp()
    {
        Board = FenReader.ImportFen(FenReader.StartingFen);
        CAttackMap.Calculate();
    }
}