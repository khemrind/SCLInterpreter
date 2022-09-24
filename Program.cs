using System.Diagnostics;

namespace Interpreter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // create timer
            Stopwatch stopwatch = new();

            // start message
            Console.WriteLine("SCL Interpreter (CS4308) Khemrind Ung. \nVisit https://www.github.com/khemrind for source.\n");

            // read file
            if (TryGetSource(out string text, args) == false)
                Console.WriteLine("Program encountered an error."); 

            else
            {
                // initialize document with text
                var document = new Document(source: text);

                // start process and record time
                stopwatch.Start(); 
                Scanner.Process(document);
                stopwatch.Stop();

                // print highlighted text and word content
                document.PrintToConsole();
                document.PrintWords();
            }

            // end program
            Console.WriteLine($"\nProcess time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("Press any key to exit...", ConsoleColor.Red);
            Console.ReadKey();
        }

        public static bool TryGetSource(out string text, string[] args)
        {
            text = string.Empty;

            #region Testing
            // default, testing
            // pressing enter with no path assumes a hardcoded testing path
            if (args.Length == 0) 
            {
                try
                {
                    // prompt
                    Console.Write("Enter source file path (.scl): ");
                    string path = Console.ReadLine();

                    // test path
                    if (path == "") path = "C:\\projects\\Interpreter\\assets\\linkedg.scl";

                    // read text
                    text = File.ReadAllText(path);
                    Console.WriteLine();
                    return true;
                }

                catch (Exception error) 
                {
                    Console.WriteLine(error.Message);
                    return false;
                }
            }
            #endregion

            // user defined
            else if (args.Length == 1) 
            {
                try
                {
                    // read string from file
                    text = File.ReadAllText(args[0]);
                    Console.WriteLine();
                    return true;
                }

                // report error
                catch (Exception error) 
                {
                    Console.WriteLine(error.Message);
                    return false;
                }
            }

            // too many arguments
            else return false;
        }
    }
}