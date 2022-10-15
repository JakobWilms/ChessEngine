using System.Collections.Generic;
using Avalonia.Media.Imaging;
using static Chess.PieceType;

namespace Chess;

public class CResources
{
    public Dictionary<PieceType, Dictionary<bool, Dictionary<bool, Bitmap>>> Textures { get; }
    public Bitmap PossibleMove { get; }
    public Bitmap DarkSquare { get; }
    public Bitmap LightSquare { get; }

    public CResources(string path)
    {
        Textures = new Dictionary<PieceType, Dictionary<bool, Dictionary<bool, Bitmap>>>(); // PieceType, Dark=False/Light=True, Possible
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
        {
            Textures.Add(pieceType, new Dictionary<bool, Dictionary<bool, Bitmap>>());
            Textures[pieceType].Add(true, new Dictionary<bool, Bitmap>());
            Textures[pieceType].Add(false, new Dictionary<bool, Bitmap>());
            Textures[pieceType][true].Add(false, new Bitmap($"{path}{pieceType.ToString()}_Light.bmp"));
            Textures[pieceType][false].Add(false, new Bitmap($"{path}{pieceType.ToString()}_Dark.bmp"));
            Textures[pieceType][true].Add(true, new Bitmap($"{path}{pieceType.ToString()}_Light_Possible.bmp"));
            Textures[pieceType][false].Add(true, new Bitmap($"{path}{pieceType.ToString()}_Dark_Possible.bmp"));
        }

        PossibleMove = new Bitmap($"{path}PossibleMove.bmp");
        DarkSquare = new Bitmap($"{path}DarkSquare.bmp");
        LightSquare = new Bitmap($"{path}LightSquare.bmp");
    }

    public Bitmap GetTexture(PieceType type, bool lightSquare, bool possible) => Textures[type][lightSquare][possible];
}