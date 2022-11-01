namespace Chess;

public class MainEngine : AlphaBetaEngine
{
    private const int BishopPairValue = 50;
    private const int RookPairValue = -10;
    private const int KnightPairValue = -10;

    public override int Evaluate(CBoard board)
    {
        int evaluation = 0;
        evaluation += EvaluateMaterial(board);
        evaluation += EvaluatePieceSquares(board);

        return board.ToMove == ColorType.White ? evaluation : -evaluation;
    }

    public int EvaluatePieceSquares(CBoard board)
    {
        int evaluation = 0;
        for (int i = 0; i < 12; i++)
        {
            ulong bitBoard = board.GetPieceSet((PieceType)i);
            while (bitBoard != 0)
            {
                byte square = CUtils.BitScanForward(bitBoard);
                bitBoard = CUtils.ResetLs1B(bitBoard);
                if (i % 2 == 0) evaluation += PieceSquareTable.PieceSquareTables[0][i][square];
                else evaluation -= PieceSquareTable.PieceSquareTables[0][i][square];
            }
        }

        return evaluation;
    }

    private int EvaluateMaterial(CBoard board)
    {
        int evaluation = 0;
        byte[] popCount = new byte[12];
        for (byte i = 0; i < 12; i++) popCount[i] = board.PopCount(i);

        for (byte i = 0; i < 12; i++) // Raw Material Weight
            if (i % 2 == 0) evaluation += PieceValues[i] * popCount[i];
            else evaluation -= PieceValues[i] * popCount[i];

        if (popCount[2] >= 2) evaluation += KnightPairValue; // Knight Pair Penalty
        if (popCount[3] >= 2) evaluation -= KnightPairValue;

        if (popCount[4] >= 2) evaluation += BishopPairValue; // Bishop Pair Bonus
        if (popCount[5] >= 2) evaluation -= BishopPairValue;

        if (popCount[6] >= 2) evaluation += RookPairValue; // Rook Pair Penalty
        if (popCount[7] >= 2) evaluation -= RookPairValue;

        return evaluation;
    }
}