using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace StringSearch
{
    class Display
    {
        public static void NoListAvailable()
        {
            string title = "Keine Wortliste vorhanden";
            string message = "Es ist noch keine zu durchsuchende Wortliste vorhanden! Bitte erst die entsprechende Liste erstellen.";
            MessageBox.Show(message, title);
        }

        public static void NotifyUserAboutMaxThreadsAvailable()
        {
            string title = "Zu viele Threads für dieses System";
            string message = "Auf diesem System stehen nur " + Environment.ProcessorCount + " Threads zur Verfügung! Die maximale Anzahl der für die Operation erlaubten Threads wurde auf " + Environment.ProcessorCount + " gesetzt.";
            MessageBox.Show(message, title);
        }

        public static void PrintList(List<string> wordList, ListBox listBox)
        {
            listBox.Items.Clear();
            for (int i = 0; i < wordList.Count; i++)
            {
                listBox.Items.Add(wordList[i]);
            }
        }

        public static void PrintResults (List<string> wordList, ListBox listbox, Label calculationTimeLabel, long calculationTime, Label UIUpdateTimeLabel, Label ListCountLabel)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            PrintList(wordList, listbox);
            timer.Stop();

            calculationTimeLabel.Content = calculationTime + " ms";
            UIUpdateTimeLabel.Content = timer.ElapsedMilliseconds + " ms";
            ListCountLabel.Content = listbox.Items.Count;
        }
    }
}
