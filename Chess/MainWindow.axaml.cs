using Avalonia.Controls;

namespace Chess
{
    public partial class MainWindow : Window
    {
        public static string[] Args = null!;

        public MainWindow()
        {
            InitializeComponent();

            CAttackMap.Calculate();
            CBoard board = FenReader.ImportFen(FenReader.StartingFen);
            Engine.ReadEngines(Args);
            CDisplay.Instance = new CDisplay(this, Args[0], board);
            CDisplay.Instance.Display();
        }
        
    }
}