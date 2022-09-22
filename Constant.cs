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
            "variables", "global", "input",
            "implementations", "main", "definetype",
            "parameters", "constants", 
            "begin", "endfun", "create", "destroy",
            "display", "set", "return", 
            "define", "of", "type", "pointer",
            "array", "structures","endstruct", "struct", "pointer",
            "enumerate","endenum", "enum", "call", "exit", "increment",
            "is", "using", "description"
        };

        public static readonly string[] Conditionals =
        {
            "repeat", "until", "endrepeat",
            "if", "then", "else", "endif",
            "while", "endwhile","endfor", "for", "to", "do"
        };

        public static readonly string[] Operators =
        {
            "\\+", "-", "=",
            "\\+=", "-=", "\\+\\+", "--",
            ":", "\\*", "\\/", "%", "==", 
            "!=", ">", "<", "\\^",
            ">=", "<=", "\\band\\b", "\\bor\\b"
        };

        public static readonly string[] BinaryOperators =
        {
            "band", "bor", "bxand", "bxor", 
            "lshift", "rshift", "negate"
        };

        // get keywords
        // identifiers
        // operators
        // constants

        public static string GetOrPattern(string[] tokens, bool isolated = false)
        {
            string pattern = "";
            for (int index = 0; index < tokens.Length - 1; index++)
            {
                if (isolated) pattern += "\\b" + tokens[index] + "\\b" + "|";
                else pattern += tokens[index] + "|";
            }
                
            return pattern + tokens.Last();
        }

    }

    
}
