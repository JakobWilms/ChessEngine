using System.Threading.Tasks;
using Chess.Book;

namespace Chess;

public abstract class Engine
{
    
    protected static readonly int[] PieceValues = { 100, 100, 320, 320, 330, 330, 500, 500, 900, 900, 20000, 20000 };
    protected const int MinValue = int.MinValue + 1;

    private static Engine? White { get; set; }
    private static Engine? Black { get; set; }
    
    protected Random Random { get; }

    protected Engine() => Random = new Random();

    public static async void TryEngineMove(CBookEntry? entry = null)
    {
        await Task.Delay(10);
        CMove? move = null;
        if (White != null && CDisplay.Instance.Board.ToMove == ColorType.White) move = White.FindMove(entry);
        else if (Black != null && CDisplay.Instance.Board.ToMove == ColorType.Black) move = Black.FindMove(entry);
        if (move != null) CDisplay.Instance.VisibleMove(move);
    }

    protected CMove? FromMoveArray(CMove?[] moves)
    {
        int maxIndex = -1;
        for (var index = 0; index < moves.Length; index++)
        {
            var move = moves[index];
            if (move != null) continue;
            maxIndex = index;
            break;
        }

        return maxIndex == -1 ? null : moves[Random.Next(maxIndex)];
    }
    
    protected abstract CMove? FindMove(CBookEntry? entry = null);
    
    public static void ReadEngines(string[] args)
    {
        if (args.Length < 2) return;
        White = FromCode(args[1]);
        Black = FromCode(args[2]);
    }
    
    private static Engine? FromCode(string code) =>
        code switch
        {
            "r" => new RandomEngine(),
            "e" => new MainEngine(),
            "m" => new MaterialEngine(),
            "mab" => new MaterialAlphaBetaEngine(),
            "c" => new CaptureEngine(),
            "n" => null,
            _ => throw new InvalidOperationException()
        };
}