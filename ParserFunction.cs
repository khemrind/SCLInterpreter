using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public static partial class Parser
    {
        /// <summary>
        /// Contains all of the logic necessary to construct a sentence. Throws an error if it fails to finish a sentence.
        /// </summary>
        /// <param name="word">The first word in the sentence.</param>
        public static void ConstructRecord()
        {
            if (LimitReached) return;

            Console.WriteLine($"Index {Index} <{CurrentWord.Part}>");
            Console.WriteLine($"<{CurrentValue}>");

            //
            ReturnsNode returns_node = CurrentWord.Part switch
            {
                Part.Keyword => () =>
                {
                    if (CurrentValue == "import") return new Import();
                    else if (CurrentValue == "description") return new Description();
                    else return new Node();
                },

                // default case: skip using generic node
                _ => () => new Node(),
            };

            returns_node().Construct();
            ConstructRecord();
        }

        delegate Node ReturnsNode();

        public class Node
        {
            public string Code { get; set; } = null;
            public string Value { get; set; } = null;
            public List<Node> Content { get; } = new();

            public virtual void Construct()
            {
                // process start
                // construct content
                // finish sentence
                // out the beginning of the next sentence

                Console.WriteLine("skipping..\n");
                Index++;
            }

            public virtual void Evaluate()
            {
                // create intermediate code and throw errors
            }
        }

        public class Import : Node
        {
            public override void Construct()
            {
                Content.Add(CurrentAsLeaf);
                Console.WriteLine("added import");

                if (NextWord.Part == Part.Literal)
                {
                    Index++;
                    Content.Add(CurrentAsLeaf);
                    Console.WriteLine("added a literal");
                }

                Record.Add(this);
                Index++;

                Console.WriteLine(JsonConvert.SerializeObject(Content, Formatting.Indented));
                Console.WriteLine();
            }
        }

        public class Description : Node
        {
            public override void Construct()
            {
                Content.Add(CurrentAsLeaf);
                Console.WriteLine("added description");

                if (NextWord.Part == Part.Comment)
                {
                    Index++;
                    Content.Add(CurrentAsLeaf);
                    Console.WriteLine("added a comment");
                }

                Record.Add(this);
                Index++;

                Console.WriteLine(JsonConvert.SerializeObject(Content, Formatting.Indented));
                Console.WriteLine();
            }
        }

    }
}
