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
        static Regex regexNumber = new Regex(@"^\d$");
        static Regex regexSpace = new Regex(@"\s+");

        public SpaceSeparatedTokenizer()
        {

        }

        Token match(string s)
        {
            return new Token(TokenType.Number, "1")); ;
        }

        public IList<Token> Tokenize(string src)
        {
            var tokenString = regexSpace.Split(src);
            return tokenString.Select(x =>match(x));
            // TODO: 仮のダミー実装
            var dummy = new List<Token>();
            dummy.Add(new Token(TokenType.Number, "1"));
            dummy.Add(new Token(TokenType.Plus, "+"));
            dummy.Add(new Token(TokenType.Number, "2"));
            dummy.Add(new Token(TokenType.Terminate, "[EOF]"));
            return dummy;
        }

    }
}
