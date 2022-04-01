using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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

                var words = SplitText(text);

                var tripletsTable = SearchTriplets(words);
                
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

            ArrayList SplitText(string text)
            {
                var words = new ArrayList();

                var splittedWords = Regex.Split(text, @"[\r|\p{P}|\t|\s]");

                foreach (var word in splittedWords)
                {
                    if (!String.IsNullOrEmpty(word) && word.Length > 2)
                        words.Add(word);
                }

                return words;
            }

            Dictionary<string, int> SearchTriplets(ArrayList words)
            {
                var tripletsTable = new Dictionary<string, int>();
                string triplet = "";

                foreach (string word in words)
                {
                    for (int position = 0; position < (word.Length - 2); position++)
                    {
                        triplet = word.Substring(position, 3);

                        if (!tripletsTable.ContainsKey(triplet))
                        {
                            tripletsTable.Add(triplet, 1);
                        }
                        else
                        {
                            tripletsTable[triplet] = ++tripletsTable[triplet];
                        }

                    }
                }
                return tripletsTable;
            }
        }
    }
}
