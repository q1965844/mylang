using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    /// <summary>
    /// トークンの種類
    /// </summary>
    public enum TokenType 
    {
        Plus,   // "+"
        Minus,  // "-"
        Star,   // "*"
        Slash,  // "/"

        Assign,     // "="
        Semicolon,  // ";"
        LeftParen,  // "("
        RightParen, // ")"
        LeftBrace,  // "{"
        RightBrace, // "}"
        Comma,      // ","

        Number,  // 数値
        Symbol,  // 識別子 "<,>, : "
        Variable,// 变数

        Let,         // "let"
        Print,       // "print"
        Return,      // "return"
        Function,    // "function"
        Double_slash,   // '//'
        Slash_star,     // '/*'
        Star_slash,     // '*/'


        Terminate, // ソースの終わりを表す
    }

    /// <summary>
    /// トークン
    /// </summary>
    public class Token
    {
        public readonly TokenType Type;
        public readonly string Text;

        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;
        }

        public bool IsTerminate => (Type == TokenType.Terminate);
        public bool IsNumber => (Type == TokenType.Number);
        public bool IsVariable => (Type == TokenType.Variable);
        public bool IsSymbol => (Type == TokenType.Symbol);
        public bool IsAssign => (Type == TokenType.Assign);
        public bool IsBinaryOperator => (Type == TokenType.Plus || Type == TokenType.Minus || Type == TokenType.Star || Type == TokenType.Slash);
    }

    public interface ITokenizer
    {
        /// <summary>
        /// ソースコードをトークンに分割する
        /// </summary>
        /// <param name="src">ソースコード</param>
        /// <returns>トークンのリスト</returns>
        IList<Token> Tokenize(string src);
    }
}
