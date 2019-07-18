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
            { "let",TokenType.Let},
            { "return",TokenType.Return},
            { "print",TokenType.Print},
            { "Function",TokenType.Function},
            { ";",TokenType.Semicolon},
        };

        public SimpleTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            return Reader(src).Concat(new[] { new Token(TokenType.Terminate, "[EOF]") }).ToList();
        }

        List<Token> Reader(string str)
        {
            var currPos = 0;
            var dataLenght = str.Length;
            var currToken = "";
            var fin_Token = new List<Token>();
            while (currPos < dataLenght)
            {
                var data = str.Substring(currPos, 1);
                TokenType tt;
                if (SpacePattern.IsMatch(data))
                {
                    currPos++;
                }
                else if (TokenMatch.TryGetValue(data, out tt)) //Single Symbol
                {
                    fin_Token.Add(new Token(tt, data));
                    currPos++;
                }
                else if (NumberPattern.IsMatch(data))        //Number
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
                else if (SymbolPattern.IsMatch(data))        //Multi-Symbol
                {
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
                    fin_Token.Add(new Token(TokenType.Symbol, currToken));
                    currToken = "";
                }
                else if (VariablePattern.IsMatch(data))        //Variable
                {
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
                else
                {
                    throw new Exception("no token tyep");
                }

            }
            return fin_Token;
        }
    }
}

