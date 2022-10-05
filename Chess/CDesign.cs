using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace Chess;

public class CDesign
{
    private readonly int BoardSize;
    private readonly MainWindow Window;

    public CDesign(int boardSize, MainWindow window)
    {
        BoardSize = boardSize;
        Window = window;
    }
    
}