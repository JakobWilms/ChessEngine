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
            square = CUtils.BitScanForward(blocker);
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
            square = CUtils.BitScanRevers(blocker);
            attacks ^= CAttackMap.RayAttacks[square][dir];
        }

        return attacks;
    }
}