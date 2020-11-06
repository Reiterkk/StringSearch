using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace StringSearch
{
    class WordList
    {
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
            //List<string> matchingWords = new List<string>();
            for (int i = 0; i < wordList.Length; i++)
            {
                if (wordList[i].StartsWith(searchedString))
                {
                    matchingWords.Add(wordList[i]);
                }
            }
            //return matchingWords;
        }
    }
}
