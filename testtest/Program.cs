using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace testtest
{
    class MainClass
    {
        public static async Task Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            string path = "in.put";

            StreamReader reader = null;

            try
            {
                reader = new StreamReader(path);
                string text = await reader.ReadToEndAsync();
                var words = SplitText(text);
                foreach (string word in words)
                {
                    Console.WriteLine("{0}, ", word);
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
            Console.WriteLine("{0} ms", stopwatch.ElapsedMilliseconds);

            ArrayList SplitText(string text)
            {
                var words = new ArrayList();

                string[] splittedWords = Regex.Split(text, @"[\r|\p{P}|\t|\s]");

                foreach (string word in splittedWords)
                {
                    if (!String.IsNullOrEmpty(word))
                        words.Add(word);
                }

                return words;
            }
        }

     
    }

      
}
