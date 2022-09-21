using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Interpreter
{
    public static class Scanner
    {
        public static Document Document { get; set; }

        public static void Process(Document document)
        {
            Document = document;

            // literals
            Register("\".*\"", 0, Part.Literal);

            // comments
            Register(@"\/\/.*\n", 0, Part.Comment);

            // description
            Register(@"description([\S\s]*?\*\/)", 1, Part.Comment);

            // types
            Register(@"type ((unsigned )?\w+)", 1, Part.Type);

            // keywords
            Register(Constant.GetOrPattern(Constant.Keywords), 0, Part.Keyword);

            // keywords
            Register(Constant.GetOrPattern(Constant.Controllers), 0, Part.Controller);

            
        }

        private static void Register(string pattern, int group, Part part)
        {
            var color = GetColorFrom(part);
            var matches = Regex.Matches(Document.Source, pattern);
            foreach (Match match in matches)
            {
                var target = match.Groups[group];

                // create word
                Word word = new()
                {
                    Start = target.Index,
                    End = target.Index + target.Length,
                    Part = part
                };

                // fill table with color
                for (int index = word.Start; index < word.End; index++)
                    if (Document.Colors[index] == null) Document.Colors[index] = color;

                Document.Words.Add(word);
            }
        }

        private static ConsoleColor GetColorFrom(Part part)
        {
            return part switch
            {
                Part.Type => ConsoleColor.Cyan,
                Part.Constant => ConsoleColor.Gray,
                Part.Literal => ConsoleColor.DarkYellow,
                Part.Operator => ConsoleColor.White,
                Part.Controller => ConsoleColor.Magenta,
                Part.Keyword => ConsoleColor.Blue,
                Part.Comment => ConsoleColor.DarkGray,
                Part.Inert => ConsoleColor.White,
                _ => ConsoleColor.White,
            };
        }
    }

    public class Word
    {
        public int Start { get; set; }
        public int End { get; set; }
        public Part Part { get; set; } = Part.Inert;
    }

    public enum Part
    {
        Type,
        Constant,
        Literal,
        Operator,
        Controller,
        Keyword,
        Comment,
        Inert
    }
}
