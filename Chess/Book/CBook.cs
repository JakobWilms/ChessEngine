using System.Collections.Generic;
using System.Linq;

namespace Chess.Book;

public class CBook
{
    private SortedDictionary<ulong, CBookEntry>? _entries;
    private SortedDictionary<ulong, CBookEntry>? _otherEntries1;
    private SortedDictionary<ulong, CBookEntry>? _otherEntries2;
    private CBoard? _board;
    private PgnExtract? _pgnExtract;

    private int _gameNb, _ply;
    private int _minGames, _maxPly;

    /// <summary>
    /// Creates a new CBook-File from a .extract File, where each position and move occurs a minimum number of times and the games are searched to a given depth
    /// </summary>
    /// <param name="file">The .extract file to read the games from</param>
    /// <param name="binFile">The file to save to</param>
    /// <param name="minGames">The minimum number of occurrences to keep a position / move</param>
    /// <param name="maxPly">The maximum ply to which a game will be searched</param>
    public void Make(string file, string binFile, int minGames, int maxPly)
    {
        _minGames = minGames;
        _maxPly = maxPly;
        Console.WriteLine("Inserting Games...");
        InsertFile(file);
        Console.WriteLine("Filtering Entries...");
        Filter();
        Console.WriteLine("Saving Entries...");
        Save(binFile);
        Console.WriteLine("Done!");
    }

    /// <summary>
    /// Merges two CBook-Files with a given weight for each entry and saves them into another file
    /// </summary>
    /// <param name="in1">The first CBook-File to merge</param>
    /// <param name="in2">The second CBook-File to merge</param>
    /// <param name="outFile">The file to save to</param>
    /// <param name="weight1">The weight for the entries of the first CBook</param>
    /// <param name="weight2">The weight for the entries of the second CBook</param>
    public void Merge(string in1, string in2, string outFile, int weight1, int weight2)
    {
        _minGames = 1;
        Console.WriteLine("Opening Entries...");
        Open(in1, 1);
        Open(in2, 2);
        _entries = new SortedDictionary<ulong, CBookEntry>();
        Console.WriteLine($"Merging {_otherEntries1!.Count + _otherEntries2!.Count} Entries...");
        Merge(weight1, weight2);
        Console.WriteLine($"Filtering {_entries!.Count} Entries...");
        Console.WriteLine("Saving Entries...");
        Save(outFile);
        Console.WriteLine("Done!");
    }

    /// <summary>
    /// Merges two already opened CBooks with a given weight for each entry by iterating over those
    /// </summary>
    /// <param name="weight1">The weight for the entries of the first CBook</param>
    /// <param name="weight2">The weight for the entries of the second CBook</param>
    private void Merge(int weight1, int weight2)
    {
        int pos1 = 0, pos2 = 0;

        ulong[] keys1 = _otherEntries1!.Keys.ToArray();
        ulong[] keys2 = _otherEntries2!.Keys.ToArray();

        while (true)
        {
            if (pos1 < keys1.Length && pos2 < keys2.Length)
            {
                ulong hash1 = keys1[pos1];
                ulong hash2 = keys2[pos2];

                if (hash1 > hash2)
                {
                    _entries!.Add(hash2, _otherEntries2[hash2]);
                    pos2++;
                }
                else if (hash2 > hash1)
                {
                    _entries!.Add(hash1, _otherEntries1[hash1]);
                    pos1++;
                }
                else if (hash1 == hash2)
                {
                    _entries!.Add(hash1,
                        CBookEntry.Merge(_otherEntries1[hash1], _otherEntries2[hash2], weight1, weight2));
                    pos1++;
                    pos2++;
                }
            }
            else if (pos1 >= keys1.Length && pos2 >= keys2.Length)
                break;
            else if (pos1 >= keys1.Length)
            {
                ulong hash2 = keys2[pos2];
                _entries!.Add(hash2, _otherEntries2[hash2]);
                pos2++;
            }
            else if (pos2 >= keys2.Length)
            {
                ulong hash1 = keys1[pos1];
                _entries!.Add(hash1, _otherEntries1[hash1]);
                pos1++;
            }

            if ((pos1 + pos2) % 10000 == 0) Console.WriteLine($"{pos1 + pos2} Entries!");
        }
    }

    /// <summary>
    /// Opens a CBook-File
    /// </summary>
    /// <param name="file">he CBook-File to open</param>
    /// <returns>A SortedDictionary containing all the CBookEntries in the file</returns>
    public SortedDictionary<ulong, CBookEntry> Open(string file)
    {
        Open(file, 0, false);
        return _entries!;
    }

