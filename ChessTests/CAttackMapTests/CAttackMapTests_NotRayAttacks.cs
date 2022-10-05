using Chess;
using NUnit.Framework;
using static Chess.CSquare;

namespace ChessTests.CAttackMapTests;

public class CAttackMapTestsNotRayAttacks
{
    [Test]
    public void KingAttacks_Correct()
    {
        Assert.AreEqual(0x0302, CAttackMap.ArrKingAttacks[(byte)A1]);
        Assert.AreEqual(0x_1c_141c_0000, CAttackMap.ArrKingAttacks[(byte)D4]);
    }
    
    [Test]
    public void KnightAttacks_Correct()
    {
        Assert.AreEqual(0x020400, CAttackMap.ArrKnightAttacks[(byte)A1]);
        Assert.AreEqual(0x_0000_1422_0022_1400, CAttackMap.ArrKnightAttacks[(byte)D4]);
    }
    
    [Test]
    public void PawnAttacks_Correct()
    {
        Assert.AreEqual(0x0200, CAttackMap.ArrPawnAttacks[0][(byte)A1]);
        Assert.AreEqual(0x1400000000, CAttackMap.ArrPawnAttacks[0][(byte)D4]);
    }
}