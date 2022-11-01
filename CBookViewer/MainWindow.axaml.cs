using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Chess;
using Chess.Book;

namespace CBookViewer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            CAttackMap.Calculate();
            OpenFileButton.Click += OnFileOpen;
        }

        private void OnFileOpen(object? sender, RoutedEventArgs routedEventArgs)
        {
            CBook book = new CBook();
            SortedDictionary<ulong, CBookEntry> dictionary = book.Open(FileNameBox.Text);
            CBookEntry[] entries = dictionary.Values.ToArray();
            Expander[] expanders = new Expander[entries.Length];
            for (var index = 0; index < entries.Length; index++)
            {
                var entry = entries[index];
                string[] content = new string[entry.Index];
                for (var i = 0; i < entry.Index; i++)
                {
                    var move = entry.Moves[i];
                    var count = entry.Counts[i];
                    content[i] = $"{(CSquare)move!.From}{(CSquare)move.To}: {count}";
                }

                ListBox listBox = new ListBox
                {
                    Items = content
                };

                ScrollViewer scrollViewer = new ScrollViewer
                {
                    Content = listBox
                };

                Expander expander = new Expander
                {
                    Header = $"Hash: {entry.Hash}, Move Count: {entry.Index}",
                    ExpandDirection = ExpandDirection.Down, IsExpanded = false,
                    Content = scrollViewer
                };
                expanders[index] = expander;
            }

            Repeater.Items = expanders;
        }
    }
}