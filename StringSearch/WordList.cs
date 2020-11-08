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
            string title = "Keine Wortliste vorhanden";
            string message = "Es wurde noch keine Wortliste erstellt, daher gibt es noch nichts zu durchsuchen! Bitte zuerst eine Wortliste generieren.";
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

        public void Sort(string[] wordlist)
        {
            Array.Sort(wordlist);
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
                //if (wordList[i].StartsWith(searchedString, StringComparison.Ordinal))
                if (wordList[i].StartsWith(searchedString))
                    {
                    matchingWords.Add(wordList[i]);
                }
            }
        }

        public static void ParallelLinearSearch(string[] wordList, string searchedString, List<string> matchingWords, int maxAllowedThreads)
        {
            //ConcurrentBag<int> threadIDs = new ConcurrentBag<int>();

            Parallel.ForEach(Partitioner.Create(0, wordList.Length), new ParallelOptions { MaxDegreeOfParallelism = maxAllowedThreads }, (range, state) =>
            {
                //threadIDs.Add(Thread.CurrentThread.ManagedThreadId);

                for (int i = range.Item1; i < range.Item2; i++)
                {
                    //if (wordList[i].StartsWith(searchedString, StringComparison.Ordinal))
                    if (wordList[i].StartsWith(searchedString))
                    {
                        lock (matchingWords)
                        {
                            matchingWords.Add(wordList[i]);
                        }
                    }
                }
            });

            //Parallel.For(0, wordList.Length, new ParallelOptions { MaxDegreeOfParallelism = maxAllowedThreads }, (i) =>
            //{
            //    threadIDs.Add(Thread.CurrentThread.ManagedThreadId);
            //    //Thread.Sleep(1);
            //    if (wordList[i].StartsWith(searchedString, StringComparison.Ordinal))
            //    {
            //        lock(matchingWords)
            //        {
            //            matchingWords.Add(wordList[i]);
            //        }
            //    }
            //});

            //int usedThreads = threadIDs.Distinct().Count();
            //return usedThreads;
        }

        public static void SerialBinarySearch(string[] sortedWordList, string searchedString, List<string> matchingWords)
        {
            int minIndex = 0;
            int maxIndex = sortedWordList.Length -1;
            int matchingIndex = -1;

            while(minIndex <= maxIndex)
            {
                int middleIndex = (minIndex + maxIndex) / 2;
                if (sortedWordList[middleIndex].StartsWith(searchedString))
                {
                    matchingIndex = middleIndex;
                    break;
                //} else if ( String.CompareOrdinal(sortedWordList[middleIndex], 0, searchedString, 0, searchedString.Length) < 0)
                } else if ( String.Compare(sortedWordList[middleIndex].Substring(0, Math.Min(searchedString.Length, sortedWordList[middleIndex].Length)), searchedString) <= 0)
            {
                    minIndex = middleIndex + 1;
                } else
                {
                    maxIndex = middleIndex - 1;
                }
            }

            if (matchingIndex > -1)
            {
                int i = matchingIndex;
                while (i >= 0 && sortedWordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(sortedWordList[i--]);
                }

                i = matchingIndex + 1;
                while (i < sortedWordList.Length && sortedWordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(sortedWordList[i++]);
                }
            }

        }

        public static List<string> SerialLinearListSearch(List<string> wordList, string searchedString)
        {
            List<string> matchingWords = new List<string>();
            for (int i = 0; i < wordList.Count; i++)
            {
                if (wordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(wordList[i]);
                }
            }
            return matchingWords;
        }
    }
}
