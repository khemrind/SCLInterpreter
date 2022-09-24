using Newtonsoft.Json;
using System;

namespace Interpreter
{
    public class Document
    {
        // source name
        public string Name { get; set; }

        // source text
        public string Source { get; set; }

        // array of colors assigned to each character
        public ConsoleColor?[] Colors { get; set; }

        // word list
        public List<Word> Words { get; set; } = new();

        // word record for output
        public List<List<object>> WordData = new();

        public Document(string source)
        {
            Source = source;

            // initialize array with null
            Colors = new ConsoleColor?[source.Length];
        }

        public void PrintToConsole()
        {
            PrintHeader("interpreted source");

            // print each character with its defined color
            var limit = Source.Length;
            for (int index = 0; index < limit; index++)
            {
                // retrieve character and color
                var character = Source[index];
                var color = Colors[index];

                // use a color is it's defined, white otherwise
                if (color is ConsoleColor defined)
                    Console.ForegroundColor = defined;
                else Console.ForegroundColor = ConsoleColor.White;
                
                // print to console
                Console.Write(character);
            }

            Console.WriteLine();
        }

        public void PrintWords()
        {
            // print each category of word: part
            var parts = Enum.GetValues(typeof(Part)).Cast<Part>().ToArray();
            foreach (var part in parts)
            {
                // add record to word data
                WordData.Add(PrintWordGroup(part));
            }

            // output data to separate json files
            int index = 0;
            foreach (var record in WordData)
            {
                // create file name
                var filename = $"{Name}_{parts[index].ToString().ToLower()}.json";

                // skip empty records
                if (record.Count >= 1)
                {
                    // serialize and write to file
                    var serialized = JsonConvert.SerializeObject(record, Formatting.Indented);
                    File.WriteAllText(filename, serialized);
                }

                index++;
            }
        }

        private List<object> PrintWordGroup(Part part)
        {
            PrintHeader(part.ToString());

            // create a new record
            var record = new List<object>();

            // retrieve words of a specific category
            var group = Words.Where(item => item.Part == part);

            // write each word with its location
            Console.WriteLine("[");
            foreach (var word in group)
            {
                var value = Source[word.Start..word.End];

                // create an entry
                var entry = new
                {
                    value,
                    start = word.Start,
                    end = word.End,
                };

                // add entry to record
                record.Add(entry);

                // print json style
                WriteWith($"    {value.Trim()} ", Scanner.GetColorFrom(part));
                WriteWith($"[", ConsoleColor.Red);
                WriteWith(word.Start, ConsoleColor.Gray);
                WriteWith($":", ConsoleColor.Red);
                WriteWith(word.End, ConsoleColor.Gray);
                WriteWith($"]", ConsoleColor.Red);
                Console.WriteLine(",");
            }

            Console.WriteLine("]\n");

            return record;
        }

        public static void WriteWith(object text, ConsoleColor color)
        { 
            // write to console with specified color
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void PrintHeader(object text = null)
        {
            // print header, reset color
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Gray;
            var print = (text == null) ? "" : text.ToString();
            Console.Write(" " + print.Trim() + " ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine();
        }
    }
}
