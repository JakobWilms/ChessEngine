using System;
using System.Text;
using System.Text.RegularExpressions;
using static Chess.PieceType;

namespace Chess;

public static class FenReader
{
    public const string StartingFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

    public static CBoard ImportFen(string fen)
    {
        Regex digit = new Regex("[0-8]");
        Regex character = new Regex("[a-zA-Z]");

        CBoard board = new CBoard();
        string[] split = fen.Split(' ');

        int stringPos = 0;
        for (int r8 = 56; r8 >= 0; r8 -= 8)
        {
            for (int f = 0; f < 8; f++, stringPos++)
            {
                if (split[0][stringPos] == '/')
                    stringPos++;
                if (character.IsMatch(split[0][stringPos].ToString()))
                {
                    //Console.WriteLine($"{r8 + f} {stringPos} {PieceTypeFromFenChar(split[0][stringPos])}");
                    board.SetSquare(PieceTypeFromFenChar(split[0][stringPos]), r8 + f);
                }
                else if (digit.IsMatch(split[0][stringPos].ToString()))
                    f += byte.Parse(split[0][stringPos].ToString()) - 1;
            }
        }

        board.ToMove = split[1][0] == 'w' ? ColorType.White : ColorType.Black;
        board.SetWhiteCastleKing(split[2].Contains('K'));
        board.SetBlackCastleKing(split[2].Contains('k'));

        board.SetWhiteCastleQueen(split[2].Contains('Q'));
        board.SetBlackCastleQueen(split[2].Contains('q'));

        if (split[3].Equals("-"))
            board.SetEnPassantTargetSquare(CSquare.A1);
        else if (Enum.TryParse(split[3].ToUpper(), out CSquare ep))
            board.SetEnPassantTargetSquare(ep);
        else
            throw new ArgumentOutOfRangeException(split[3]);

        board.SetHalfMoves(byte.Parse(split[4]));
        board.FullMoves = ushort.Parse(split[5]);

        return board;
    }

    public static string ExportFen(CBoard board)
    {
        StringBuilder builder = new StringBuilder();
        for (int r8 = 56; r8 >= 0; r8 -= 8)
        {
            int empty = 0;
            for (int f = 0; f < 8; f++)
            {
                PieceType pieceType = board.GetPieceType((byte)(r8 + f));
                if (pieceType == Empty) empty++;
                else
                {
                    if (empty != 0) builder.Append(empty);
                    empty = 0;
                    builder.Append(FenCharFromPieceType(pieceType));
                }
            }

            if (empty != 0) builder.Append(empty);
            if (r8 != 0) builder.Append('/');
        }

        builder.Append(' ').Append(board.ToMove == ColorType.White ? 'w' : 'b').Append(' ');
        bool wck = board.GetWhiteCastleKing(),
            wcq = board.GetWhiteCastleQueen(),
            bck = board.GetBlackCastleKing(),
            bcq = board.GetBlackCastleQueen();
        if (!wck && !wcq && !bck && !bcq) builder.Append('-');
        else
            builder.Append(wck ? 'K' : "")
                .Append(bck ? 'k' : "")
                .Append(wcq ? 'Q' : "")
                .Append(bcq ? 'q' : "");
        builder.Append(' ').Append(board.GetEp() == CSquare.A1 ? '-' : board.GetEp().ToString());
        builder.Append(' ').Append(board.GetHalfMoves()).Append(' ').Append(board.FullMoves);

        return builder.ToString();
    }

    private static PieceType PieceTypeFromFenChar(char c) =>
        c switch
        {
            'P' => WhitePawn,
            'p' => BlackPawn,
            'N' => WhiteKnight,
            'n' => BlackKnight,
            'B' => WhiteBishop,
            'b' => BlackBishop,
            'R' => WhiteRook,
            'r' => BlackRook,
            'Q' => WhiteQueen,
            'q' => BlackQueen,
            'K' => WhiteKing,
            'k' => BlackKing,
            _ => throw new ArgumentOutOfRangeException()
        };

    private static char FenCharFromPieceType(PieceType pieceType) =>
        pieceType switch
        {
            WhitePawn => 'P',
            BlackPawn => 'p',
            WhiteKnight => 'N',
            BlackKnight => 'n',
            WhiteBishop => 'B',
            BlackBishop => 'b',
            WhiteRook => 'R',
            BlackRook => 'r',
            WhiteQueen => 'Q',
            BlackQueen => 'q',
            WhiteKing => 'K',
            BlackKing => 'k',
            _ => throw new ArgumentOutOfRangeException()
        };
}