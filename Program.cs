using System.Diagnostics;

namespace Interpreter
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Stopwatch stopwatch = new();

            // start message
            Console.WriteLine("SCL Interpreter (CS4308) Khemrind Ung. \nVisit https://www.github.com/khemrind for source.\n");

            // read file
            if (TryGetSource(out string text, args) == false)
                Console.WriteLine("Program encountered an error."); 

            else
            {
                stopwatch.Start();

                var document = new Document(source: text);

                Scanner.Process(document);

                document.PrintToConsole();

                stopwatch.Stop();
            }

            // end program
            Console.WriteLine($"\nProcess time: {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("Press any key to exit...", ConsoleColor.Red);
            Console.ReadKey();
        }

        public static bool TryGetSource(out string text, string[] args)
        {
            text = string.Empty;

            // default, testing
            if (args.Length == 0) 
            {
                try
                {
                    // prompt
                    Console.Write("Enter source file path (.scl): ");
                    string path = Console.ReadLine();

                    // TESTING
                    if (path == "") path = "C:\\projects\\Interpreter\\assets\\welcome.scl";

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

            // user defined
            else if (args.Length == 1) 
            {
                try
                {
                    text = File.ReadAllText(args[0]);
                    Console.WriteLine();
                    return true;
                }

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