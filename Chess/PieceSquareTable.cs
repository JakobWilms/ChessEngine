namespace Chess;

public static class PieceSquareTable
{
    public static readonly int[][][]
        PieceSquareTables = // Middle/Endgame, W,BPawn/W,BKnight/W,BBishop/W,BRook/W,BQueen/W,BKing, Square
        {
            new[] // MiddleGame
            {
                new[] // WPawn M
                {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    5, 10, 10, -20, -20, 10, 10, 5,
                    5, -5, -10, 0, 0, -10, -5, 5,
                    0, 0, 0, 20, 20, 0, 0, 0,
                    5, 5, 10, 25, 25, 10, 5, 5,
                    10, 10, 20, 30, 30, 20, 10, 10,
                    50, 50, 50, 50, 50, 50, 50, 50,
                    0, 0, 0, 0, 0, 0, 0, 0,
                },
                new[] // BPawn M
                {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    50, 50, 50, 50, 50, 50, 50, 50,
                    10, 10, 20, 30, 30, 20, 10, 10,
                    5, 5, 10, 25, 25, 10, 5, 5,
                    0, 0, 0, 20, 20, 0, 0, 0,
                    5, -5, -10, 0, 0, -10, -5, 5,
                    5, 10, 10, -20, -20, 10, 10, 5,
                    0, 0, 0, 0, 0, 0, 0, 0
                },
                new[] // WKnight M
                {
                    -50, -40, -30, -30, -30, -30, -40, -50,
                    -40, -20, 0, 5, 5, 0, -20, -40,
                    -30, 5, 10, 15, 15, 10, 5, -30,
                    -30, 0, 15, 20, 20, 15, 0, -30,
                    -30, 5, 15, 20, 20, 15, 5, -30,
                    -30, 0, 10, 15, 15, 10, 0, -30,
                    -40, -20, 0, 0, 0, 0, -20, -40,
                    -50, -40, -30, -30, -30, -30, -40, -50,
                },
                new[] // BKnight M
                {
                    -50, -40, -30, -30, -30, -30, -40, -50,
                    -40, -20, 0, 0, 0, 0, -20, -40,
                    -30, 0, 10, 15, 15, 10, 0, -30,
                    -30, 5, 15, 20, 20, 15, 5, -30,
                    -30, 0, 15, 20, 20, 15, 0, -30,
                    -30, 5, 10, 15, 15, 10, 5, -30,
                    -40, -20, 0, 5, 5, 0, -20, -40,
                    -50, -40, -30, -30, -30, -30, -40, -50,
                },
                new[] // WBishop M
                {
                    -20, -10, -10, -10, -10, -10, -10, -20,
                    -10, 5, 0, 0, 0, 0, 5, -10,
                    -10, 10, 10, 10, 10, 10, 10, -10,
                    -10, 0, 10, 10, 10, 10, 0, -10,
                    -10, 5, 5, 10, 10, 5, 5, -10,
                    -10, 0, 5, 10, 10, 5, 0, -10,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -20, -10, -10, -10, -10, -10, -10, -20,
                },
                new[] // BBishop M
                {
                    -20, -10, -10, -10, -10, -10, -10, -20,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -10, 0, 5, 10, 10, 5, 0, -10,
                    -10, 5, 5, 10, 10, 5, 5, -10,
                    -10, 0, 10, 10, 10, 10, 0, -10,
                    -10, 10, 10, 10, 10, 10, 10, -10,
                    -10, 5, 0, 0, 0, 0, 5, -10,
                    -20, -10, -10, -10, -10, -10, -10, -20,
                },
                new[] // WRook M
                {
                    0, 0, 0, 5, 5, 0, 0, 0,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    5, 10, 10, 10, 10, 10, 10, 5,
                    0, 0, 0, 0, 0, 0, 0, 0,
                },
                new[] // BRook M
                {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    5, 10, 10, 10, 10, 10, 10, 5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    0, 0, 0, 5, 5, 0, 0, 0
                },
                new[] // WQueen M
                {
                    -20, -10, -10, -5, -5, -10, -10, -20,
                    -10, 0, 5, 0, 0, 0, 0, -10,
                    -10, 5, 5, 5, 5, 5, 0, -10,
                    0, 0, 5, 5, 5, 5, 0, -5,
                    -5, 0, 5, 5, 5, 5, 0, -5,
                    -10, 0, 5, 5, 5, 5, 0, -10,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -20, -10, -10, -5, -5, -10, -10, -20,
                },
                new[] // BQueen M
                {
                    -20, -10, -10, -5, -5, -10, -10, -20,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -10, 0, 5, 5, 5, 5, 0, -10,
                    -5, 0, 5, 5, 5, 5, 0, -5,
                    0, 0, 5, 5, 5, 5, 0, -5,
                    -10, 5, 5, 5, 5, 5, 0, -10,
                    -10, 0, 5, 0, 0, 0, 0, -10,
                    -20, -10, -10, -5, -5, -10, -10, -20
                },
                new[] // WKing M
                {
                    20, 30, 10, 0, 0, 10, 30, 20,
                    20, 20, 0, 0, 0, 0, 20, 20,
                    -10, -20, -20, -20, -20, -20, -20, -10,
                    -20, -30, -30, -40, -40, -30, -30, -20,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                },
                new[] // BKing M
                {
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -20, -30, -30, -40, -40, -30, -30, -20,
                    -10, -20, -20, -20, -20, -20, -20, -10,
                    20, 20, 0, 0, 0, 0, 20, 20,
                    20, 30, 10, 0, 0, 10, 30, 20
                }
            },

            new[] // Endgame
            {
                new[] // WPawn E
                {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    50, 50, 50, 50, 50, 50, 50, 50,
                    10, 10, 20, 30, 30, 20, 10, 10,
                    5, 5, 10, 25, 25, 10, 5, 5,
                    0, 0, 0, 20, 20, 0, 0, 0,
                    5, -5, -10, 0, 0, -10, -5, 5,
                    5, 10, 10, -20, -20, 10, 10, 5,
                    0, 0, 0, 0, 0, 0, 0, 0
                },
                new[] // BPawn E
                {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    5, 10, 10, -20, -20, 10, 10, 5,
                    5, -5, -10, 0, 0, -10, -5, 5,
                    0, 0, 0, 20, 20, 0, 0, 0,
                    5, 5, 10, 25, 25, 10, 5, 5,
                    10, 10, 20, 30, 30, 20, 10, 10,
                    50, 50, 50, 50, 50, 50, 50, 50,
                    0, 0, 0, 0, 0, 0, 0, 0,
                },
                new[] // WKnight E
                {
                    -50, -40, -30, -30, -30, -30, -40, -50,
                    -40, -20, 0, 0, 0, 0, -20, -40,
                    -30, 0, 10, 15, 15, 10, 0, -30,
                    -30, 5, 15, 20, 20, 15, 5, -30,
                    -30, 0, 15, 20, 20, 15, 0, -30,
                    -30, 5, 10, 15, 15, 10, 5, -30,
                    -40, -20, 0, 5, 5, 0, -20, -40,
                    -50, -40, -30, -30, -30, -30, -40, -50,
                },
                new[] // BKnight E
                {
                    -50, -40, -30, -30, -30, -30, -40, -50,
                    -40, -20, 0, 5, 5, 0, -20, -40,
                    -30, 5, 10, 15, 15, 10, 5, -30,
                    -30, 0, 15, 20, 20, 15, 0, -30,
                    -30, 5, 15, 20, 20, 15, 5, -30,
                    -30, 0, 10, 15, 15, 10, 0, -30,
                    -40, -20, 0, 0, 0, 0, -20, -40,
                    -50, -40, -30, -30, -30, -30, -40, -50,
                },
                new[] // WBishop E
                {
                    -20, -10, -10, -10, -10, -10, -10, -20,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -10, 0, 5, 10, 10, 5, 0, -10,
                    -10, 5, 5, 10, 10, 5, 5, -10,
                    -10, 0, 10, 10, 10, 10, 0, -10,
                    -10, 10, 10, 10, 10, 10, 10, -10,
                    -10, 5, 0, 0, 0, 0, 5, -10,
                    -20, -10, -10, -10, -10, -10, -10, -20,
                },
                new[] // BBishop E
                {
                    -20, -10, -10, -10, -10, -10, -10, -20,
                    -10, 5, 0, 0, 0, 0, 5, -10,
                    -10, 10, 10, 10, 10, 10, 10, -10,
                    -10, 0, 10, 10, 10, 10, 0, -10,
                    -10, 5, 5, 10, 10, 5, 5, -10,
                    -10, 0, 5, 10, 10, 5, 0, -10,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -20, -10, -10, -10, -10, -10, -10, -20,
                },
                new[] // WRook E
                {
                    0, 0, 0, 0, 0, 0, 0, 0,
                    5, 10, 10, 10, 10, 10, 10, 5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    0, 0, 0, 5, 5, 0, 0, 0
                },
                new[] // BRook E
                {
                    0, 0, 0, 5, 5, 0, 0, 0,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    -5, 0, 0, 0, 0, 0, 0, -5,
                    5, 10, 10, 10, 10, 10, 10, 5,
                    0, 0, 0, 0, 0, 0, 0, 0,
                },
                new[] // WQueen E
                {
                    -20, -10, -10, -5, -5, -10, -10, -20,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -10, 0, 5, 5, 5, 5, 0, -10,
                    -5, 0, 5, 5, 5, 5, 0, -5,
                    0, 0, 5, 5, 5, 5, 0, -5,
                    -10, 5, 5, 5, 5, 5, 0, -10,
                    -10, 0, 5, 0, 0, 0, 0, -10,
                    -20, -10, -10, -5, -5, -10, -10, -20
                },
                new[] // BQueen E
                {
                    -20, -10, -10, -5, -5, -10, -10, -20,
                    -10, 0, 5, 0, 0, 0, 0, -10,
                    -10, 5, 5, 5, 5, 5, 0, -10,
                    0, 0, 5, 5, 5, 5, 0, -5,
                    -5, 0, 5, 5, 5, 5, 0, -5,
                    -10, 0, 5, 5, 5, 5, 0, -10,
                    -10, 0, 0, 0, 0, 0, 0, -10,
                    -20, -10, -10, -5, -5, -10, -10, -20,
                },
                new[] // WKing E
                {
                    -50, -40, -30, -20, -20, -30, -40, -50,
                    -30, -20, -10, 0, 0, -10, -20, -30,
                    -30, -10, 20, 30, 30, 20, -10, -30,
                    -30, -10, 30, 40, 40, 30, -10, -30,
                    -30, -10, 30, 40, 40, 30, -10, -30,
                    -30, -10, 20, 30, 30, 20, -10, -30,
                    -30, -30, 0, 0, 0, 0, -30, -30,
                    -50, -30, -30, -30, -30, -30, -30, -50
                },
                new[] // BKing E
                {
                    -50, -30, -30, -30, -30, -30, -30, -50,
                    -30, -30, 0, 0, 0, 0, -30, -30,
                    -30, -10, 20, 30, 30, 20, -10, -30,
                    -30, -10, 30, 40, 40, 30, -10, -30,
                    -30, -10, 30, 40, 40, 30, -10, -30,
                    -30, -10, 20, 30, 30, 20, -10, -30,
                    -30, -20, -10, 0, 0, -10, -20, -30,
                    -50, -40, -30, -20, -20, -30, -40, -50,
                }
            }
        };
}