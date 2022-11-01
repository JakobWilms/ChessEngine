using Chess;
using NUnit.Framework;

namespace ChessTests.CEvaluationTests;

[SetUpFixture]
public class CEvaluationTestsSetup
{
    [OneTimeSetUp]
    public void SetUp() => CAttackMap.Calculate();
}