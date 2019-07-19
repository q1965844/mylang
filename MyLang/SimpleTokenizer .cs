using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace MyLang
{
    /// <summary>
    /// 単純なトークナイザ(tokenizer)
    /// 
    /// トークンは、必ず一つ以上のスペースで区切られている必要がある
    /// </summary>
    class SimpleTokenizer : ITokenizer
    {
        static Regex NumberPattern = new Regex(@"^\d+$");
        static Regex SpacePattern = new Regex(@"\s+");
        static Regex VariablePattern = new Regex(@"[A-Za-z_]\w*");
        static Regex SymbolPattern = new Regex(@"\W+");

        static Dictionary<string, TokenType> TokenMatch = new Dictionary<string, TokenType>
        {
            { "+",TokenType.Plus},
            { "-",TokenType.Minus},
            { "*",TokenType.Star},
            { "/",TokenType.Slash},

            { "=",TokenType.Assign},
            { ";",TokenType.Semicolon},
            { "(",TokenType.LeftParen},
            { ")",TokenType.RightParen},
            { "{",TokenType.LeftBrace},
            { "}",TokenType.RightBrace},
            { ",",TokenType.Comma},

            { "let",TokenType.Let},
            { "return",TokenType.Return},
            { "print",TokenType.Print},
            { "function",TokenType.Function},
            { "//",TokenType.Double_slash},
            { "/*",TokenType.Slash_star},
            { "*/",TokenType.Star_slash},
        };

        public SimpleTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            return Reader(src).Concat(new[] { new Token(TokenType.Terminate, "[EOF]") }).ToList();
        }

        public List<Token> Reader(string str)
        {
            var currPos = 0;
            var dataLenght = str.Length;
            var currToken = "";
            var fin_Token = new List<Token>();
            while (currPos < dataLenght)
            {
                var data = str.Substring(currPos, 1);
                if (SpacePattern.IsMatch(data))
                {
                    currPos++;
                }
                else if (TokenMatch.ContainsKey(data))      //Single Symbol
                {
                    fin_Token.Add(new Token(TokenMatch[data], data));
                    currPos++;
                }
                else if (NumberPattern.IsMatch(data))        //Number
                {
                    Match_Number(data);
                }
                else if (SymbolPattern.IsMatch(data))        //Multi-Symbol
                {
                    Match_MultiSymbol(data);
                }
                else if (VariablePattern.IsMatch(data))        //Variable
                {
                    Match_Variable(data);
                }
                else
                {
                    throw new Exception("no tokentyep");
                }
            }
            #region All Match void
            
            void Match_Number(string data)
            {
                while (NumberPattern.IsMatch(data))
                {
                    currToken += data;
                    currPos++; 
                    if (currPos >= dataLenght || !NumberPattern.IsMatch(data))
                    {
                        break;
                    }
                    data = str.Substring(currPos, 1);
                }
                fin_Token.Add(new Token(TokenType.Number, currToken));
                currToken = "";
            }

            void Match_MultiSymbol(string data)
            {
                TokenType tt;
                while (SymbolPattern.IsMatch(data))
                {
                    currToken += data;
                    currPos++;
                    if (currPos >= dataLenght || !SymbolPattern.IsMatch(data))
                    {
                        break;
                    }
                    data = str.Substring(currPos, 1);
                }
                if (TokenMatch.TryGetValue(currToken, out tt))
                    fin_Token.Add(new Token(tt, currToken));
                else
                    fin_Token.Add(new Token(TokenType.Symbol, currToken));
                currToken = "";
            }

            void Match_Variable(string data)
            {
                TokenType tt;
                while (VariablePattern.IsMatch(data))
                {
                    currToken += data;
                    currPos++;
                    if (currPos >= dataLenght || !VariablePattern.IsMatch(data))
                    {
                        break;
                    }
                    data = str.Substring(currPos, 1);
                }
                if (TokenMatch.TryGetValue(currToken, out tt))
                    fin_Token.Add(new Token(tt, currToken));
                else
                    fin_Token.Add(new Token(TokenType.Variable, currToken));
                currToken = "";
            }
            #endregion
            return fin_Token;
        }

        
    }
}

