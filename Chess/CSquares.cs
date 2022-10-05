using System;

namespace Chess;

public static class CSquares
{
    public const ulong HFile = 0x8080_8080_8080_8080;
    public const ulong GFile = 0x4040_4040_4040_4040;
    public const ulong FFile = 0x2020_2020_2020_2020;
    public const ulong EFile = 0x1010_1010_1010_1010;
    public const ulong DFile = 0x0808_0808_0808_0808;
    public const ulong CFile = 0x0404_0404_0404_0404;
    public const ulong BFile = 0x0202_0202_0202_0202;
    public const ulong AFile = 0x_0101_0101_0101_0101;

    public const ulong Rank1 = 0xff;
    public const ulong Rank2 = 0xff00;
    public const ulong Rank3 = 0xff0000;
    public const ulong Rank4 = 0xff000000;
    public const ulong Rank5 = 0xff00000000;
    public const ulong Rank6 = 0xff0000000000;
    public const ulong Rank7 = 0xff000000000000;
    public const ulong Rank8 = 0xff00000000000000;

    public const ulong A1H8Diagonal = 0x8040201008040201;
    public const ulong H1A8Diagonal = 0x0102040810204080;

    public const ulong DarkSquares = 0xAA55AA55AA55AA55;
    public const ulong LightSquares = 0x55AA55AA55AA55AA;

    public const ulong One = 0x1;

    private static readonly sbyte[] SimpleDirs = { -9, -8, -7, -1, 1, 7, 8, 9 };

    public static byte RowOf(CSquare square) => (byte)((byte)square >> 3);

    private static sbyte AddendOf(CDir dir) =>
        dir switch
        {
            CDir.Nort => 8,
            CDir.NoEa => 9,
            CDir.East => 1,
            CDir.SoEa => -7,
            CDir.Sout => -8,
            CDir.SoWe => -9,
            CDir.West => -1,
            CDir.NoWe => 7,
            _ => throw new ArgumentOutOfRangeException()
        };

    public static CSquare AddDir(CSquare square, CDir dir) => (CSquare)((byte)square + AddendOf(dir));

    public static bool IsSquare(CSquare square, CDir dir)
    {
        sbyte addend = AddendOf(dir);
        if (dir is CDir.Nort or CDir.Sout) return (byte)square + addend is < 64 and >= 0;
        ulong sq = One << (byte)square;
        ulong shift = dir switch
        {
            CDir.NoEa => NorthEast(sq),
            CDir.East => EastOne(sq),
            CDir.SoEa => SouthEast(sq),
            CDir.SoWe => SouthWest(sq),
            CDir.West => WestOne(sq),
            CDir.NoWe => NorthWest(sq),
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
        return shift != 0;
    }

    public static bool IsSquare(CSquare square, CDoubleDir dir)
    {
        ulong sq = One << (byte)square;
        ulong shift = dir switch
        {
            CDoubleDir.Nne => NoNoEa(sq),
            CDoubleDir.Nee => NoEaEa(sq),
            CDoubleDir.See => SoEaEa(sq),
            CDoubleDir.Sse => SoSoEa(sq),
            CDoubleDir.Ssw => SoSoWe(sq),
            CDoubleDir.Sww => SoWeWe(sq),
            CDoubleDir.Nww => NoWeWe(sq),
            CDoubleDir.Nnw => NoNoWe(sq),
            CDoubleDir.Nn => NorthOne(NorthOne(sq)),
            CDoubleDir.Ee => EastOne(EastOne(sq)),
            CDoubleDir.Ss => WestOne(WestOne(sq)),
            CDoubleDir.Ww => WestOne(WestOne(sq)),
            CDoubleDir.N => NorthOne(sq),
            CDoubleDir.Ne => NorthEast(sq),
            CDoubleDir.E => EastOne(sq),
            CDoubleDir.Se => SouthEast(sq),
            CDoubleDir.S => SouthOne(sq),
            CDoubleDir.Sw => SouthWest(sq),
            CDoubleDir.W => WestOne(sq),
            CDoubleDir.Nw => NorthWest(sq),
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, null)
        };
        return shift != 0;
    }
    
    public static ulong EastOne(ulong l) => (l << 1) & ~AFile;
    public static ulong WestOne(ulong l) => (l >> 1) & ~HFile;
    public static ulong NorthOne(ulong l) => l << 8;
    public static ulong SouthOne(ulong l) => l >> 8;

    public static ulong NorthEast(ulong l) => (l << 9) & ~AFile;
    public static ulong SouthEast(ulong l) => (l >> 7) & ~AFile;
    public static ulong SouthWest(ulong l) => (l >> 9) & ~HFile;
    public static ulong NorthWest(ulong l) => (l << 7) & ~HFile;

    public static ulong NoNoEa(ulong l) => (l << 17) & ~AFile;
    public static ulong NoEaEa(ulong l) => (l << 10) & ~(AFile | BFile);
    public static ulong SoEaEa(ulong l) => (l >> 6) & ~(AFile | BFile);
    public static ulong SoSoEa(ulong l) => (l >> 15) & ~AFile;
    public static ulong SoSoWe(ulong l) => (l >> 17) & ~HFile;
    public static ulong SoWeWe(ulong l) => (l >> 10) & ~(GFile | HFile);
    public static ulong NoWeWe(ulong l) => (l << 6) & ~(GFile | HFile);
    public static ulong NoNoWe(ulong l) => (l << 15) & ~HFile;
    
}