    /// <summary>
    /// Opens a CBook-File
    /// </summary>
    /// <param name="file">The CBook-File to open</param>
    /// <param name="i">A parameter specifying to which Dictionary the entries will be saved</param>
    /// <param name="output">Whether to continuously output the number of entries opened, or not. Defaults to true</param>
    private void Open(string file, int i, bool output = true)
    {
        if (i == 1) _otherEntries1 = new SortedDictionary<ulong, CBookEntry>();
        else if (i == 2) _otherEntries2 = new SortedDictionary<ulong, CBookEntry>();
        else _entries = new SortedDictionary<ulong, CBookEntry>();
        byte[] bytes = System.IO.File.ReadAllBytes(file);
        for (int pos = 0; pos + 7 < bytes.Length;)
        {
            ulong hash = BitConverter.ToUInt64(bytes, pos);
            CBookEntry entry = new CBookEntry(hash);
            pos = entry.Deserialize(bytes, pos);
            if (i == 1)
            {
                if (!_otherEntries1!.ContainsKey(hash)) _otherEntries1.Add(hash, entry);
                if (output && _otherEntries1!.Count % 10000 == 0) Console.WriteLine($"{_otherEntries1!.Count} Entries...");
            }
            else if (i == 2)
            {
                if (!_otherEntries2!.ContainsKey(hash)) _otherEntries2.Add(hash, entry);
                if (output && _otherEntries2!.Count % 10000 == 0) Console.WriteLine($"{_otherEntries2!.Count} Entries...");
            }
            else
            {
                if (!_entries!.ContainsKey(hash)) _entries.Add(hash, entry);
                if (output && _entries!.Count % 10000 == 0) Console.WriteLine($"{_entries!.Count} Entries...");
            }
        }
    }

    /// <summary>
    /// Inserts a given file into this CBook object
    /// </summary>
    /// <param name="fileName">The .extract file to read the games from</param>
    private void InsertFile(string fileName)
    {
        _gameNb = 0;
        _pgnExtract = new PgnExtract(fileName);
        _entries = new SortedDictionary<ulong, CBookEntry>();

        while (_pgnExtract.NextGame())
        {
            _board = CBoard.StartingBoard();
            _ply = 0;
            while (_pgnExtract.NextMove())
            {
                if (_ply >= _maxPly) break;
                CBookMove move = NextMove();
                AddEntry(new SerializableMove(move));
                move.Move.Make(_board);
                _ply++;
            }

            _gameNb++;
            if (_gameNb % 10000 == 0) Console.WriteLine($"{_gameNb} Games...");
        }

        Console.WriteLine($"{_gameNb} Games, {_entries.Count} Entries.");
    }

    /// <summary>
    /// Gets the next possible move in the current .extract-file. If it is null, the current position will be reset and tried again. If it is null again, we will skip to the next game
    /// </summary>
    /// <param name="firstCall">Whether the method is called for the first time for this move</param>
    /// <returns>The next possible move</returns>
    private CBookMove NextMove(bool firstCall = true)
    {
        CBookMove? move = CBookMove.NewCBookMove(_pgnExtract!.MoveString, _board!,
            _pgnExtract.ColumnNumber, _pgnExtract.LineNumber);
        if (move != null) return move;
        if (!firstCall)
        {
            _pgnExtract.NextGame();
            _pgnExtract.NextMove();
        }

        _board = CBoard.StartingBoard();
        _ply = 0;
        return NextMove(false);
    }

    /// <summary>
    /// Filters all moves with less than <see cref="_minGames"/> occurrences and all position with the same condition 
    /// </summary>
    private void Filter()
    {
        int count = 0;
        List<ulong> remove = new List<ulong>();
        ulong[] keys = _entries!.Keys.ToArray();
        for (var index = 0; index < keys.Length; index++)
        {
            var u = keys[index];
            if (_entries[u].Filter(_minGames))
                remove.Add(u);
            count++;
            if (count % 100000 == 0) Console.WriteLine($"{count} Entries filtered...");
        }

        for (var index = 0; index < remove.Count; index++)
        {
            var u = remove[index];
            _entries.Remove(u);
        }

        Console.WriteLine($"{_entries.Count} Entries left...");
    }

    /// <summary>
    /// Saves the current <see cref="_entries"/> to a given file
    /// </summary>
    /// <param name="binFileName">The file to save to</param>
    private void Save(string binFileName)
    {
        int length = 0;
        CBookEntry[] entries = _entries!.Values.ToArray();
        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            length += 9 + 3 * entry.Index;
        }

