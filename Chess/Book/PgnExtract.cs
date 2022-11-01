using System.IO;
using System.Text;

namespace Chess.Book;

public class PgnExtract
{
    private readonly StreamReader _reader;
    public string? Line { get; private set; }
    public int ColumnNumber { get; private set; }
    public int LineNumber { get; private set; }
    private readonly StringBuilder _builder;
    public string MoveString { get; private set; }

    public PgnExtract(string fileName)
    {
        _reader = new StreamReader(fileName);
        _builder = new StringBuilder();
        MoveString = "";
        LineNumber = 1;
    }

    public bool NextGame()
    {
        Line = _reader.ReadLine();
        ColumnNumber = 0;
        LineNumber++;
        return Line != null;
    }

    public bool NextMove()
    {
        _builder.Clear();
        if (Line!.Length <= ColumnNumber) return false;
        for (char c = Line![ColumnNumber]; c != ' '; ColumnNumber++, c = Line[ColumnNumber])
        {
            _builder.Append(c);
            if (Line.Length == ColumnNumber) break;
        }

        MoveString = _builder.ToString();
        ColumnNumber++;
        return true;
    }
}