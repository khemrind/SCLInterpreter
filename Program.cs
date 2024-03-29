﻿using Newtonsoft.Json;
using System.Diagnostics;

namespace Interpreter
{
    public static class Program
    {
        // program document
        public static Document Document { get; set; }

        public static void Main(string[] args)
        {
            // create timer
            Stopwatch stopwatch = new();
            long process_time = -1;
            long execution_time = -1;

            // start message
            Console.WriteLine("SCL Interpreter (CS4308) Khemrind Ung, 2022. \n"
                + "Visit https://github.com/khemrind/SCLInterpreter for source.\n");

            // read file
            if (TryGetSource(out string text, out string filename, args) == false)
                Console.WriteLine("Program encountered an error.");

            else
            {
                // initialize document with text
                Document = new(source: text);
                Document.Name = Path.GetFileNameWithoutExtension(filename);

                // start process and record time
                stopwatch.Start(); 
                Scanner.Process();
                stopwatch.Stop();

                // print highlighted text and word content
                Document.PrintToConsole();
                Document.PrintWords();

                // parse document
                Document.PrintHeader("parsing");
                stopwatch.Start();
                try { Parser.Process(); }
                catch (ParseException error) 
                { 
                    Console.WriteLine($"Program encountered an error: \n{error.Message}\n");
                    var lines = Document.Source.Split("\n");

                    // print the erroneous lines
                    if (error.LineNumber >= 0) Console.WriteLine(lines[error.LineNumber - 1].Trim());
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(lines[error.LineNumber].Trim());
                    Console.ForegroundColor = ConsoleColor.White;
                    if (error.LineNumber < lines.Length) Console.WriteLine(lines[error.LineNumber + 1].Trim());
                }
                stopwatch.Stop();
                Console.WriteLine();
                process_time = stopwatch.ElapsedMilliseconds;

                // print code
                Document.PrintHeader("generated code");
                Console.WriteLine(Parser.Code);
                File.WriteAllText($"{Document.Name}.cs", Parser.Code);
                Console.WriteLine();

                // compile and execute
                Document.PrintHeader("execution");
                stopwatch.Restart();
                Executer.Process();
                stopwatch.Stop();
                execution_time = stopwatch.ElapsedMilliseconds;
            }

            // end program
            Console.WriteLine($"\nProcess time: {process_time}ms");
            Console.WriteLine($"Execution time: {execution_time}ms");
            Document.WriteWith("Press any key to exit...\n", ConsoleColor.Red);
            Console.ReadKey();
        }

        public static bool TryGetSource(out string text, out string filename, string[] args)
        {
            text = string.Empty;
            filename = string.Empty;

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
                    if (path == "") path = "C:\\projects\\Interpreter\\assets\\test.scl";
                    filename = path;

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
                    // read string from file
                    text = File.ReadAllText(args[0]);
                    filename = args[0];
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