        byte[] bytes = new byte[length];
        int index = 0;
        int count = 0;
        for (var i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            byte[] serialized = entry.Serialize();
            for (var j = 0; j < serialized.Length; j++)
            {
                bytes[index] = serialized[j];
                index++;
            }

            count++;
            if (count % 10000 == 0) Console.WriteLine($"{count} Entries serialized...");
        }

        Console.WriteLine($"{_entries.Count} Entries serialized...");
        Console.WriteLine("Writing To File...");
        System.IO.File.WriteAllBytes(binFileName, bytes);
    }

    /// <summary>
    /// Adds a move to <see cref="_entries"/>: If the position already exists, the move will be inserted with <see cref="CBookEntry.Insert"/>, otherwise, a new one will be created
    /// </summary>
    /// <param name="move">The move to add</param>
    private void AddEntry(SerializableMove move)
    {
        if (_entries!.ContainsKey(_board!.Zobrist.Hash)) _entries[_board.Zobrist.Hash].Insert(move);
        else
        {
            CBookEntry entry = new CBookEntry(_board.Zobrist.Hash);
            entry.Insert(move);
            _entries.Add(_board.Zobrist.Hash, entry);
        }
    }
}

/// <summary>
/// Represents a CBookEntry, containing the <see cref="Zobrist"/>-Hash of the position and the moves and counts stored in the position
/// </summary>
public class CBookEntry
{
    /// <summary>
    /// The <see cref="Zobrist"/>-Hash of the position this entry represents
    /// </summary>
    public ulong Hash { get; }

    /// <summary>
    /// The <see cref="SerializableMove"/>s stored in this position
    /// </summary>
    public SerializableMove?[] Moves { get; private set; }

    /// <summary>
    /// The number of occurrences of the corresponding moves
    /// </summary>
    public ushort[] Counts { get; private set; }

    /// <summary>
    /// The current index
    /// </summary>
    public byte Index { get; private set; }

    /// <summary>
    /// The total count of moves stored in this entry
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Creates a new, empty CBookEntry with a given <see cref="Zobrist"/>-Hash of the position
    /// </summary>
    /// <param name="hash">The <see cref="Zobrist"/>-Hash of the position</param>
    public CBookEntry(ulong hash)
    {
        Hash = hash;
        Moves = new SerializableMove[128];
        Counts = new ushort[128];
        Count = 0;
        Index = 0;
    }

    /// <summary>
    /// Adds a move with a given number of occurrences to this entry
    /// </summary>
    /// <param name="move">The move to add</param>
    /// <param name="count">The number of occurrences of the move</param>
    private void AddMoveAndCount(SerializableMove move, ushort count)
    {
        Moves[Index] = move;
        Counts[Index] = count;
        Count += count;
        Index++;
    }

    /// <summary>
    /// Merges two CBookEntries from the same position with given weights and returns a new one
    /// </summary>
    /// <param name="a">The first CBookEntry to merge</param>
    /// <param name="b">The second CBookEntry to merge</param>
    /// <param name="weight">The weight of the first CBookEntry</param>
    /// <param name="otherWeight">The weight of the first CBookEntry</param>
    /// <returns>The two CBookEntries merged</returns>
    /// <exception cref="ArgumentException">If the hashes of the two entries are not equal</exception>
    public static CBookEntry Merge(CBookEntry a, CBookEntry b, int weight, int otherWeight)
    {
        if (!a.Equals(b)) throw new ArgumentException();
        CBookEntry entry = new CBookEntry(a.Hash);
        for (var index = 0; index < a.Index; index++)
            entry.AddMoveAndCount(a.Moves[index]!, (ushort)(a.Counts[index] * weight));


        entry.Count = a.Count;
        for (int i = 0; i < b.Index; i++)
        {
            int pos = Array.IndexOf(a.Moves, b.Moves[i]);
            if (pos != -1)
                entry.Counts[pos] = (ushort)(a.Counts[pos] + b.Counts[i] * otherWeight);
            else
                entry.AddMoveAndCount(b.Moves[i]!, (ushort)(b.Counts[i] * otherWeight));
        }

        return entry;
    }

    /// <summary>
    /// Filters all the moves with less than a given number of occurrences
    /// </summary>
    /// <param name="minGames">The minimum number of occurrences for a move to have</param>
    /// <returns>Whether the total number of move counts is at least minGames</returns>
    public bool Filter(int minGames)
    {
        for (int i = 0; i < Index; i++)
        {
            if (Counts[i] >= minGames) continue;
            Moves[i] = null;
            Counts[i] = 0;
        }

        Sort();
        return Count < minGames;
    }

