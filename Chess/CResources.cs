using System.Collections.Generic;
using Avalonia.Media.Imaging;
using static Chess.PieceType;

namespace Chess;

public class CResources
{
    public Dictionary<PieceType, Bitmap> Textures { get; }
    public Bitmap PossibleMove { get; }

    public CResources(string path)
    {
        Textures = new Dictionary<PieceType, Bitmap>();
        PieceType[] types =
        {
            WhitePawn, BlackPawn,
            WhiteKnight, BlackKnight,
            WhiteBishop, BlackBishop,
            WhiteRook, BlackRook,
            WhiteQueen, BlackQueen, 
            WhiteKing, BlackKing, 
            Empty
        };
        foreach (var pieceType in types)
            Textures.Add(pieceType, new Bitmap($"{path}{pieceType.ToString()}.bmp"));
        PossibleMove = new Bitmap($"{path}PossibleMove.bmp");
    }
}