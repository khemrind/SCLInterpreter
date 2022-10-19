using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            // begin class
            Code += $"public static class {Document.Name}\n{{\n";
            var scope = new Scope();

            while (LimitReached == false)
            {
                if (Current.Part == Part.Keyword)
                {
                    if (Current.Value == "constants") ConstructConstants();
                    else if (Current.Value == "function") ConstructFunction();
                    else Index++;
                }

                else Index++;
            }

            // end class
            scope.Dispose();
            Code += "}";
        }

        private static string ConstructExpression()
        {
            // element
            List<string> tokens = new() { Current.Value };

            // optional connecting elements
            while (true)
            {
                // connecting operator
                if (Next.Part == Part.Operator)
                {
                    Index++;
                    tokens.Add(Current.Value);
                    // element
                    if (IsElement(Next))
                    {
                        Index++;
                        tokens.Add(Current.Value);
                    }
                }

                else break;
            }

            // construct code
            return string.Join(" ", tokens);
        }

        private static string ConstructBoolExpression()
        {
            // element
            List<string> tokens = new() { Current.Value };

            // optional connecting elements
            while (true)
            {
                // connecting operator
                if (Next.Value == ">" || Next.Value == "<" || Next.Value == "==")
                {
                    Index++;
                    tokens.Add(Current.Value);
                    // element
                    if (IsElement(Next))
                    {
                        Index++;
                        tokens.Add(Current.Value);
                    }
                }

                else break;
            }

            // construct code
            return string.Join(" ", tokens);
        }

        private static string ConstructStringExpression()
        {
            // element
            List<string> tokens = new() { Current.Value };

            // optional connecting elements
            while (true)
            {
                // connecting operator
                if (Next.Value == ",")
                {
                    Index++;
                    // element
                    if (IsElement(Next))
                    {
                        Index++;
                        tokens.Add(Current.Value);
                    }
                }

                else break;
            }

            List<string> string_tokens = new();
            foreach (var token in tokens)
            {
                if (token.Contains('"')) string_tokens.Add(token);
                else string_tokens.Add($"({token}).ToString()");
            }

            // construct code
            return string.Join(" + ", string_tokens);
        }

        private static string ConstructConstant()
        {
            // identifier
            Assert(Next.Part == Part.Identifier, "constant", "identifier");
            var identifier = Current.Value;

            // equals
            Assert(Next.Value == "=", "constant", "equals sign");

            // expression
            Assert(Next.IsElement(), "constant", "element");
            var expression = ConstructExpression();

            // of
            Assert(Next.Value == "of", "constant", "of");

            // type
            Assert(Next.Value == "type", "constant", "type");

            // typename
            Assert(Next.Part == Part.Type, "constant", "typename");
            var typename = Current.Value == "integer" ? "int" : Current.Value;

            // newline
            Assert(Next.Value == "\n", "constant", "newline");

            // construct code
            return Scope.Indent + "const " + typename + " " + identifier + " = " + expression + ";";
        }

        private static void ConstructConstants()
        {
            // constants matched

            // newline
            Assert(Next.Value == "\n", "constants", "newline");

            // statements
            while (true)
            {
                // tab
                if (Next.Value != Tab) break;
                Index++;

                // identifier
                if (Next.Value != "define") break;
                Index++;

                // add statement
                Code += ConstructConstant() + "\n";
            }
        }

        private static string ConstructParameter()
        {
            // identifier
            var identifier = Current.Value;

            // of
            Assert(Next.Value == "of", "parameter", "of");

            // type
            Assert(Next.Value == "type", "parameter", "type");

            // typename
            Assert(Next.Part == Part.Type, "parameter", "typename");
            var typename = Current.Value == "integer" ? "int" : Current.Value;

            // newline
            Assert(Next.Value == "\n", "parameter", "newline");

            return typename + " " + identifier;
        }

        private static string ConstructParameters()
        {
            List<string> parameters = new();

            // parameters matched

            // newline
            Assert(Next.Value == "\n", "parameters", "newline");

            while (true)
            {
                // tab
                if (Next.Value != Tab) break;
                Index++;

                // identifier
                if (Next.Part != Part.Identifier) break;
                Index++;

                // add statement
                parameters.Add(ConstructParameter());
            }

            return string.Join(", ", parameters);
        }

        private static string ConstructVariable()
        {
            // define matched

            // identifier
            Assert(Next.Part == Part.Identifier, "variable", "identifier");
            var identifier = Current.Value;

            // of
            Assert(Next.Value == "of", "variable", "of");

            // type
            Assert(Next.Value == "type", "variable", "type");

            // typename
            Assert(Next.Part == Part.Type, "variable", "typename");
            var typename = Current.Value == "integer" ? "int" : Current.Value;

            // newline
            Assert(Next.Value == "\n", "variable", "newline");

            return Scope.Indent + typename + " " + identifier + ";";
        }

        private static void ConstructVariables()
        {
            // variables matched

            // newline
            Assert(Next.Value == "\n", "variables", "newline");

            // statements
            while (true)
            {
                // tab
                if (Next.Value != Tab) break;
                Index++;

                // identifier
                if (Next.Value != "define") break;
                Index++;

                // add statement
                Code += ConstructVariable() + "\n";
            }
        }

        private static void ConstructFunction()
        {
            // function matched

            // identifier
            Assert(Next.Part == Part.Method || Next.Value == "main", "function", "method name");
            var method = (Current.Value == "main") ? "Main" : Current.Value;

            // check return type
            string return_type = "void";
            if (Next.Value == "return")
            {
                Index++;

                // type
                Assert(Next.Value == "type", "function", "type");

                // typename
                Assert(Next.Part == Part.Type, "variable", "typename");
                return_type = Current.Value == "integer" ? "int" : Current.Value;
            }

            // is
            Assert(Next.Value == "is", "function", "is");

            // newline
            Assert(Next.Value == "\n", "function", "newline");

            // parameters
            string parameters = "";
            if (Next.Value == "parameters")
            {
                Index++;
                parameters = ConstructParameters();
            }

            // function declaration
            Code += Scope.Indent + $"public static {return_type} {method}({parameters})\n"
                + Scope.Indent + "{\n";

            // enter function scope
            var scope = new Scope();

            // variables
            if (Next.Value == "variables")
            {
                Index++;
                ConstructVariables();
            }

            // begin
            Assert(Next.Value == "begin", "function", "begin");

            // newline
            Assert(Next.Value == "\n", "function", "newline");

            // body
            ConstructBody();

            // endfun
            Assert(Next.Value == "endfun", "function", "endfun");
            scope.Dispose();

            // newline
            Assert(Next.Value == "\n", "function", "newline");

            Code += Scope.Indent + "}\n";
        }

        private static void ConstructWhileLoop()
        {
            // while matched

            // bool expression
            Assert(Next.IsElement(), "while", "element");
            string expression = ConstructBoolExpression();

            // do
            Assert(Next.Value == "do", "while", "do");

            // newline
            Assert(Next.Value == "\n", "while", "newline");

            // declare while
            Code += Scope.Indent + $"while ({expression})\n"
                + Scope.Indent + "{\n";

            // enter loop scope
            var scope = new Scope();

            // construct a body
            ConstructBody();

            // endwhile
            scope.Dispose();
            Assert(Next.Value == "endwhile", "while", "endwhile");

            // newline
            Assert(Next.Value == "\n", "while", "newline");

            Code += Scope.Indent + "}\n";
        }

        private static void ConstructDisplay()
        {
            // display matched

            // string expression
            Assert(Next.IsElement(), "display", "element");
            string expression = ConstructStringExpression();

            // newline
            Assert(Next.Value == "\n", "display", "newline");

            Code += Scope.Indent + "Console.WriteLine(" + expression + ");\n";
        }

        private static void ConstructAssignment()
        {
            // set matched

            // identifier
            Assert(Next.Part == Part.Identifier, "set", "identifier");
            var identifier = Current.Value;

            // equals
            Assert(Next.Value == "=", "set", "equals sign");

            // expression
            Assert(Next.IsElement(), "set", "element");
            string expression = ConstructExpression();

            // newline
            Assert(Next.Value == "\n", "set", "newline");

            Code += Scope.Indent + identifier + " = " + expression + ";\n";
        }

        private static void ConstructCall()
        {
            // call matched

            // method
            Assert(Next.Part == Part.Method, "call", "method name");
            var method = Current.Value;

            // using
            string arguments = "";
            if (Next.Value == "using")
            {
                Index++;
                // expression
                Assert(Next.IsElement(), "call", "element");
                arguments = ConstructExpression();
            }

            // newline
            Assert(Next.Value == "\n", "call", "newline");

            Code += Scope.Indent + $"{method}({arguments});\n";
        }

        private static void ConstructReturn()
        {
            // return matched

            // expression
            string expression = "";
            if (Next.IsElement())
            {
                Index++;
                expression = ConstructExpression();
            }

            // newline
            Assert(Next.Value == "\n", "return", "newline");

            Code += Scope.Indent + "return " + expression + ";\n";
        }

        private static void ConstructBody()
        {
            while (true)
            {
                if (Next.Value.Contains(Tab) == false) break;
                Index++;

                if (Next.Value == "display")
                {
                    Index++;
                    ConstructDisplay();
                }
                else if (Next.Value == "call")
                {
                    Index++;
                    ConstructCall();
                }
                else if (Next.Value == "set")
                {
                    Index++;
                    ConstructAssignment();
                }
                else if (Next.Value == "return")
                {
                    Index++;
                    ConstructReturn();
                }

                else if (Next.Value == "while")
                {
                    Index++;
                    ConstructWhileLoop();
                }

                else break;
            }
        }
    }
}
