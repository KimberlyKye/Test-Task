using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;


namespace testtest
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.Write("Enter path of file: ");
            var path = Console.ReadLine();

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            StreamReader reader = null;

            try
            {
                reader = new StreamReader(path);
                var text = reader.ReadToEnd();

                TripletsProcessing tripletsProcessing = new TripletsProcessing();
                var tripletsTable = new ConcurrentDictionary<string, int>();

                var words = tripletsProcessing.SplitText(text);
                var wordsFirstHalf = words.Take(words.Count / 2);
                var wordsSecondHalf = words.Skip(words.Count / 2).Take(words.Count / 2);


                tripletsProcessing.SearchTriplets(wordsFirstHalf, ref tripletsTable);

                Thread secondThreadForSearchingTriplets = new Thread(AddingWordsInSecondThread);
                secondThreadForSearchingTriplets.Start();

                void AddingWordsInSecondThread()
                {
                    tripletsProcessing.SearchTriplets(wordsSecondHalf, ref tripletsTable);
                };

                var sortedTripletsTable = from tripletPair in tripletsTable
                                          orderby tripletPair.Value descending
                                          select tripletPair;

                var topTenOfTriplets = sortedTripletsTable.Take(100);
                foreach (var tripletWithCount in topTenOfTriplets)
                {
                    Console.WriteLine($"Triplet \"{tripletWithCount.Key}\" is using {tripletWithCount.Value} times");
                }


            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine("{0} Exception caught.", ex);
            }
            finally
            {
                reader?.Close();
            }

            stopwatch.Stop();
            Console.Write("\n The program's working time is {0} ms", stopwatch.ElapsedMilliseconds);


        }

        class TripletsProcessing
        {

            public List<string> SplitText(string text)
            {
                var words = new List<string>();

                var splittedWords = Regex.Split(text, @"[\r|\t|\n|\p{P}|\p{S}|\p{Z}]");

                foreach (var word in splittedWords)
                {
                    if (!String.IsNullOrEmpty(word) && word.Length > 2)
                        words.Add(word);
                }

                return words;
            }

            public void SearchTriplets(IEnumerable<string> words, ref ConcurrentDictionary<string, int> tripletsTable)
            {
                string triplet = "";

                foreach (string word in words)
                {
                    for (int position = 0; position < (word.Length - 2); position++)
                    {
                        triplet = word.Substring(position, 3);
                        tripletsTable.AddOrUpdate(triplet, 1, (key, oldValue) => oldValue + 1);
                    }
                }
            }
        }
    }
}
