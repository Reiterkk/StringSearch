using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly string[] letters = new string[dimension] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        private readonly string[] wordList = new string[dimension * dimension * dimension * dimension];
        private string[] sortedWordList;
        private readonly Stopwatch timer = new Stopwatch();
        private string searchedString = "";
        private bool sortedListIsNotAvailable = true;
        List<string> matchingWords = new List<string>();
        List<List<string>> IncrementalMatchingWords = new List<List<string>>();
        private int lastStringLength = 0;
        private int lastListCount = 1;

        public int MaxAllowedThreads { get; set; } = 1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TbSearchString_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchedString = TbSearchString.Text;
        }

        private async void BtnGenerateRandomWordList_Click(object sender, RoutedEventArgs e)
        {
            timer.Reset();
            timer.Start();
            Task t = Task.Run(() =>
            {
                WordList.Create(wordList, letters, dimension);
                WordList.Shuffle(wordList);
                IncrementalMatchingWords.Add(wordList.ToList());
            });
            await Task.WhenAll(t);
            timer.Stop();
            LblCreateListTime.Content = timer.ElapsedMilliseconds + " ms";

            timer.Reset();
            timer.Start();
            WordList.Display(wordList, LbRandomWordList);
            timer.Stop();
            LblUpdateUiCreateListTime.Content = timer.ElapsedMilliseconds + " ms";
            LblListboxItemCount.Content = LbRandomWordList.Items.Count;
        }

        private void BtnSerialLinearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (WordList.IsAvailable())
            {
                matchingWords.Clear();
                timer.Reset();
                timer.Start();
                WordList.SerialLinearSearch(wordList, searchedString, matchingWords);
                timer.Stop();
                LblSerialLinearSearchTime.Content = timer.ElapsedMilliseconds + " ms";

                timer.Reset();
                timer.Start();
                WordList.DisplayList(matchingWords, LbSerialLinearSearchResults);
                timer.Stop();
                LblSerialLinearSearchUITime.Content = timer.ElapsedMilliseconds + " ms";
                LblSerialLinearSearchWordsCount.Content = LbSerialLinearSearchResults.Items.Count;
            }
        }

        private void BtnParallelLinearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (WordList.IsAvailable())
            {
                matchingWords.Clear();
                timer.Reset();
                timer.Start();
                WordList.ParallelLinearSearch(wordList, searchedString, matchingWords, MaxAllowedThreads);
                timer.Stop();
                LblParallelLinearSearchTime.Content = timer.ElapsedMilliseconds + " ms";

                timer.Reset();
                timer.Start();
                WordList.DisplayList(matchingWords, LbParallelLinearSearchResults);
                timer.Stop();
                LblParallelLinearSearchUITime.Content = timer.ElapsedMilliseconds + " ms";
                LblParallelLinearSearchWordsCount.Content = LbParallelLinearSearchResults.Items.Count + "\nThreads: " + MaxAllowedThreads;
            }
        }

        private void BtnSerialBinarySearch_Click(object sender, RoutedEventArgs e)
        {
            if(WordList.IsAvailable())
            {
                if (sortedListIsNotAvailable)
                {
                    sortedWordList = (string[])wordList.Clone();
                    timer.Reset();
                    timer.Start();
                    //Array.Sort(sortedWordList);
                    sortedWordList = sortedWordList.AsParallel().WithDegreeOfParallelism(MaxAllowedThreads).OrderBy(t => t).ToArray();
                    timer.Stop();
                    LblSerialBinarySearchSortListTime.Content = timer.ElapsedMilliseconds + " ms";
                    WordList.Display(sortedWordList, LbRandomWordList);
                    sortedListIsNotAvailable = false;
                }

                List<string> matchingWords = new List<string>();
                timer.Reset();
                timer.Start();
                WordList.SerialBinarySearch(sortedWordList, searchedString, matchingWords);
                timer.Stop();
                LblSerialBinarySearchTime.Content = timer.ElapsedMilliseconds + " ms";

                timer.Reset();
                timer.Start();
                WordList.DisplayList(matchingWords, LbSerialBinarySearchResults);
                timer.Stop();
                LblSerialBinarySearchUITime.Content = timer.ElapsedMilliseconds + " ms";
                LblSerialBinarySearchWordsCount.Content = LbSerialBinarySearchResults.Items.Count;

            }
        }

        private void RBtn1_Checked(object sender, RoutedEventArgs e)
        {
            MaxAllowedThreads = 1;
        }

        private void RBtn2_Checked(object sender, RoutedEventArgs e)
        {
            if(Environment.ProcessorCount < 2)
            {
                NotifyUserAboutMaxThreadsAvailable();
                RBtn2.IsChecked = false;
                RBtnMax.IsChecked = true;
            }
            MaxAllowedThreads = Math.Min(2, Environment.ProcessorCount);
        }

        private void RBtn4_Checked(object sender, RoutedEventArgs e)
        {
            if (Environment.ProcessorCount < 4)
            {
                NotifyUserAboutMaxThreadsAvailable();
                RBtn4.IsChecked = false;
                RBtnMax.IsChecked = true;
            }
            MaxAllowedThreads = Math.Min(4, Environment.ProcessorCount);
        }

        private void RBtn8_Checked(object sender, RoutedEventArgs e)
        {
            if (Environment.ProcessorCount < 8)
            {
                NotifyUserAboutMaxThreadsAvailable();
                RBtn8.IsChecked = false;
                RBtnMax.IsChecked = true;
            }
            MaxAllowedThreads = Math.Min(8, Environment.ProcessorCount);
        }

        private void RBtnMax_Checked(object sender, RoutedEventArgs e)
        {
            MaxAllowedThreads = Environment.ProcessorCount;
        }

        private void NotifyUserAboutMaxThreadsAvailable()
        {
            string title = "Zu viele Threads für dieses System";
            string message = "Auf diesem System stehen nur " + Environment.ProcessorCount + " Threads zur Verfügung! Die maximale Anzahl der für die Operation erlaubten Threads wurde auf " + Environment.ProcessorCount + " gesetzt.";
            MessageBox.Show(message, title);
        }

        private void TbIncrementalSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (lastStringLength > TbIncrementalSearch.Text.Length && TbIncrementalSearch.Text.Length > 0)
            {
                while (IncrementalMatchingWords.Count > TbIncrementalSearch.Text.Length)
                {
                    IncrementalMatchingWords.RemoveAt(IncrementalMatchingWords.Count - 1);
                }
                while (IncrementalMatchingWords.Count >= lastListCount && IncrementalMatchingWords.Count > 1)
                {
                    IncrementalMatchingWords.RemoveAt(IncrementalMatchingWords.Count - 1);
                }
            } 
            else if (lastStringLength > TbIncrementalSearch.Text.Length && TbIncrementalSearch.Text.Length == 0)
            {
                while (IncrementalMatchingWords.Count > 1)
                {
                    IncrementalMatchingWords.RemoveAt(IncrementalMatchingWords.Count - 1);
                }
            }
            lastStringLength = TbIncrementalSearch.Text.Length;
            lastListCount = IncrementalMatchingWords.Count;

            if (TbIncrementalSearch.Text.Length > 0)
            {
                if (WordList.IsAvailable())
                {
                    LbIncrementalSearchResults.Visibility = Visibility.Visible;

                    matchingWords.Clear();
                    TbSearchString.Text = TbIncrementalSearch.Text;
                    timer.Reset();
                    timer.Start();
                    IncrementalMatchingWords.Add(WordList.SerialLinearListSearch(IncrementalMatchingWords[IncrementalMatchingWords.Count - 1], searchedString));
                    timer.Stop();
                    LblIncrementalSearchTime.Content = timer.ElapsedMilliseconds + " ms";

                    timer.Reset();
                    timer.Start();
                    WordList.DisplayList(IncrementalMatchingWords[IncrementalMatchingWords.Count - 1], LbIncrementalSearchResults);
                    timer.Stop();
                    LblIncrementalSearchUITime.Content = timer.ElapsedMilliseconds + " ms";
                    LblIncrementalSearchWordsCount.Content = LbIncrementalSearchResults.Items.Count;
                }
                else
                {
                    TbIncrementalSearch.Text = "";
                }
            } else
            {
                LbIncrementalSearchResults.Visibility = Visibility.Hidden;
                LblIncrementalSearchTime.Content = "";
                LblIncrementalSearchUITime.Content = "";
                LblIncrementalSearchWordsCount.Content = "";
                TbSearchString.Text = "";
            }

        }

        private void LbIncrementalSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb.SelectedIndex > -1)
            {
                TbIncrementalSearch.Text = lb.SelectedItem.ToString();
                lb.Visibility = Visibility.Hidden;
                lb.SelectedIndex = -1;
            }
        }
    }
}
