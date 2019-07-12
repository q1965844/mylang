using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLang
{
    public class Parser
    {
        IList<Token> tokens_;
        int pos_ = 0;

        static Dictionary<TokenType, Ast.BinOpType> BinOpMap = new Dictionary<TokenType, Ast.BinOpType>
        {
            {TokenType.Plus, Ast.BinOpType.Add },
            {TokenType.Minus, Ast.BinOpType.Sub },
            {TokenType.Star, Ast.BinOpType.Multiply },
            {TokenType.Slash, Ast.BinOpType.Divide },
        };

        public Parser()
        {

        }

        /// <summary>
        /// 現在のトークンを取得する
        /// </summary>
        /// <returns></returns>
        Token currentToken()
        {
            return tokens_[pos_];
        }

        /// <summary>
        /// 次のトークンに進む
        /// </summary>
        void progress()
        {
            Logger.Trace($"progress {currentToken().Text}");
            pos_++;
        }

        public Ast.Ast Parse(IList<Token> tokens)
        {
            tokens_ = tokens;
            pos_ = 0;
            return Start();
        }

        Ast.Exp Start()
        {
            
            var lhs = parseMultiply();
            if (parseMultiply() == null)
            {
                return null;
            }
            return parseAdd(lhs);
        }

        Ast.Exp parseMultiply()
        {
            var lhs = parseNumber();
            if (lhs == null)
            {
                return null;
            }
            return MultiplyCombine(lhs);
         }

        Ast.Exp MultiplyCombine(Ast.Exp lhs)
        {
            var b = currentToken();
            if (b.Type == TokenType.Star || b.Type == TokenType.Slash)
            {
                var binopType = BinOpMap[b.Type];
                progress();
                var rhs = parseNumber();
                if (rhs == null)
                {
                    throw new Exception("NO rhs");
                }
                var exp =new Ast.BinOp(binopType, lhs, rhs);
                return MultiplyCombine(exp);
            }
            else
            {
                return lhs;
            }
        }

        Ast.Exp parseAdd(Ast.Exp lhs)
        {
            var b = currentToken();
            if (b.Type == TokenType.Plus || b.Type == TokenType.Minus)
            {
                var binopType = BinOpMap[b.Type];
                progress();
                var rhs = parseMultiply();
                if (rhs == null)
                {
                    throw new Exception("NO rhs");
                }
                var exp = new Ast.BinOp(binopType, lhs, rhs);
                return parseAdd(exp);
            }
            else
            {
                return lhs;
            }
        }

        Ast.Exp parseNumber()
        {
            var n = currentToken();
            if (n.IsNumber)
            {
                progress();
                return new Ast.Number(float.Parse(n.Text));
            }
            else
            {
                return null;
            }
        }

    }
}