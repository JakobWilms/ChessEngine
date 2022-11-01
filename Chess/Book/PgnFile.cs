using System.IO;
using System.Text;

namespace Chess.Book;

public static class PgnFile
{

    public static void Extract(string inFile, string outFile)
    {
        StreamReader reader = new StreamReader(inFile);
        StringBuilder builder = new StringBuilder();
        int games = 0;
        int lineNumber = 1, columnNumber = 1;
        Tt tt = Tt.MoveNumber;
        using var writer = new StreamWriter(outFile);
        for (string? line = reader.ReadLine(); line != null; line = reader.ReadLine())
        {
            if (line == "" || line[0] == '[')
            {
                lineNumber++;
                continue;
            }

            for (var index = 0; index < line.Length; index++)
            {
                var c = line[index];
                if (tt == Tt.Result)
                {
                    lineNumber++;
                    columnNumber = 1;
                    writer.WriteLine(builder.ToString());
                    builder.Clear();
                    tt = Tt.MoveNumber;
                    games++;
                    if (games % 1000 == 0) Console.WriteLine($"{games} Games!");
                    continue;
                }

                if (c == ' ')
                {
                    switch (tt)
                    {
                        case Tt.MoveNumber:
                            tt = Tt.WMove;
                            break;
                        case Tt.WMove:
                            tt = Tt.MoveOrResult;
                            builder.Append(' ');
                            break;
                        case Tt.BMove:
                            tt = Tt.MoveNumberOrResult;
                            builder.Append(' ');
                            break;
                    }

                    continue;
                }

                if (tt == Tt.MoveNumberOrResult)
                {
                    if (c == '.') tt = Tt.MoveNumber;
                    else if (c == '-') tt = Tt.Result;
                }
                else if (tt == Tt.MoveOrResult)
                {
                    tt = char.IsDigit(c) ? Tt.Result : Tt.BMove;
                }

                if (tt is Tt.WMove or Tt.BMove)
                {
                    builder.Append(c);
                }
            }
        }
        
        Console.WriteLine($"Done. {games} Games.");
        Console.WriteLine($"l: {lineNumber}, c: {columnNumber}");
    }
}

public enum Tt
{
    MoveNumber,
    WMove,
    BMove,
    Result,
    MoveOrResult,
    MoveNumberOrResult
}