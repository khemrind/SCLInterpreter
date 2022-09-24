using System;
using System.Text.RegularExpressions;

namespace Interpreter
{
    public static class Scanner
    {
        public static Document Document { get; set; }

        public static void Process(Document document)
        {
            Document = document;

            // comments
            Register(@"\/\/.*\n", 0, Part.Comment); // just like this!

            // literals
            Register("\".*?\"|\\<.*\\>", 0, Part.Literal); // quotes and c++ imports

            // description
            Register(@"description([\S\s]*?\*\/)", 1, Part.Comment); // description paragraph

            // types
            Register(@"\btype ((unsigned )?\w+)", 1, Part.Type); // explicit type declaration
            Register(@"definetype \w+ \w+ (\w+)", 1, Part.Type); // with definetype

            // constants
            Register(@"symbol (\w+)", 1, Part.Constant); // with symbol

            // numeric
            Register(@"\b[0-9]+[\.]?[0-9]*\b", 0, Part.Numeric); // number, optional float, isolated

            // keywords
            Register(Constant.GetOrPattern(Constant.Keywords, isolated: true), 0, Part.Keyword);

            // controllers
            Register(Constant.GetOrPattern(Constant.Conditionals, isolated: true), 0, Part.Conditional);

            // identifiers: specific implications
            Register(@"define +(\w+)", 1, Part.Identifier);
            Register(@"set +(\w+)", 1, Part.Identifier);
            Register(@"(\w+) of type ", 1, Part.Identifier);
            Register(@"(\w+) array *\[\] of type ", 1, Part.Identifier);
            Register(@", +(\w+)", 1, Part.Identifier);
            Register(@"using (\w+)", 1, Part.Identifier);

            // methods
            Register(@"function +(\w+)", 1, Part.Method); // 
            Register(@"endfun +(\w+)", 1, Part.Method); // 
            Register(@"call (\w+)\b", 1, Part.Method); // 
            Register(@"call +\w+\.(\w+)", 1, Part.Method); // 
            Register(@"(\w+)\(.*\)", 1, Part.Method); // 

            // operators
            Register(Constant.GetOrPattern(Constant.Operators), 0, Part.Operator);

            // binary operators
            Register(Constant.GetOrPattern(Constant.BinaryOperators, isolated: true), 0, Part.BinaryOperator);

            // inert
            Register(@"\.|\(|\)|,|\[|\]", 0, Part.Inert); // all structural elements

            // identifiers: the rest
            Register(@"\w+", 0, Part.Identifier); // any other word
        }

        private static void Register(string pattern, int group, Part part)
        {
            // get specified color
            var color = GetColorFrom(part);

            // perform search
            var matches = Regex.Matches(Document.Source, pattern);

            // process each word
            foreach (Match match in matches)
            {
                // specified target
                var target = match.Groups[group];

                // create word
                Word word = new()
                {
                    Start = target.Index,
                    End = target.Index + target.Length,
                    Part = part
                };

                // add the word if there is space
                if (Document.Colors[word.Start] == null) 
                    Document.Words.Add(word);

                // fill table with color
                for (int index = word.Start; index < word.End; index++)
                    if (Document.Colors[index] == null) Document.Colors[index] = color;
            }
        }

        public static ConsoleColor GetColorFrom(Part part)
        {
            // assign each part a color
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
