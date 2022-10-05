using Chess;
using NUnit.Framework;

namespace ChessTests.CMoveGenerationTests;

[SetUpFixture]
public class CMoveGenerationTestsSetup
{
    [OneTimeSetUp]
    public void SetUp()
    {
        CAttackMap.Calculate();
    }
}