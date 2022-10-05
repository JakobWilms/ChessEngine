using static Chess.CSquares;
using static Chess.CSquare;

namespace Chess;

public static class CAttackMap
{
    public static readonly ulong[][] RayAttacks = new ulong[64][]; // Square, Direction

    private static readonly ulong[] RankAttacks = new ulong[64];
    private static readonly ulong[] FileAttacks = new ulong[64];
    private static readonly ulong[] DiagonalAttacks = new ulong[64];
    private static readonly ulong[] AntiDiagonalAttacks = new ulong[64];

    public static readonly ulong[][] ArrPawnAttacks = new ulong[2][]; // White/Black, Square
    public static readonly ulong[] ArrKnightAttacks = new ulong[64];
    public static readonly ulong[] ArrBishopAttacks = new ulong[64];
    public static readonly ulong[] ArrRookAttacks = new ulong[64];
    public static readonly ulong[] ArrQueenAttacks = new ulong[64];
    public static readonly ulong[] ArrKingAttacks = new ulong[64];


    public static void Calculate()
    {
        for (int i = 0; i < 64; i++) RayAttacks[i] = new ulong[8];
        AttackWest();
        AttackNorthWest();
        AttackNorth();
        AttackNorthEast();
        AttackEast();
        AttackSouthEast();
        AttackSouth();
        AttackSouthWest();

        King();
        Knight();
        Pawn();

        for (byte sq = 0; sq < 64; sq++)
        {
            RankAttacks[sq] = RayAttacks[sq][(byte)CDir.East] | RayAttacks[sq][(byte)CDir.West];
            FileAttacks[sq] = RayAttacks[sq][(byte)CDir.Nort] | RayAttacks[sq][(byte)CDir.Sout];
            DiagonalAttacks[sq] = RayAttacks[sq][(byte)CDir.NoEa] | RayAttacks[sq][(byte)CDir.SoWe];
            AntiDiagonalAttacks[sq] = RayAttacks[sq][(byte)CDir.NoWe] | RayAttacks[sq][(byte)CDir.SoEa];
            ArrBishopAttacks[sq] = DiagonalAttacks[sq] | AntiDiagonalAttacks[sq];
            ArrRookAttacks[sq] = RankAttacks[sq] | FileAttacks[sq];
            ArrQueenAttacks[sq] = ArrBishopAttacks[sq] | ArrRookAttacks[sq];
        }
    }

    private static void King()
    {
        ulong sqBb = One;
        for (int sq = 0; sq < 64; sq++, sqBb <<= 1) ArrKingAttacks[sq] = KingAttacks(sqBb);
    }
    private static void Knight()
    {
        ulong sqBb = One;
        for (int sq = 0; sq < 64; sq++, sqBb <<= 1) ArrKnightAttacks[sq] = KnightAttacks(sqBb);
    }
    
    private static void Pawn()
    {
        ArrPawnAttacks[0] = new ulong[64];
        ArrPawnAttacks[1] = new ulong[64];
        ulong sqBb = One;
        for (int sq = 0; sq < 64; sq++, sqBb <<= 1)
        {
            ArrPawnAttacks[0][sq] = WPawnAttacks(sqBb);
            ArrPawnAttacks[1][sq] = BPawnAttacks(sqBb);
        }
    }

    private static ulong WPawnAttacks(ulong pawns) => NorthWest(pawns) | NorthEast(pawns);
    private static ulong BPawnAttacks(ulong pawns) => SouthWest(pawns) | SouthEast(pawns);

    private static ulong KnightAttacks(ulong knights) =>
        NoNoEa(knights) | NoEaEa(knights) | SoEaEa(knights) | SoSoEa(knights) | SoSoWe(knights) |
        SoWeWe(knights) | NoWeWe(knights) | NoNoWe(knights);

    private static ulong KingAttacks(ulong kingSet)
    {
        ulong attacks = EastOne(kingSet) | WestOne(kingSet);
        kingSet |= attacks;
        attacks |= NorthOne(kingSet) | SouthOne(kingSet);
        return attacks;
    }

    private static void AttackNorth()
    {
        ulong nort = AFile & ~One;
        for (int sq = 0; sq < 64; sq++, nort <<= 1) RayAttacks[sq][(byte)CDir.Nort] = nort;
    }

    private static void AttackSouth()
    {
        ulong sout = HFile & ~(One << (byte)H8);
        for (int sq = 63; sq >= 0; sq--, sout >>= 1) RayAttacks[sq][(byte)CDir.Sout] = sout;
    }

    private static void AttackEast()
    {
        ulong east = Rank1 & ~One;
        for (int r8 = 0; r8 < 64; r8 += 8, east <<= 8)
        {
            ulong ea = east;
            for (int f = 0; f < 8; f++, ea = EastOne(ea)) RayAttacks[r8 + f][(byte)CDir.East] = ea;
        }
    }

    private static void AttackWest()
    {
        ulong west = Rank8 & ~(One << (byte)H8);
        for (int r8 = 56; r8 >= 0; r8 -= 8, west >>= 8)
        {
            ulong we = west;
            for (int f = 7; f >= 0; f--, we = WestOne(we)) RayAttacks[r8 + f][(byte)CDir.West] = we;
        }
    }

    private static void AttackNorthEast()
    {
        ulong noEa = A1H8Diagonal & ~One;
        for (int f = 0; f < 8; f++, noEa = EastOne(noEa))
        {
            ulong ne = noEa;
            for (int r8 = 0; r8 < 64; r8 += 8, ne <<= 8) RayAttacks[r8 + f][(byte)CDir.NoEa] = ne;
        }
    }

    private static void AttackSouthEast()
    {
        ulong soEa = H1A8Diagonal & ~(One << (byte)A8);
        for (int r8 = 56; r8 >= 0; r8 -= 8, soEa >>= 8)
        {
            ulong se = soEa;
            for (int f = 0; f < 8; f++, se = EastOne(se)) RayAttacks[r8 + f][(byte)CDir.SoEa] = se;
        }
    }

    private static void AttackNorthWest()
    {
        ulong noWe = H1A8Diagonal & ~(One << (byte)H1);
        for (int r8 = 0; r8 < 64; r8 += 8, noWe <<= 8)
        {
            ulong nw = noWe;
            for (int f = 7; f >= 0; f--, nw = WestOne(nw)) RayAttacks[r8 + f][(byte)CDir.NoWe] = nw;
        }
    }

    private static void AttackSouthWest()
    {
        ulong soWe = A1H8Diagonal & ~(One << (byte)H8);
        for (int r8 = 56; r8 >= 0; r8 -= 8, soWe >>= 8)
        {
            ulong sw = soWe;
            for (int f = 7; f >= 0; f--, sw = WestOne(sw))
            {
                RayAttacks[r8 + f][(byte)CDir.SoWe] = sw;
            }
        }
    }
}