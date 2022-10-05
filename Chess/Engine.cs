using System;
using System.Threading.Tasks;

namespace Chess;

public abstract class Engine
{
    public static Engine? White { get; private set; }
    public static Engine? Black { get; private set; }

    protected readonly ColorType Color;

    protected Engine(ColorType color) => Color = color;
    
    public static async void TryEngineMove()
    {
        await Task.Delay(10);
        CMove? move = null;
        if (CDisplay.Instance.Board.ToMove == ColorType.White && White != null) move = White.FindMove();
        else if (CDisplay.Instance.Board.ToMove == ColorType.Black && Black != null) move = Black.FindMove();
        if (move != null) CDisplay.Instance.VisibleMove(move);
    }
    
    protected abstract CMove? FindMove();
    
    public static void ReadEngines(string[] args)
    {
        if (args.Length < 2) return;
        White = FromCode(args[1][0], ColorType.White);
        Black = FromCode(args[2][0], ColorType.Black);
    }
    
    private static Engine? FromCode(char code, ColorType color) =>
        code switch
        {
            'r' => new RandomEngine(color),
            'n' => null,
            _ => throw new InvalidOperationException()
        };
}