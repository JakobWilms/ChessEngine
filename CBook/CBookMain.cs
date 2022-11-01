using Chess;

namespace CBook;

public static class CBookMain
{
    public static void Main(string[] args)
    {
        CAttackMap.Calculate();
        
        string prefix = "/home/jakob/RiderProjects/Chess/openingdb/";
        //PgnFile.Extract(prefix + "b-filter.pgn", prefix + "black.extract");
        var book = new Chess.Book.CBook();
        book.Make(prefix + "white.extract", prefix + "white-tmp1.cbook", 50, 28);
        book.Make(prefix + "black.extract", prefix + "black-tmp1.cbook", 50, 28);
        book.Make(prefix + "white.extract", prefix + "white-tmp2.cbook", 5, 24);
        book.Make(prefix + "black.extract", prefix + "black-tmp2.cbook", 5, 24);
        book.Merge(prefix + "white-tmp1.cbook", prefix + "white-tmp2.cbook", prefix + "book-tmp1.cbook", 10, 1);
        book.Merge(prefix + "black-tmp1.cbook", prefix + "black-tmp2.cbook", prefix + "book-tmp2.cbook", 10, 1);
        book.Merge(prefix + "book-tmp1.cbook", prefix + "book-tmp2.cbook", prefix + "book.cbook", 1, 1);
        
        //CBookEntry[] entries = book.Open(prefix + "test.cbook").Values.ToArray();
        //book.Make(prefix + "test.extract", prefix + "test.cbook", 1, 2);
    }
}