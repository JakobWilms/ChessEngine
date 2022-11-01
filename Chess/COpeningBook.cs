using System.Collections.Generic;
using System.Text;
using Chess.Book;

namespace Chess;

public class COpeningBook
{
    private readonly SortedDictionary<ulong, CBookEntry> _entries;

    public COpeningBook(string file)
    {
        _entries = new CBook().Open(file);
    }

    public CBookEntry? BookEntry(CBoard board) => BookEntry(board.GetHash());

    public CBookEntry? BookEntry(ulong hash)
    {
        return _entries.ContainsKey(hash) ? _entries[hash] : null;
    }
}

public class OpeningDisplay
{
    private readonly CMove[] _moves;
    private readonly ushort[] _counts;

    public OpeningDisplay(CBookEntry entry, CBoard board)
    {
        _moves = new CMove[entry.Index];
        _counts = new ushort[entry.Index];

        for (int i = 0; i < entry.Index; i++)
        {
            _counts[i] = entry.Counts[i];
            _moves[i] = new CMove(entry.Moves[i]!.From, entry.Moves[i]!.To, board);
        }
    }

    public string ToString(CBoard board)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < _moves.Length; i++)
        {
            builder.Append(_moves[i].ToSan(board)).Append("   ").Append(_counts[i]).Append('\n');
        }

        return builder.ToString();
    }
}