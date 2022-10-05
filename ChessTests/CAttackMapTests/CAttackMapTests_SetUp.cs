using Chess;
using NUnit.Framework;

namespace ChessTests.CAttackMapTests;

[SetUpFixture]
public class CAttackMapTestsSetUp
{
    
    [OneTimeSetUp]
    public void SetUp()
    {
        CAttackMap.Calculate();
    }
}