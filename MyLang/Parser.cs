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

        static Dictionary<TokenType, Ast.SymbolType> SymbolType = new Dictionary<TokenType, Ast.SymbolType>
        {
            {TokenType.Assign, Ast.SymbolType.equal },
        };

        static Dictionary<TokenType, Ast.Keyword> KeywordType = new Dictionary<TokenType, Ast.Keyword>
        {
            {TokenType.Let, Ast.Keyword.let },
            {TokenType.Function, Ast.Keyword.function },
            {TokenType.Return, Ast.Keyword.return_ },
            {TokenType.Print, Ast.Keyword.print },
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
            pos_++;
        }

        void match(TokenType s)
        {
            var t = currentToken();
            if (t.Type == s)
                progress();
            else
                throw new Exception("match fail!!");
        }
    
        public Ast.Ast Parse(IList<Token> tokens)
        {
            tokens_ = tokens;
            pos_ = 0;
            return Baselist();
        }

        Ast.Baselist Baselist()
        {
            List<Ast.Ast> baselist = new List<Ast.Ast>();

            while (currentToken().Text != "[EOF]")
            {
                if (currentToken().Text != "[EOF]")
                {
                    baselist.Add(Base());
                }
                else
                {
                    break;
                }
            }
            return new Ast.Baselist(baselist);
        }

        Ast.Stat Base()
        {
            var c = currentToken();
            var cType = c.Type;
            var cKeyWork = KeywordType[cType];
            switch (cType)
            {
                case TokenType.Let:
                    progress();
                    var id = start();
                    match(TokenType.Assign);
                    var num = start();
                    match(TokenType.Semicolon);
                    return new Ast.Base(cKeyWork,id, num);
                case TokenType.Print:
                    progress();
                    var value = start();
                    match(TokenType.Semicolon);
                    return new Ast.Base(cKeyWork,value);
                case TokenType.Return:
                    progress();
                    var exp = start();
                    match(TokenType.Semicolon);
                    return new Ast.Base(cKeyWork, exp);
                case TokenType.Function:
                    throw new Exception("Function fail!!");
                default:
                    throw new Exception("Base fail!!");
            }

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
        //  expr ::= term  ( (+ | - ) term  )...
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
        //
        // term->term x factor|term/factor|factor
        // 
        // term ->   factor ( (*|/) factor  )...
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
                var right = p_value();
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

        //parser Number
        Ast.Exp p_value()
        {
            var t = currentToken();
            if (t.IsNumber)
            {
                progress();
                return new Ast.Number(float.Parse(t.Text));
            }
            else if (t.IsVariable)
            {
                progress();
                return new Ast.Variable(t.Text);
            }
            else
            {
                throw new Exception("p_value BUG");
            }
        }

    }
}

#if false

// Javascript 
function add(a,b)
{
  return a + b;
}

// Language C

// Function definition
int add(int a, int b) {
  return a + b;
}

int add;   // Variable definition 

#endif