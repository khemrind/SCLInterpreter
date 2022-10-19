using System;
using System.Diagnostics;
using System.Linq;

namespace Interpreter
{
    public static partial class Parser
    {
        public static Document Document { get => Program.Document; }

        // generated code
        public static string Code { get; set; } = "";

        // processing index
        private static int Index { get; set; } = 0;

        // indexed word properties
        private static Word Current { get => Document.Words[Index]; }
        private static Word Next { get => Document.Words[Index + 1]; }
        private static bool LimitReached { get => Index >= Document.Words.Count; }

        // scope level
        private static int ScopeLevel { get; set; } = 0;

        #region Helper Members

        private const string Tab = "    ";

        private static void Assert(bool condition, string process, string expectation)
        {
            Console.WriteLine(Scope.GetIndent(ScopeLevel - 1) + $"Parsing <{process}> expecting: {expectation}");

            if (condition) Index++;
            else
            {
                var line = GetLineNumber();
                throw new ParseException($"At {process}: expected {expectation}. (line {line})") { LineNumber = line };
            }
        }

        private static int GetLineNumber()
        {
            return Document.Source.Take(Current.End).Count(item => item == '\n') + 1;
        }

        private static void RemoveComments()
        {
            // create removal list
            var to_remove = new List<Word>();
            var enumerator = Document.Words.AsEnumerable().Reverse();
            bool matching_whitespace = false;

            // iterate in reverse
            foreach (var word in enumerator)
            {
                // mark leading whitespace
                if (matching_whitespace && word.Value != "," && word.Part == Part.Structural)
                {
                    to_remove.Add(word);
                }
                else matching_whitespace = false;

                // mark comment 
                if (word.Part == Part.Comment)
                {
                    to_remove.Add(word);
                    matching_whitespace = true;
                }
            }

            Document.Words.RemoveAll(item => to_remove.Contains(item));
        }

        private class Scope : IDisposable
        {
            public Scope() => ScopeLevel++;
            public void Dispose() => ScopeLevel--;
            public static string Indent { get => GetIndent(ScopeLevel); }

            public static string GetIndent(int level)
            {
                if (level == 1) return "    ";
                else if (level == 2) return "        ";
                else if (level == 3) return "            ";

                return "";
            }
        }

        public static bool IsElement(this Word word)
        {
            return word.Part == Part.Numeric
                || word.Part == Part.Literal
                || word.Part == Part.Identifier;
        }

        #endregion
    }

    public class ParseException : Exception
    {
        public ParseException() : base() { }
        public ParseException(string message) : base(message) { }

        public int LineNumber { get; set; }
    }

}
