using System;
using System.Linq;
using System.Collections;
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

                var (wordsFirstHalf, wordsSecondHalf) = tripletsProcessing.SplitText(text);

                var tripletsTable = tripletsProcessing.SearchTriplets(wordsFirstHalf, wordsSecondHalf);
                
                var sortedTripletsTable = from tripletPair in tripletsTable
                                          orderby tripletPair.Value descending
                                          select tripletPair;

                var topTenOfTriplets = sortedTripletsTable.Take(10);
                foreach (var tripletWithCount in topTenOfTriplets){
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

            public ( ArrayList, ArrayList ) SplitText(string text)
            {
                var wordsFirstHalf = new ArrayList();
                var wordsSecondHalf = new ArrayList();


                var splittedWords = Regex.Split(text, @"[\r|\t|\p{P}|\p{S}|\p{Z}]");

                var splittedWordsFirstHalf = new string[splittedWords.Length / 2] ;

                for (var countWords = 1; countWords < splittedWords.Length / 2; countWords++)
                {
                    splittedWordsFirstHalf[countWords] = splittedWords[countWords];
                }

                var splittedWordsSecondHalf = new string[splittedWords.Length / 2 + 1];

                for (var countWords = splittedWords.Length / 2; countWords < splittedWords.Length; countWords++)
                {
                    splittedWordsSecondHalf[countWords - splittedWords.Length / 2] = splittedWords[countWords];
                }


                foreach (var word in splittedWordsFirstHalf)
                {
                    if (!String.IsNullOrEmpty(word) && word.Length > 2)
                        wordsFirstHalf.Add(word);
                }

                foreach (var word in splittedWordsSecondHalf)
                {
                    if (!String.IsNullOrEmpty(word) && word.Length > 2)
                        wordsSecondHalf.Add(word);
                }

                return (wordsFirstHalf, wordsSecondHalf);
            }

            public ConcurrentDictionary<string, int> SearchTriplets(ArrayList wordsFirstHalf, ArrayList wordsSecondHalf)
            {
                var tripletsTable = new ConcurrentDictionary<string, int>();

                string triplet = "";

                Thread myThread = new Thread(AddingWordsInSecondThread);
                myThread.Start();

                void AddingWordsInSecondThread()
                {
                    foreach (string word in wordsFirstHalf)
                    {
                        for (int position = 0; position < (word.Length - 2); position++)
                        {
                            triplet = word.Substring(position, 3);
                            tripletsTable.AddOrUpdate(triplet, 1, (key, oldValue) => oldValue + 1);
                        }
                    }
                }

                foreach (string word in wordsSecondHalf)
                {
                    for (int position = 0; position < (word.Length - 2); position++)
                    {
                        triplet = word.Substring(position, 3);
                        tripletsTable.AddOrUpdate(triplet, 1, (key, oldValue) => oldValue + 1);
                    }
                }

                return tripletsTable;
            }
        }
    }
}