    /// <summary>
    /// Sorts the moves contained in this instance as follows: The order of the not null moves stays the same, all null entries are sorted to the back.
    /// The index and count will be set appropriately
    /// </summary>
    private void Sort()
    {
        SerializableMove?[] moves = new SerializableMove[128];
        ushort[] counts = new ushort[128];
        ushort myCount = 0;
        byte myIndex = 0;
        for (var index = 0; index < Moves.Length; index++)
        {
            var move = Moves[index];
            var count = Counts[index];
            if (move == null) continue;
            moves[myIndex] = move;
            counts[myIndex] = count;
            myCount += count;
            myIndex++;
        }

        Moves = moves;
        Counts = counts;
        Count = myCount;
        Index = myIndex;
    }
    
    /// <summary>
    /// Sums all the counts in <see cref="Counts"/> and saves them to <see cref="Count"/>
    /// </summary>
    private void CalculateCount()
    {
        Count = 0;
        for (var index = 0; index < Index; index++)
        {
            if (Moves[index] == null) break;
            Count += Counts[index];
        }
    }

    /// <summary>
    /// Inserts a <see cref="SerializableMove"/> into this entry: If it exists, the count of the move will be increased, else the move will be added
    /// </summary>
    /// <param name="move"></param>
    public void Insert(SerializableMove move)
    {
        int pos = Array.IndexOf(Moves, move);
        if (pos == -1) AddMoveAndCount(move, 1);
        else Counts[pos]++;
        Count++;
    }

    /// <summary>
    /// Deserializes a CBookEntry from a given array of bytes and a starting index
    /// </summary>
    /// <param name="bytes">The bytes to deserialize from</param>
    /// <param name="pos">The starting index</param>
    /// <returns>The new index to continue deserializing</returns>
    public int Deserialize(byte[] bytes, int pos)
    {
        byte length = bytes[pos + 8];
        for (int i = 0; i < length; i++)
        {
            byte b1 = bytes[pos + 9 + i * 3];
            byte b2 = bytes[pos + 9 + i * 3 + 1];
            byte b3 = bytes[pos + 9 + i * 3 + 2];
            ushort count = (ushort)(((b2 & 0xf) << 8) | b3);
            AddMoveAndCount(new SerializableMove(b1, b2), count);
        }

        return pos + 9 + length * 3;
    }

    /// <summary>
    /// Serializes this CBookEntry to a byte array in the following form:
    /// 8 bytes <see cref="Hash"/>, 1 byte for the number of moves to follow, 3 bytes per <see cref="SerializableMove"/>
    /// </summary>
    /// <returns>The serialized CBookEntry</returns>
    public byte[] Serialize()
    {
        CalculateCount();
        byte[] output = new byte[9 + Index * 3];
        byte myIndex = 0;
        byte[] hashBytes = BitConverter.GetBytes(Hash);
        for (var index = 0; index < hashBytes.Length; index++)
        {
            var b = hashBytes[index];
            output[myIndex] = b;
            myIndex++;
        }

        output[myIndex] = Index;
        myIndex++;
        for (int i = 0; i < Index; i++)
        {
            byte b1 = Moves[i]!.SerializeFirstByte();
            byte b2 = Moves[i]!.SerializeSecondByte();
            ushort count = (ushort)(Counts[i] * 2048 / Count);
            b2 = (byte)(b2 | (count & 0xf00) >> 8);
            byte b3 = (byte)(count & 0xff);
            output[myIndex + 3 * i] = b1;
            output[myIndex + 3 * i + 1] = b2;
            output[myIndex + 3 * i + 2] = b3;
        }

        return output;
    }
    
    /// <summary>
    /// Selects a random <see cref="CMove"/> from this Entry where each move has a weight of the corresponding count in <see cref="Counts"/>
    /// </summary>
    /// <param name="board">The current state of the <see cref="CBoard"/></param>
    /// <returns>A randomly selected CMove</returns>
    public CMove? RandomMove(CBoard board)
    {
        int gesCount = 0;
        for (var index = 0; index < Counts.Length; index++)
        {
            var u = Counts[index];
            if (u == 0) break;
            gesCount += u;
        }

        SerializableMove? serializableMove = null;
        int pos = new Random().Next(gesCount);
        gesCount = 0;
        for (int i = 0; i < Counts.Length; i++)
        {
            gesCount += Counts[i];
            if (gesCount < pos) continue;
            serializableMove = Moves[i];
            break;
        }

        if (serializableMove == null) return null;
        CMove move = new CMove(serializableMove.From, serializableMove.To, board);
        return move;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Hash == ((CBookEntry)obj).Hash;
    }

    public override int GetHashCode() => Hash.GetHashCode();
}