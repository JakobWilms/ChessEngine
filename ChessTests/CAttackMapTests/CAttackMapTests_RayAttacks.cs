using Chess;
using NUnit.Framework;
using static Chess.CDir;
using static Chess.CSquare;

namespace ChessTests.CAttackMapTests;

public class CAttackMapTestsRayAttacks
{
    [Test]
    public void NorthAttacks_Correct()
    {
        Assert.AreEqual(0x_0101_0101_0101_0100, CAttackMap.RayAttacks[(byte)A1][(byte)Nort]);
        Assert.AreEqual(0x_0808_0808_0000_0000, CAttackMap.RayAttacks[(byte)D4][(byte)Nort]);
    }
    
    [Test]
    public void EastAttacks_Correct()
    {
        Assert.AreEqual(0x_0000_0000_0000_00fe, CAttackMap.RayAttacks[(byte)A1][(byte)East]);
        Assert.AreEqual(0x_0000_0000_f000_0000, CAttackMap.RayAttacks[(byte)D4][(byte)East]);
    }
    
    [Test]
    public void SouthAttacks_Correct()
    {
        Assert.AreEqual(0x_0080_8080_8080_8080, CAttackMap.RayAttacks[(byte)H8][(byte)Sout]);
        Assert.AreEqual(0x0000000000080808, CAttackMap.RayAttacks[(byte)D4][(byte)Sout]);
    }
    
    [Test]
    public void WestAttacks_Correct()
    {
        Assert.AreEqual(0x_7f00_0000_0000_0000, CAttackMap.RayAttacks[(byte)H8][(byte)West]);
        Assert.AreEqual(0x_0000_0000_0700_0000, CAttackMap.RayAttacks[(byte)D4][(byte)West]);
    }
    
    [Test]
    public void NorthEastAttacks_Correct()
    {
        Assert.AreEqual(0x_8040_2010_0804_0200, CAttackMap.RayAttacks[(byte)A1][(byte)NoEa]);
        Assert.AreEqual(0x_8040_2010_0000_0000, CAttackMap.RayAttacks[(byte)D4][(byte)NoEa]);
    }
    
    [Test]
    public void NorthWestAttacks_Correct()
    {
        Assert.AreEqual(0x_0102_0408_1020_4000, CAttackMap.RayAttacks[(byte)H1][(byte)NoWe]);
        Assert.AreEqual(0x_0001_0204_0000_0000, CAttackMap.RayAttacks[(byte)D4][(byte)NoWe]);
    }
    
    [Test]
    public void SouthEastAttacks_Correct()
    {
        Assert.AreEqual(0x_0002_0408_1020_4080, CAttackMap.RayAttacks[(byte)A8][(byte)SoEa]);
        Assert.AreEqual(0x_0000_0000_0010_2040, CAttackMap.RayAttacks[(byte)D4][(byte)SoEa]);
    }
    
    [Test]
    public void SouthWestAttacks_Correct()
    {
        Assert.AreEqual(0x_0040_2010_0804_0201, CAttackMap.RayAttacks[(byte)H8][(byte)SoWe]);
        Assert.AreEqual(0x_0000_0000_0004_0201, CAttackMap.RayAttacks[(byte)D4][(byte)SoWe]);
    }
}