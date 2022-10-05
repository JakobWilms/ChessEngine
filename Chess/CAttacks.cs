using System;

namespace Chess;

public static class CAttacks
{
    public static ulong RookAttacks(ulong occupied, CSquare square) =>
        FileAttacks(occupied, square) | RankAttacks(occupied, square);

    public static ulong BishopAttacks(ulong occupied, CSquare square) =>
        DiagonalAttacks(occupied, square) | AntiDiagonalAttacks(occupied, square);

    public static ulong QueenAttacks(ulong occupied, CSquare square) =>
        RookAttacks(occupied, square) | BishopAttacks(occupied, square);

    public static ulong DiagonalAttacks(ulong occupied, CSquare from) =>
        GetPositiveRayAttacks(occupied, CDir.NoEa, from)
        | GetNegativeRayAttacks(occupied, CDir.SoWe, from);

    public static ulong AntiDiagonalAttacks(ulong occupied, CSquare from) =>
        GetPositiveRayAttacks(occupied, CDir.NoWe, from)
        | GetNegativeRayAttacks(occupied, CDir.SoEa, from);

    public static ulong FileAttacks(ulong occupied, CSquare from) =>
        GetPositiveRayAttacks(occupied, CDir.Nort, from)
        | GetNegativeRayAttacks(occupied, CDir.Sout, from);

    public static ulong RankAttacks(ulong occupied, CSquare from) =>
        GetPositiveRayAttacks(occupied, CDir.East, from)
        | GetNegativeRayAttacks(occupied, CDir.West, from);

    private static ulong GetPositiveRayAttacks(ulong occupied, CDir dir, CSquare from) =>
        GetPositiveRayAttacks(occupied, (byte)dir, (byte)from);

    private static ulong GetNegativeRayAttacks(ulong occupied, CDir dir, CSquare from) =>
        GetNegativeRayAttacks(occupied, (byte)dir, (byte)from);

    private static ulong GetPositiveRayAttacks(ulong occupied, byte dir, byte square)
    {
        ulong attacks = CAttackMap.RayAttacks[square][dir];
        ulong blocker = attacks & occupied;
        if (blocker != 0)
        {
            square = BitScanForward(blocker);
            attacks ^= CAttackMap.RayAttacks[square][dir];
        }

        return attacks;
    }

    private static ulong GetNegativeRayAttacks(ulong occupied, byte dir, byte square)
    {
        ulong attacks = CAttackMap.RayAttacks[square][dir];
        ulong blocker = attacks & occupied;
        if (blocker != 0)
        {
            square = BitScanRevers(blocker);
            attacks ^= CAttackMap.RayAttacks[square][dir];
        }

        return attacks;
    }

    private static byte BitScanForward(ulong bb)
    {
        if (bb == 0) throw new ArgumentOutOfRangeException();
        return Index64Forward[(IsolateLs1B(bb) * DeBruin64) >> 58];
    }

    public static byte BitScanForwardIsolatedLs1B(ulong bb) => Index64Forward[bb * DeBruin64 >> 58];

    private static byte BitScanRevers(ulong bb)
    {
        if (bb == 0) throw new ArgumentOutOfRangeException();
        bb |= bb >> 1;
        bb |= bb >> 2;
        bb |= bb >> 4;
        bb |= bb >> 8;
        bb |= bb >> 16;
        bb |= bb >> 32;
        return Index64Reverse[(bb * DeBruin64) >> 58];
    }


    private static ulong IsolateLs1B(ulong l) => l & (l - 1) ^ l;

    private static readonly byte[] Index64Forward =
    {
        0, 1, 48, 2, 57, 49, 28, 3,
        61, 58, 50, 42, 38, 29, 17, 4,
        62, 55, 59, 36, 53, 51, 43, 22,
        45, 39, 33, 30, 24, 18, 12, 5,
        63, 47, 56, 27, 60, 41, 37, 16,
        54, 35, 52, 21, 44, 32, 23, 11,
        46, 26, 40, 15, 34, 20, 31, 10,
        25, 14, 19, 9, 13, 8, 7, 6
    };

    private static readonly byte[] Index64Reverse =
    {
        0, 47, 1, 56, 48, 27, 2, 60,
        57, 49, 41, 37, 28, 16, 3, 61,
        54, 58, 35, 52, 50, 42, 21, 44,
        38, 32, 29, 23, 17, 11, 4, 62,
        46, 55, 26, 59, 40, 36, 15, 53,
        34, 51, 20, 43, 31, 22, 10, 45,
        25, 39, 14, 33, 19, 30, 9, 24,
        13, 18, 8, 12, 7, 6, 5, 63
    };

    private const ulong DeBruin64 = 0x03f79d71b4cb0a89;
}