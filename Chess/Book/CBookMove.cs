namespace Chess.Book;

public class CBookMove
{
    public CMove Move { get; }

    private CBookMove(CMove move) => Move = move;

    public static CBookMove? NewCBookMove(string moveString, CBoard board, int columnNumber,
        int lineNumber)
    {
        CBookMove? move = NewCBookMove(moveString, board);
        if (move == null) Console.WriteLine($"Error: Game {lineNumber}, c {columnNumber}, move {moveString}");
        return move;
    }

    public static CBookMove? NewCBookMove(string moveString, CBoard board)
    {
        moveString = moveString.Replace("+", "").Replace("#", "");
        CMove? move = null;
        CMove?[] moves = CMoveGeneration.MoveGen(board);
        for (int i = 0; i < moves.Length; i++)
        {
            if (moves[i] == null) break;
            if (!moves[i]!.ToSan(moves).Equals(moveString)) continue;
            move = moves[i];
            break;
        }

        return move == null ? null : new CBookMove(move);
    }

    public override bool Equals(object? obj) => Move.Equals(obj);

    public override int GetHashCode() => Move.GetHashCode();
}