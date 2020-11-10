using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace StringSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int MaxAllowedThreads { get; set; } = 1;
        private int LastStringLength { get; set; } = 0;
        private int LastListCount { get; set; } = 1;
        private string SearchedString { get; set; } = "";
        private List<string> MatchingWords { get; set; } = new List<string>();
        private List<List<string>> IncrementalMatchingWords = new List<List<string>>();
        private Stopwatch Timer { get; set; } = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
        }
        private void TbSearchString_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchedString = TbSearchString.Text;
        }

        private void RBtn1_Checked(object sender, RoutedEventArgs e)
        {
            MaxAllowedThreads = 1;
        }

        private void RBtn2_Checked(object sender, RoutedEventArgs e)
        {
            if (Environment.ProcessorCount < 2)
            {
                Display.NotifyUserAboutMaxThreadsAvailable();
                RBtn2.IsChecked = false;
                RBtnMax.IsChecked = true;
            }
            MaxAllowedThreads = Math.Min(2, Environment.ProcessorCount);
        }

        private void RBtn4_Checked(object sender, RoutedEventArgs e)
        {
            if (Environment.ProcessorCount < 4)
            {
                Display.NotifyUserAboutMaxThreadsAvailable();
                RBtn4.IsChecked = false;
                RBtnMax.IsChecked = true;
            }
            MaxAllowedThreads = Math.Min(4, Environment.ProcessorCount);
        }

        private void RBtn8_Checked(object sender, RoutedEventArgs e)
        {
            if (Environment.ProcessorCount < 8)
            {
                Display.NotifyUserAboutMaxThreadsAvailable();
                RBtn8.IsChecked = false;
                RBtnMax.IsChecked = true;
            }
            MaxAllowedThreads = Math.Min(8, Environment.ProcessorCount);
        }

        private void RBtnMax_Checked(object sender, RoutedEventArgs e)
        {
            MaxAllowedThreads = Environment.ProcessorCount;
        }

        private async void BtnGenerateRandomWordList_Click(object sender, RoutedEventArgs e)
        {
            Timer.Reset();
            Timer.Start();
            Task t = Task.Run(() =>
            {
                WordList.Create();
                WordList.Shuffle();
                IncrementalMatchingWords.Add(WordList.ShuffledWordList);
            });
            await Task.WhenAll(t);
            Timer.Stop();
            Display.PrintResults(WordList.ShuffledWordList, LbRandomWordList, LblCreateListTime, Timer.ElapsedMilliseconds, LblUpdateUiCreateListTime, LblListboxItemCount);
        }

        private void BtnSerialLinearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (WordList.IsAvailable("shuffled"))
            {
                MatchingWords.Clear();
                Timer.Reset();
                Timer.Start();
                MatchingWords = WordList.SerialLinearSearch(WordList.ShuffledWordList, SearchedString, 0, WordList.ShuffledWordList.Count);
                Timer.Stop();
                Display.PrintResults(MatchingWords, LbSerialLinearSearchResults, LblSerialLinearSearchTime, Timer.ElapsedMilliseconds, LblSerialLinearSearchUITime, LblSerialLinearSearchWordsCount);
            }
        }

        private void BtnParallelLinearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (WordList.IsAvailable("shuffled"))
            {
                MatchingWords.Clear();
                Timer.Reset();
                Timer.Start();
                MatchingWords = WordList.ParallelLinearSearch(WordList.ShuffledWordList, SearchedString, MaxAllowedThreads);
                Timer.Stop();
                Display.PrintResults(MatchingWords, LbParallelLinearSearchResults, LblParallelLinearSearchTime, Timer.ElapsedMilliseconds, LblParallelLinearSearchUITime, LblParallelLinearSearchWordsCount);
            }
        }

        private void BtnSerialBinarySearch_Click(object sender, RoutedEventArgs e)
        {
            if (WordList.IsAvailable("shuffled"))
            {
                if (!WordList.SortedListIsCreated)
                {
                    Timer.Reset();
                    Timer.Start();
                    WordList.Sort(MaxAllowedThreads);
                    Timer.Stop();
                    Display.PrintResults(WordList.SortedWordList, LbRandomWordList, LblSerialBinarySearchSortListTime, Timer.ElapsedMilliseconds, LblUpdateUiCreateListTime, LblListboxItemCount);
                }

                Timer.Reset();
                Timer.Start();
                MatchingWords = WordList.SerialBinarySearch(WordList.SortedWordList, SearchedString);
                Timer.Stop();
                Display.PrintResults(MatchingWords, LbSerialBinarySearchResults, LblSerialBinarySearchTime, Timer.ElapsedMilliseconds, LblSerialBinarySearchUITime, LblSerialBinarySearchWordsCount);
            }
        }

        private void TbIncrementalSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            RemoveFromIncrementalListIfSearchedStringGotShorter();

            if (TbIncrementalSearch.Text.Length > 0)
            {
                if (WordList.IsAvailable("shuffled"))
                {
                    LbIncrementalSearchResults.Visibility = Visibility.Visible;

                    MatchingWords.Clear();
                    TbSearchString.Text = TbIncrementalSearch.Text;
                    Timer.Reset();
                    Timer.Start();
                    int end = IncrementalMatchingWords[IncrementalMatchingWords.Count - 1].Count;
                    IncrementalMatchingWords.Add(WordList.SerialLinearSearch(IncrementalMatchingWords[IncrementalMatchingWords.Count - 1], SearchedString, 0, end));
                    Timer.Stop();
                    Display.PrintResults(IncrementalMatchingWords[IncrementalMatchingWords.Count - 1], LbIncrementalSearchResults, LblIncrementalSearchTime, Timer.ElapsedMilliseconds, LblIncrementalSearchUITime, LblIncrementalSearchWordsCount);
                }
                else
                {
                    TbIncrementalSearch.Text = "";
                }
            }
            else
            {
                LbIncrementalSearchResults.Visibility = Visibility.Hidden;
                LblIncrementalSearchTime.Content = "";
                LblIncrementalSearchUITime.Content = "";
                LblIncrementalSearchWordsCount.Content = "";
                TbSearchString.Text = "";
            }

        }

        private void RemoveFromIncrementalListIfSearchedStringGotShorter()
        {
            // Current search string is shorter than the last one but not empty -> remove last list entries.
            if (LastStringLength > TbIncrementalSearch.Text.Length && TbIncrementalSearch.Text.Length > 0)
            {
                /* If the characters were typed one after the other, the list should have SearchedString.Lenght entries before the search.
                  If a character has been removed, the list must be reduced until it only has SearchedString.Lenght entries.
                  */
                while (IncrementalMatchingWords.Count > TbIncrementalSearch.Text.Length)
                {
                    IncrementalMatchingWords.RemoveAt(IncrementalMatchingWords.Count - 1);
                }
                // If the user selects a suggested word and then removes a character, the search must continue in the last list before the selection.
                while (IncrementalMatchingWords.Count >= LastListCount && IncrementalMatchingWords.Count > 1)
                {
                    IncrementalMatchingWords.RemoveAt(IncrementalMatchingWords.Count - 1);
                }
            }
            // Search string is empty -> remove all but the initial word list.
            else if (TbIncrementalSearch.Text.Length == 0)
            {
                while (IncrementalMatchingWords.Count > 1)
                {
                    IncrementalMatchingWords.RemoveAt(IncrementalMatchingWords.Count - 1);
                }
            }
            LastStringLength = TbIncrementalSearch.Text.Length;
            LastListCount = IncrementalMatchingWords.Count;
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
