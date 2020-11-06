using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StringSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int dimension = 26;
        string[] letters = new string[dimension] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        string[] wordList = new string[dimension * dimension * dimension * dimension];
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnGenerateRandomWordList_Click(object sender, RoutedEventArgs e)
        {
            WordList.Create(wordList, letters, dimension);
            WordList.Shuffle(wordList);
            WordList.Display(wordList, LbRandomWordList);
        }

        private void LbRandomWordList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
