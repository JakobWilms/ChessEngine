using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Chess.Book;

public class Pgn
{
    private const int TabSize = 8;
    private const int Eof = -1;

    private StreamReader? _reader;

    public int CharLine { get; private set; }
    public int CharColumn { get; private set; }
    private bool _charUnread;
    private bool _charFirst;
    private bool _tokenUnread;
    private bool _tokenFirst;
    public TokenType TokenType { get; private set; }
    private int _tokenLength;
    private StringBuilder _tokenString;
    public string MoveString { get; private set; }

    private int _charHack;

    public Pgn()
    {
        MoveString = "";
        _tokenString = new StringBuilder();
    }

    public void Open(string fileName)
    {
        _reader = new StreamReader(fileName);
        CharLine = 1;
        CharColumn = 0;
        _charUnread = false;
        _charFirst = true;
        _tokenUnread = false;
        _tokenFirst = true;
        TokenType = TokenType.Error;
    }

    public void Extract(string inFile, string outFile, int maxPly)
    {
        List<string> output = new List<string>();
        StringBuilder builder = new StringBuilder();
        Open(inFile);
        int games = 0;
        do
        {
            builder.Clear();
            for (int ply = 0; NextMove(); ply++)
                if (ply <= maxPly)
                    builder.Append(MoveString).Append(' ');
            output.Add(builder.ToString());
            games++;
            Console.WriteLine($"{games} Games!, l: {CharLine}");
        } while (NextGame());
        
        Close();
        System.IO.File.WriteAllLines(outFile, output);
    }

    public void Close() => _reader?.Close();

    public bool NextGame()
    {
        TokenRead();

        if (_charHack == -1) return false;

        TokenUnread();
        return true;
    }

    public bool NextMove()
    {
        while (true)
        {
            TokenRead();
            if (_charHack == -1) return false;
            if (TokenType is TokenType.ResultWin or TokenType.ResultLoss or TokenType.ResultDraw or TokenType.Eof)
                return false;
            MoveString = _tokenString.ToString();
            return true;
        }
    }

    private void TokenRead()
    {
        if (_tokenUnread)
        {
            _tokenUnread = false;
            return;
        }

        if (_tokenFirst)
        {
            _tokenFirst = false;
        }
        else if (TokenType is TokenType.Error or TokenType.Eof) throw new ApplicationException();

        ReadToken();
        if (TokenType == TokenType.Error)
            Console.WriteLine($"Lexical Error at line {CharLine}, column {CharColumn}");
    }

    private void TokenUnread()
    {
        if (_tokenUnread || _tokenFirst) throw new ApplicationException();
        _tokenUnread = true;
    }

    private void ReadToken()
    {
        SkipBlanks();
        if (_charHack == -1) return;
        TokenType = TokenType.Error;
        _tokenLength = 0;
        if (_charHack == Eof)
        {
            TokenType = TokenType.Eof;
        }
        else if (IsSymbolSkip(_charHack))
        {
            _tokenString = new StringBuilder();
            CharRead();
            while (IsSymbolSkip(_charHack))
            {
                _tokenString.Append((char)_charHack);
                CharRead();
            }

            if (_tokenString.ToString().Equals("1-0")) TokenType = TokenType.ResultWin;
            else if (_tokenString.ToString().Equals("0-1")) TokenType = TokenType.ResultLoss;
            else if (_tokenString.ToString().Equals("1/2-1/2")) TokenType = TokenType.ResultDraw;
            else ReadToken();
        }
        else if (IsSymbolStart(_charHack))
        {
            TokenType = TokenType.Integer;
            _tokenLength = 0;
            _tokenString = new StringBuilder();
            do
            {
                if (!char.IsDigit((char)_charHack)) TokenType = TokenType.Symbol;

                _tokenString.Append((char)_charHack);
                _tokenLength++;

                CharRead(false);
            } while (IsSymbolNext(_charHack));

            CharUnread();
            if (_tokenLength <= 0) throw new ApplicationException();
        }
    }

    private void SkipBlanks()
    {
        while (true)
        {
            CharRead();
            if (_charHack == 1) return;
            char c = (char)_charHack;
            if (c is ' ' or '\n' or '\t')
            {
            }
            else if (c == '[')
            {
                int open = 1;
                while (open > 0)
                {
                    CharRead();
                    if (_charHack == -1) return;
                    if ((char)_charHack == '[') open++;
                    else if ((char)_charHack == ']') open--;
                }
                CharRead();
            }
            else
            {
                if (!_charUnread) CharUnread();
                break;
            }
        }
    }

    private void CharRead(bool considerUnread = true)
    {
        if (_charUnread && considerUnread)
        {
            _charUnread = false;
            return;
        }

        if (!considerUnread) _charUnread = false;

        char c = (char)_charHack;

        if (_charFirst)
        {
            _charFirst = false;
        }
        else if (c == '\n')
        {
            CharLine++;
            CharColumn = 0;
        }
        else if (c == '\t')
            CharColumn += TabSize - CharColumn % TabSize;
        else
            CharColumn++;

        _charHack = _reader!.Read();
    }

    private void CharUnread()
    {
        if (_charUnread || _charFirst) throw new AggregateException();

        _charUnread = true;
    }

    private bool IsSymbolStart(int c) =>
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".Contains((char)c);

    private bool IsSymbolNext(int c) =>
        "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_+#=:-/".Contains((char)c);

    private bool IsSymbolSkip(int c) => "0123456789-.".Contains((char)c);
}

public enum TokenType
{
    Error = -1,
    Eof = 256,
    Symbol = 257,
    String = 258,
    Integer = 259,
    Nag = 260,
    ResultWin = 261,
    ResultLoss = 262,
    ResultDraw = 263
}