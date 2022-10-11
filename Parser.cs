using System;
using System.Linq;

namespace Interpreter
{
    public static partial class Parser
    {
        public static Document Document { get; set; }

        // parse tree
        public static List<Node> Record { get; set; } = new();

        // processing index
        public static int Index { get; set; } = 0;
        public static bool LimitReached { get => Index >= Document.Words.Count; }

        // current index properties
        public static Word CurrentWord { get => Document.Words[Index]; }
        public static string CurrentValue { get => GetWordValue(Index); }
        public static Node CurrentAsLeaf { get => new() { Value = CurrentValue }; }

        // next index properties
        public static Word NextWord { get => Document.Words[Index + 1]; }
        public static string NextValue { get => GetWordValue(Index + 1); }

        public static void Process(Document document)
        {
            document.Words.Sort((x, y) => x.Start.CompareTo(y.Start));
            Document = document;

            ConstructRecord();
        }

        private static string GetWordValue(int index)
        {
            var word = Document.Words[index];
            return Document.Source[word.Start..word.End];
        }
    }
}
