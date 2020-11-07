using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace StringSearch
{
    class WordList
    {
        private static bool ShuffledListIsAvailable { get; set; } = false;

        public static bool IsAvailable()
        {
            if (ShuffledListIsAvailable)
            {
                return true;
            }
            string message = "Es wurde noch keine Wortliste erstellt, daher gibt es noch nichts zu durchsuchen! Bitte zuerst eine Wortliste generieren.";
            string title = "Keine Wortliste vorhanden";
            MessageBox.Show(message, title);
            return false;
        }

        public static void Create(string[] wordList, string[] letters, int dimension)
        {
            int index = 0;
            for (int i = 0; i < dimension; i++)
            {
                //Thread.Sleep(400);
                for (int j = 0; j < dimension; j++)
                {
                    for (int k = 0; k < dimension; k++)
                    {
                        for (int l = 0; l < dimension; l++)
                        {
                            wordList[index++] = letters[i] + letters[j] + letters[k] + letters[l];
                        }
                    }
                }
            }
        }

        public static void Shuffle(string[] wordList)
        {
            //Fisher-Yates shuffle
            Random rnd = new Random();
            int n = wordList.Length;

            for (int i = n - 1; i > 0; i--)
            {
                int randomIndex = rnd.Next(0, i + 1);
                string tempString = wordList[i];
                wordList[i] = wordList[randomIndex];
                wordList[randomIndex] = tempString;

            }
            ShuffledListIsAvailable = true;
        }

        public static void Display(string[] wordList, ListBox listBox)
        {
            listBox.Items.Clear();
            for (int i = 0; i < wordList.Length; i++)
            {
                listBox.Items.Add(wordList[i]);
            }
        }

        public static void DisplayList(List<string> wordList, ListBox listBox)
        {
            listBox.Items.Clear();
            for (int i = 0; i < wordList.Count; i++)
            {
                listBox.Items.Add(wordList[i]);
            }
        }

        public static void SerialLinearSearch(string[] wordList, string searchedString, List<string> matchingWords)
        {
            for (int i = 0; i < wordList.Length; i++)
            {
                //Thread.Sleep(1);
                if (wordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(wordList[i]);
                }
            }
        }

        public static int ParallelLinearSearch(string[] wordList, string searchedString, List<string> matchingWords)
        {
            ConcurrentBag<int> threadIDs = new ConcurrentBag<int>();

            Parallel.ForEach(Partitioner.Create(0, wordList.Length), new ParallelOptions { MaxDegreeOfParallelism = -1 }, (range, state) =>
            {
                threadIDs.Add(Thread.CurrentThread.ManagedThreadId);

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    if (wordList[i].StartsWith(searchedString))
                    {
                        lock (matchingWords)
                        {
                            matchingWords.Add(wordList[i]);
                        }
                    }
                }
            });

            //Parallel.For(0, wordList.Length, new ParallelOptions { MaxDegreeOfParallelism = -1 }, (i) =>
            //{
            //    threadIDs.Add(Thread.CurrentThread.ManagedThreadId);
            //    //Thread.Sleep(1);
            //    if (wordList[i].StartsWith(searchedString))
            //    {
            //        lock(matchingWords)
            //        {
            //            matchingWords.Add(wordList[i]);
            //        }
            //    }
            //});

            int usedThreads = threadIDs.Distinct().Count();
            return usedThreads;
        }
    }
}
