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

            // comments
            Register(@"\/\/.*\n", 0, Part.Comment);

            // literals
            Register("\".*?\"|\\<.*\\>", 0, Part.Literal);

            // description
            Register(@"description([\S\s]*?\*\/)", 1, Part.Comment);

            // types
            Register(@"\btype ((unsigned )?\w+)", 1, Part.Type);
            Register(@"definetype \w+ \w+ (\w+)", 1, Part.Type);

            // constants
            Register(@"symbol (\w+)", 1, Part.Constant);

            // numeric
            Register(@"\b[0-9]+[\.]?[0-9]*\b", 0, Part.Numeric);

            // keywords
            Register(Constant.GetOrPattern(Constant.Keywords, isolated: true), 0, Part.Keyword);

            // controllers
            Register(Constant.GetOrPattern(Constant.Conditionals, isolated: true), 0, Part.Conditional);

            // identifiers
            Register(@"define +(\w+)", 1, Part.Identifier);
            Register(@"set +(\w+)", 1, Part.Identifier);
            Register(@"(\w+) of type ", 1, Part.Identifier);
            Register(@"(\w+) array *\[\] of type ", 1, Part.Identifier);
            Register(@", +(\w+)", 1, Part.Identifier);
            Register(@"using (\w+)", 1, Part.Identifier);

            // methods
            Register(@"function +(\w+)", 1, Part.Method);
            Register(@"endfun +(\w+)", 1, Part.Method);
            Register(@"call (\w+)\b", 1, Part.Method);
            Register(@"call +\w+\.(\w+)", 1, Part.Method);
            Register(@"(\w+)\(.*\)", 1, Part.Method);

            // operators
            Register(Constant.GetOrPattern(Constant.Operators), 0, Part.Operator);

            // binary operators
            Register(Constant.GetOrPattern(Constant.BinaryOperators, isolated: true), 0, Part.BinaryOperator);

            // inert
            Register(@"\.|\(|\)|,|\[|\]", 0, Part.Inert);

            // identifiers 2
            Register(@"\w+", 0, Part.Identifier);
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

                if (Document.Colors[word.Start] == null) Document.Words.Add(word);

                // fill table with color
                for (int index = word.Start; index < word.End; index++)
                    if (Document.Colors[index] == null) Document.Colors[index] = color;

                
            }
        }

        public static ConsoleColor GetColorFrom(Part part)
        {
            return part switch
            {
                Part.Type => ConsoleColor.Blue,
                Part.Constant => ConsoleColor.DarkCyan,
                Part.Numeric => ConsoleColor.Red,
                Part.Literal => ConsoleColor.DarkYellow,
                Part.Identifier => ConsoleColor.White,
                Part.Method => ConsoleColor.Yellow,
                Part.BinaryOperator => ConsoleColor.DarkMagenta,
                Part.Operator => ConsoleColor.Gray,
                Part.Conditional => ConsoleColor.Magenta,
                Part.Keyword => ConsoleColor.Cyan,
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
        Numeric,
        Literal,
        Identifier,
        Method,
        BinaryOperator,
        Operator,
        Conditional,
        Keyword,
        Comment,
        Inert
    }
}
