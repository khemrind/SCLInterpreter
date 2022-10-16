using System;
using System.Linq;

namespace Interpreter
{
    public static partial class Parser
    {
        public static Document Document { get => Program.Document; }

        public static Node ParseTree { get; set; } = new();

        // processing index
        private static int Index { get; set; } = 0;

        // indexed word properties
        private static Word Current { get => Document.Words[Index]; }
        private static Word Next { get => Document.Words[Index + 1]; }
        private static bool LimitReached { get => Index >= Document.Words.Count; }

        delegate Node ReturnsNode();

        public static void Process()
        {
            // sort the words by placement in text
            Document.Words.Sort((word, next_word) => word.Start.CompareTo(next_word.Start));

            //    if ((word.Value.Contains("    ") && word.Value.Length == 4) || 
            //        (word.Value.Contains("    ") && word.Value.Length == 8))

            while (LimitReached == false)
            {
                Console.WriteLine($"Index {Index} <{Current.Part}>");
                Console.WriteLine($"<{Current.Value}>");

                ReturnsNode returns_node = Current.Part switch // !! throw in function instead, use void methods
                {
                    Part.Keyword => () =>
                    {
                        if (Current.Value == "description") return ConstructDescription();
                        if (Current.Value == "global" && Next.Value == "declarations") return ConstructDeclarations();
                        else return Skip();
                    },

                    // default case: skip using generic node
                    _ => Skip,
                };

                var node = returns_node();
                if (node == null)
                    throw new Exception("ey bro wtf");
                else ParseTree.Add(node);
            }
        }

        public class Node : List<object>
        {
            public string Code { get; set; }
        }

        private static void SkipNewlines()
        {
            while (Next.Value == "\n") Index++;
        }

        private static void Assert___(Predicate<Word> is_valid)
        {
            if (is_valid(Next))
            {
                Index++;
                // node.add(Current)
            }

            else throw new Exception("issue at this type of construction, expected this token");
        }

        private static Node Skip()
        {
            Console.WriteLine("skipping..\n");
            Index++;
            return new();
        }

        private static Node ConstructDescription()
        {
            // description
            var node = new Node { Current };

            // newline
            if (Next.Value != "\n") return null;
            Index++;

            // comment
            if (Next.Part != Part.Comment) return null;
            Index++;
            node.Add(Current);

            Index++;
            return node;
        }

        private static Node ConstructDeclarations()
        {
            // global declarations
            var node = new Node { Current, Next };
            Index += 2;

            // newlines
            SkipNewlines();

            // constants
            if (Current.Value != "constants") return null;
            node.Add(ConstructConstants());

            Index++;
            return node;
        }

        private static Node ConstructConstants()
        {
            // constants
            var node = new Node { Current };

            // statements
            while (true)
            {
                if (Next.Value != "define") return null;
                Index++;
                node.Add(ConstuctDefine());
            }
        }

        private static Node ConstuctDefine()
        {
            // define
            var node = new Node { Current };

            // identifier
            if (Next.Part != Part.Identifier) return null;
            Index++;
            node.Add(Current);

            // equals
            if (Next.Value != "=") return null;
            Index++;
            node.Add(Current);

            // value
            if (Next.Part != Part.Numeric || Next.Part != Part.Literal) return null;
            Index++;
            node.Add(Current);

            // of type
            if (Next.Value != "of") return null;
            Index++;
            node.Add(Current);
            if (Next.Value != "type") return null;
            Index++;
            node.Add(Current);

            // 

        }

        


        //// parse tree
        //public static List<Node> Record { get; set; } = new();

        //// processing index
        //public static int Index { get; set; } = 0;
        //public static bool LimitReached { get => Index >= Document.Words.Count; }

        //// current index properties
        //public static Word CurrentWord { get => Document.Words[Index]; }
        //public static string CurrentValue { get => GetWordValue(Index); }
        //public static Node CurrentAsLeaf { get => new() { Value = CurrentValue }; }

        //// next index properties
        //public static Word NextWord { get => Document.Words[Index + 1]; }
        //public static string NextValue { get => GetWordValue(Index + 1); }

        //public static void Process()
        //{
        //    Document.Words.Sort((word, next_word) => word.Start.CompareTo(next_word.Start));

        //    ConstructRecord();
        //}

        //private static string GetWordValue(int index)
        //{
        //    var word = Document.Words[index];
        //    return Document.Source[word.Start..word.End];
        //}
    }
}
