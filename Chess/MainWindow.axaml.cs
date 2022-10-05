using System;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Chess
{
    public partial class MainWindow : Window
    {
        public static string[] Args = null!;

        public MainWindow()
        {
            InitializeComponent();

            int boardSize = Screens.Primary.Bounds.Height * 5 / 7;
            boardSize -= boardSize % 8;


            var cDesign = new CDesign(boardSize, this);

            CAttackMap.Calculate();
            CBoard board = FenReader.ImportFen(FenReader.StartingFen);
            Engine.ReadEngines(Args);
            CDisplay.Instance = new CDisplay(this, boardSize, Args[0], board);
            CDisplay.Instance.Display();
        }
        
    }
}