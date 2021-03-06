﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch
{
    class WordList
    {
        private const int dimension = 26;
        public static bool ShuffledListIsCreated { get; set; } = false;
        public static bool SortedListIsCreated { get; set; } = false;
        private static string[] Letters { get; set; } = new string[dimension] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        public static List<string> ShuffledWordList { get; set; } = new List<string>();
        public static List<string> SortedWordList { get; set; } = new List<string>();

        public static bool IsAvailable(string type)
        {
            if (type == "shuffled" && ShuffledListIsCreated)
            {
                return true;
            }
            else if (type == "sorted" && SortedListIsCreated)
            {
                return true;
            }
            Display.NoListAvailable();
            return false;
        }

        public static void Create()
        {
            SortedListIsCreated = false;
            SortedWordList.Clear();
            for (int i = 0; i < dimension; i++)
            {
                for (int j = 0; j < dimension; j++)
                {
                    for (int k = 0; k < dimension; k++)
                    {
                        for (int l = 0; l < dimension; l++)
                        {
                            SortedWordList.Add(Letters[i] + Letters[j] + Letters[k] + Letters[l]);
                        }
                    }
                }
            }
        }

        public static void Shuffle()
        {
            ShuffledWordList.Clear();
            // Fisher-Yates shuffle.
            Random rnd = new Random();
            int n = SortedWordList.Count;

            for (int i = n - 1; i >= 0; i--)
            {
                int randomIndex = rnd.Next(0, i + 1); // Get random index between 0 and remaining unshuffled list item count.
                string tempString = SortedWordList[i];
                ShuffledWordList.Add(SortedWordList[randomIndex]);
                SortedWordList[randomIndex] = tempString;

            }
            ShuffledListIsCreated = true;
        }

        public static void Sort(int maxAllowedThreads)
        {

            if (!SortedListIsCreated)
            {
                SortedWordList.Clear();
                if (IsAvailable("shuffled"))
                {
                    SortedWordList = ShuffledWordList.AsParallel().WithDegreeOfParallelism(maxAllowedThreads).OrderBy(word => word).ToList();
                    SortedListIsCreated = true;
                }
            }
        }

        public static List<string> SerialLinearSearch(List<string> wordList, string searchedString, int start, int end)
        {
            List<string> matchingWords = new List<string>();

            for (int i = start; i < end; i++)
            {
                if (wordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(wordList[i]);
                }
            }

            return matchingWords;
        }

        public static List<string> ParallelLinearSearch(List<string> wordList, string searchedString, int maxAllowedThreads)
        {
            //List<string> matchingWords = new List<string>();

            //Parallel.ForEach(Partitioner.Create(0, wordList.Count), new ParallelOptions { MaxDegreeOfParallelism = maxAllowedThreads }, (range, state) =>
            //{
            //    for (int i = range.Item1; i < range.Item2; i++)
            //    {
            //        if (wordList[i].StartsWith(searchedString))
            //        {
            //            lock (matchingWords)
            //            {
            //                matchingWords.Add(wordList[i]);
            //            }
            //        }
            //    }
            //});

            //return matchingWords;


            List<string> matchingWords = new List<string>();

            Task<List<string>>[] tasks = new Task<List<string>>[maxAllowedThreads];

            int step = wordList.Count / maxAllowedThreads;

            for (int i = 0; i < tasks.Length; i++)
            {
                int ilocal = i;
                tasks[ilocal] = Task.Run(() =>
                {
                    List<string> result = new List<string>();
                    result = SerialLinearSearch(wordList, searchedString, ilocal * step, (ilocal == tasks.Length - 1) ? wordList.Count : (ilocal + 1) * step);
                    return result;
                });
            }
            Task.WaitAll(tasks);
            for (int i = 0; i < tasks.Length; i++)
            {
                matchingWords.AddRange(tasks[i].Result);
            }
            return matchingWords;
        }

        public static List<string> SerialBinarySearch(List<string> wordList, string searchedString)
        {
            List<string> matchingWords = new List<string>();

            int minIndex = 0;
            int maxIndex = wordList.Count - 1;
            int matchingIndex = -1;

            while (minIndex <= maxIndex)
            {
                int middleIndex = (minIndex + maxIndex) / 2;
                if (wordList[middleIndex].StartsWith(searchedString))
                {
                    matchingIndex = middleIndex;
                    break;
                }
                // String at current position < searched string.
                else if (String.Compare(wordList[middleIndex].Substring(0, Math.Min(searchedString.Length, wordList[middleIndex].Length)), searchedString) <= 0)
                {
                    minIndex = middleIndex + 1; // Get rid of lower half.
                }
                else // String at current position > searched string. 
                {
                    maxIndex = middleIndex - 1; // Get rid of upper half.
                }
            }

            if (matchingIndex > -1)
            {
                // Find every matching word below the matching index, including the latter.
                int i = matchingIndex;
                while (i >= 0 && wordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(wordList[i--]);
                }

                // Find every matching word above the matching index.
                i = matchingIndex + 1;
                while (i < SortedWordList.Count && wordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(wordList[i++]);
                }
            }

            return matchingWords;
        }
    }
}
