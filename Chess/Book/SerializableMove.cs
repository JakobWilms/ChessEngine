using System.Collections.Generic;

namespace Chess.Book;

public class SerializableMove : IEqualityComparer<SerializableMove>
{
    public byte From { get; }
    public byte To { get; }

    public SerializableMove(CBookMove bookMove)
    {
        From = bookMove.Move.GetFrom();
        To = bookMove.Move.GetTo();
    }

    public SerializableMove(byte b1, byte b2)
    {
        From = (byte)((b1 & 0xfc) >> 2);
        To = (byte)(((b1 & 0x3) << 4) | ((b2 & 0xf0) >> 4));
    }

    public byte SerializeFirstByte() => (byte)((From << 2) | (To >> 4));

    public byte SerializeSecondByte() => (byte)((To & 0xf) << 4);

    public override bool Equals(object? obj) => obj is SerializableMove move && From == move.From && To == move.To;

    public override int GetHashCode() => HashCode.Combine(From, To);

    public bool Equals(SerializableMove? x, SerializableMove? y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.From == y.From && x.To == y.To;
    }

    public int GetHashCode(SerializableMove obj) => HashCode.Combine(obj.From, obj.To);
}