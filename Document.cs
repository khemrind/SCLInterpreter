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
            PrintHeader(" interpreted source ");

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
        }

        public static void PrintHeader(object text = null)
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            var print = (text == null) ? "" : text.ToString();
            Console.WriteLine(print);
            Console.ResetColor();
        }
    }
}
