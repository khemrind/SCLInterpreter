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
        public static void Process()
        {
            // sort the words by placement in text
            Document.Words.Sort((word, next_word) => word.Start.CompareTo(next_word.Start));

            // remove comments
            RemoveComments();

            while (LimitReached == false)
            {
                Console.WriteLine($"Index {Index} <{Current.Part}>");
                Console.WriteLine($"<{Current.Value}>");

                ConstructNode constructNode = Current.Part switch
                {
                    Part.Keyword => () =>
                    {
                        if (Current.Value == "description") return ConstructDescription();
                        else if (Current.Value == "constants") return ConstructConstants();
                        else if (Current.Value == "parameters") return ConstructParameters();
                        else if (Current.Value == "variables") return ConstructVariables();
                        else return Skip();
                    }
                    ,

                    // default case: skip using generic node
                    _ => () =>
                    {
                        return Skip();
                    }
                    ,
                };

                // add the node
                Tree.Add(constructNode());
            }

            // construct code
            ConstructCode();
        }

        private static void ConstructCode()
        {
            // create class
            Tree.Code = $"public static class {Document.Name} {{\n";

            // enter class scope
            using var scope = new Scope();

            foreach (var item in Tree)
            {
                // add code
                if (item is Node node && node.Code != null)
                    Tree.Code += scope.Indent + node.Code + "\n";
            }
            Tree.Code += "}";
        }

        private static Node ConstructDescription()
        {
            // description
            var node = new Node { Current };

            // newline
            if (Next.Value != "\n") return null;
            Index++;

            // comment
            if (Next.Part != Part.Description) return null;
            Index++;
            node.Add(Current);

            Index++;
            return node;
        }

        private static Node ConstructParameters()
        {
            // parameters
            var node = new Node { Current };

            // newline
            if (Next.Value != "\n") return null;
            Index++;

            while (true)
            {
                // tab
                if (Next.Value == Tab)
                {
                    Index++;
                    // identifier
                    if (Next.Part == Part.Identifier)
                    {
                        Index++;
                        node.Add(ConstructParameter());
                    }
                }
                    
                else break;
            }

            Index++;
            return node;
        }

        private static Node ConstructParameter()
        {
            // identifier
            var node = new Node { Current };

            // of
            if (Next.Value != "of") return null;
            Index++;
            node.Add(Current);

            // type
            if (Next.Value != "type") return null;
            Index++;
            node.Add(Current);

            // typename
            if (Next.Part != Part.Type) return null;
            Index++;
            node.Add(Current);

            // newline
            if (Next.Value != "\n") return null;
            Index++;
            return node;
        }

        private static Node ConstructVariables()
        {
            // parameters
            var node = new Node { Current };

            // newline
            if (Next.Value != "\n") return null;
            Index++;

            // statements
            while (true)
            {
                // tab
                if (Next.Value == Tab)
                {
                    Index++;
                    // identifier
                    if (Next.Value == "define")
                    {
                        Index++;
                        node.Add(ConstructVariable());
                    }
                }

                else break;
            }

            // construct code
            node.Code = "";
            foreach (var item in node)
            {
                if (item is Node statement)
                {
                    node.Code += statement.Code + "\n";
                }
            }

            Index++;
            return node;
        }

        private static Node ConstructVariable()
        {
            // define
            var node = new Node { Current };

            // identifier
            if (Next.Part != Part.Identifier) return null;
            Index++;
            var identifier = Current.Value;
            node.Add(Current);

            // of
            if (Next.Value != "of") return null;
            Index++;
            node.Add(Current);

            // type
            if (Next.Value != "type") return null;
            Index++;
            node.Add(Current);

            // typename
            if (Next.Part != Part.Type) return null;
            Index++;
            var typename = (Current.Value == "integer") ? "int" : Current.Value;
            node.Add(Current);

            // newline
            if (Next.Value != "\n") return null;
            Index++;

            // construct code
            node.Code = $"{typename} {identifier};";

            return node;
        }

        private static Node ConstructExpression()
        {
            var node = new Node() { Current };

            // optional connecting elements
            while (true)
            {
                // connecting operator
                if (Next.Part == Part.Operator)
                {
                    Index++;
                    node.Add(Current);
                    // element
                    if (IsElement(Next))
                    {
                        Index++;
                        node.Add(Current);
                    }
                }

                else break;
            }

            // construct code
            node.Code = string.Join(" ", node);

            return node;
        }

        private static Node ConstructConstants()
        {
            // constants
            var node = new Node { Current };

            // statements
            while (true)
            {
                // tab
                if (Next.Value == Tab)
                {
                    Index++;
                    // identifier
                    if (Next.Value == "define")
                    {
                        Index++;
                        node.Add(ConstructConstant());
                    }
                }

                else break;
            }

            // construct code

            Index++;
            return node;
        }

        private static Node ConstructConstant()
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

            // expression
            if (Next.IsElement() == false) return null;
            Index++;
            node.Add(ConstructExpression());

            // of
            if (Next.Value != "of") return null;
            Index++;
            node.Add(Current);

            // type
            if (Next.Value != "type") return null;
            Index++;
            node.Add(Current);

            // typename
            if (Next.Part != Part.Type) return null;
            Index++;
            node.Add(Current);

            // newline
            if (Next.Value != "\n") return null;
            Index++;

            return node;
        }
    }
}
