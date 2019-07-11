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
    class SpaceSeparatedTokenizer : ITokenizer
    {
        static Regex regexNumber = new Regex(@"^\d+$");
        static Regex regexSpace = new Regex(@"\s+");
        public SpaceSeparatedTokenizer()
        {

        }

        Token TokenMatch(string s)
        {
            switch (s)
            {
                case "+":
                    return new Token(TokenType.Plus, s);
                    
                case "-":
                    return new Token(TokenType.Minus, s);
                    
                case "*":
                    return new Token(TokenType.Star, s);
                    
                case "/":
                    return new Token(TokenType.Slash, s);
                    
                default:
                    if (regexNumber.IsMatch(s))
                    {
                        return new Token(TokenType.Number, s);
                    }
                    else
                    {
                        return new Token(TokenType.Terminate, "!!no this token!!");
                    }
            }
        }

        public IList<Token> Tokenize(string src)
        {
            var String_split= regexSpace.Split(src);
            return String_split.Select(x => TokenMatch(x)).Concat(new[] { new Token(TokenType.Terminate, "[EOF]") }).ToArray();
        }
    }
}
