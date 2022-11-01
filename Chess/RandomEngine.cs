using Chess.Book;

namespace Chess;

public class RandomEngine : Engine
{
    protected override CMove? FindMove(CBookEntry? entry = null) => FromMoveArray(CMoveGeneration.MoveGen(CDisplay.Instance.Board));
}