using System.Collections;

namespace Chess;

public class TranspositionTable
{
    private readonly Hashtable _table;

    public TranspositionTable()
    {
        _table = new Hashtable();
    }

    public void Set(ulong hash, PositionInformation information)
    {
        if (!_table.Contains(hash)) _table.Add(hash, information);
        else
        {
            PositionInformation old = (PositionInformation)_table[hash]!;
            if (information.Depth > old.Depth ||
                (byte)old.NodeType > (byte)information.NodeType
                || old.BestMove == null && information.BestMove != null)
            {
                _table[hash] = information;
            }
        }
    }

    public PositionInformation? Get(ulong hash) => _table[hash] != null ? (PositionInformation)_table[hash]! : null;
}

public class PositionInformation
{
    public CMove? BestMove;
    public int Depth;
    public NodeType NodeType;
    public int Score;

    public PositionInformation(CMove? bestMove, int depth, int score, NodeType type)
    {
        BestMove = bestMove;
        Depth = depth;
        Score = score;
        NodeType = type;
    }
}