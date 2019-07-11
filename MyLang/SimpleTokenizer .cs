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
        static Regex SpacePattern = new Regex(@"\data+");
        public SimpleTokenizer()
        {

        }

        public IList<Token> Tokenize(string src)
        {
            return Reader(src).Concat(new[] { new Token(TokenType.Terminate, "[EOF]") }).ToArray();
        }

        List<Token> Reader(string str)
        {
            var currPos = 0;
            //var data = str.Substring(currPos, 1);
            var dataLenght = str.Length;
            var currToken ="" ;
            var fin_Token = new List<Token>();
            while (currPos <dataLenght)
            {
                var data = str.Substring(currPos, 1);
                switch (data)
                {
                    case "+":
                        fin_Token.Add(new Token(TokenType.Plus, data));
                        currPos++;
                        break;
                    case "-":
                        fin_Token.Add(new Token(TokenType.Minus, data));
                        currPos++;
                        break;
                    case "*":
                        fin_Token.Add(new Token(TokenType.Star, data));
                        currPos++;
                        break;
                    case "/":
                        fin_Token.Add(new Token(TokenType.Slash, data));
                        currPos++;
                        break;
                    default:
                        if (NumberPattern.IsMatch(data))
                        {
                            while (NumberPattern.IsMatch(data))
                            {
                                currToken +=data;
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
                        else if (SpacePattern.IsMatch(data))
                        {
                            currPos++;
                        }
                        else
                        {
                            fin_Token.Add(new Token(TokenType.Terminate, "!!no this token!!"));
                            currPos++;
                        }
                        break;
                }
            }
            return fin_Token;
        }
    }
}

