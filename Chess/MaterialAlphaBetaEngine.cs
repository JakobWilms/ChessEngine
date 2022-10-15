namespace Chess;

public class MaterialAlphaBetaEngine : AlphaBetaEngine
{
    public MaterialAlphaBetaEngine(ColorType color) : base(color)
    {
    }

    protected override int Evaluate(CBoard board)
    {
        int evaluation = 0;
        byte[] popCount = new byte[12];
        for (byte i = 0; i < 12; i++) popCount[i] = board.PopCount(i);

        for (byte i = 0; i < 12; i++) // Raw Material Weight
            if (i % 2 == 0) evaluation += PieceValues[i] * popCount[i];
            else evaluation -= PieceValues[i] * popCount[i];

        return board.ToMove == ColorType.White ? evaluation : -evaluation;
    }
}