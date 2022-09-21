using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter
{
    public static class Constant
    {
        public static readonly string[] Keywords =
        {
            "import", "symbol", "forward", 
            "specifications", "references", 
            "function", "declarations", 
            "variables", "global",
            "implementations", "main", 
            "parameters", "constant", 
            "begin", "endfun", 
            "display", "set", "return", 
            "define", "of", "type", 
            "array", "struct", "pointer",
            "enum", "call", "exit", "increment",
            "is"
        };

        public static readonly string[] Controllers =
        {
            "repeat", "until", "endrepeat",
            "if", "then", "else", "endif",
            "while", "endwhile"
        };

        public static readonly string[] Operators =
        {
            "+", "-", "=",
            "+=", "-=", "++", "--",
            ":", "*", "/", "%", "==", 
            "!=", ">", "<", 
            ">=", "<=", "and", "or"
        };

        // get keywords
        // identifiers
        // operators
        // constants

        public static string GetOrPattern(string[] tokens)
        {
            string pattern = "";
            for (int index = 0; index < tokens.Length - 1; index++)
                pattern += tokens[index] + "|";
            return pattern + tokens.Last();
        }

    }

    
}
