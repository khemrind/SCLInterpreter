using System;
using System.Linq;

namespace Interpreter
{
    public static partial class Parser
    {
        public static Document Document { get => Program.Document; }

        public static Node Tree { get; set; } = new();

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

        private static Node Skip()
        {
            Console.WriteLine("skipping..\n");
            Index++;
            return new();
        }

        private static void SkipNewlines()
        {
            while (Next.Value == "\n") Index++;
        }

        private static void Assert(bool condition, string error)
        {
            if (condition) Index++;
            else throw new Exception(error);
        }

        private static int GetLineNumber(int position)
        {
            return Document.Source.Take(position).Count(item => item == '\n') + 1;
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
            public static string Indent { get => CreateIndent(ScopeLevel); }

            private static string CreateIndent(int level)
            {
                string indent = "";
                for (int index = level; index > 0; index--)
                    indent += Tab;

                return indent;
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

    // define node
    public class Node : List<object>
    {
        public string Code { get; set; }
    }

    // define a node constructor function
    delegate Node ConstructNode();
}
