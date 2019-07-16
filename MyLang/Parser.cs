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
            return start();
        }

        //parser start
        Ast.Exp start()
        {
            var left = p_multiply(p_value());
            return p_add(left);
        }

        //parser "+" & "-"
        // BNF: expr ->   expr (+ | - ) term | term
        //
        //  expr ::= term  ( (+ | - ) expr  )...
        Ast.Exp p_add(Ast.Exp left)
        {
            if (left == null)
            {
                return null;
            }
            var m = currentToken();
            if (m.Type == TokenType.Plus || m.Type == TokenType.Minus)
            {
                progress();
                var right = p_multiply(p_value());
                if (right == null)
                {
                    throw new Exception("no  right");
                }
                var exp = new Ast.BinOp(BinOpMap[m.Type], left, right);
                return p_add(exp);
            }
            else
            {
                return left;
            }
        }

        //parser "*" & "/"
        Ast.Exp p_multiply(Ast.Exp left)
        {
            if (left == null)
            {
                return null;
            }
            var m = currentToken();
            if (m.Type == TokenType.Star || m.Type == TokenType.Slash)
            {
                progress();
                var right = p_multiply(p_value());
                if (right == null)
                {
                    throw new Exception("no  right");
                }
                var exp = new Ast.BinOp(BinOpMap[m.Type], left, right);
                return p_multiply(exp);
            }
            else
            {
                return left;
            }
        }

        Ast.Exp p_value()
        {
            var t = currentToken();
            if (t.IsNumber)
            {
                progress();
                return new Ast.Number(float.Parse(t.Text));
            }
            else
            {
                return null;
            }
        }

    }
}