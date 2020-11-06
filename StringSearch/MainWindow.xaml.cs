﻿using Microsoft.VisualBasic.CompilerServices;
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
        private Stopwatch timer = new Stopwatch();
        private string searchedString = "";
        private bool listIsCreated = false;
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
            listIsCreated = true;
        }

        private void BtnSerialLinearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (ListIsCreated())
            {

                List<string> matchingWords = new List<string>();
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

        private void BtnFindAllMethod_Click(object sender, RoutedEventArgs e)
        {
            if (ListIsCreated())
            {

                timer.Reset();
                timer.Start();
                string[] matchingWords = Array.FindAll(wordList, element => element.StartsWith(searchedString, StringComparison.Ordinal));
                timer.Stop();
                LblFindAllMethodTime.Content = timer.ElapsedMilliseconds + " ms";

                timer.Reset();
                timer.Start();
                WordList.Display(matchingWords, LbFindAllMethodResults);
                timer.Stop();
                LblFindAllMethodUITime.Content = timer.ElapsedMilliseconds + " ms";
                LblFindAllMethodWordsCount.Content = LbSerialLinearSearchResults.Items.Count;
            }
        }

        private bool ListIsCreated()
        {
            if (listIsCreated)
            {
                return true;
            }
            string message = "Es wurde noch keine Wortliste erstellt, daher gibt es noch nichts zu durchsuchen! Bitte zuerst eine Wortliste generieren.";
            string title = "Keine Wortliste vorhanden";
            MessageBox.Show(message, title);
            return false;
        }
    }
}
