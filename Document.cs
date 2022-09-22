using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter
{
    public class Document
    {
        public string Source { get; set; }
        public ConsoleColor?[] Colors { get; set; }
        public List<Word> Words { get; set; } = new();

        public Document(string source)
        {
            Source = source;
            Colors = new ConsoleColor?[source.Length];
        }

        public void PrintToConsole()
        {
            PrintHeader("interpreted source");

            var limit = Source.Length;
            for (int index = 0; index < limit; index++)
            {
                var character = Source[index];
                var color = Colors[index];

                if (color is ConsoleColor defined)
                    Console.ForegroundColor = defined;
                else Console.ForegroundColor = ConsoleColor.White;
                
                Console.Write(character);
            }

            Console.WriteLine();
        }

        public void PrintWords()
        {
            var parts = Enum.GetValues(typeof(Part)).Cast<Part>();
            foreach (var part in parts)
                PrintWordGroup(part);
        }

        private void PrintWordGroup(Part part)
        {
            PrintHeader(part.ToString());
            var group = Words.Where(item => item.Part == part);
            Console.WriteLine("[");
            foreach (var word in group)
            {
                var value = Source[word.Start..word.End];

                WriteWith($"    {value.Trim()} ", Scanner.GetColorFrom(part));
                WriteWith($"[", ConsoleColor.Red);
                WriteWith(word.Start, ConsoleColor.Gray);
                WriteWith($":", ConsoleColor.Red);
                WriteWith(word.End, ConsoleColor.Gray);
                WriteWith($"]", ConsoleColor.Red);
                Console.WriteLine(",");
            }
            Console.WriteLine("]\n");
        }

        private static void WriteWith(object text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintHeader(object text = null)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            var print = (text == null) ? "" : text.ToString();
            Console.Write(" " + print.Trim() + " ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("");
            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
