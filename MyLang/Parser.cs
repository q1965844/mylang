using MyLang.Ast;
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

        static Dictionary<TokenType, Ast.Keyword> KeywordType = new Dictionary<TokenType, Ast.Keyword>
        {
            {TokenType.Let, Ast.Keyword.let },
            {TokenType.Variable, Ast.Keyword.variable },
            {TokenType.Function, Ast.Keyword.function },
            {TokenType.Return, Ast.Keyword.return_ },
            {TokenType.Print, Ast.Keyword.print },
        };
        static Dictionary<string, Ast.Base> function = new Dictionary<string, Ast.Base> { };

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

        bool match(TokenType s)
        {
            var t = currentToken();
            if (t.Type == s)
            {
                progress();
                return true;
            }
            else
            {
                return false;
                //throw new Exception("match error");
            }
        }

        public Ast.Ast Parse_Start(IList<Token> tokens)
        {
            tokens_ = tokens;
            pos_ = 0;
            return BaseList();
        }

        Ast.BaseList BaseList()
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
            return new Ast.BaseList(baselist);
        }

        Ast.Stat Base()
        {
            var c = currentToken();
            var cType = c.Type;
            var cKeyWork = KeywordType[cType];
            switch (cType)
            {
                case TokenType.If:
                    progress();
                    if (match(TokenType.LeftParen))
                    {
                        
                    }

                case TokenType.Let:
                    progress();
                    var id = start();
                    match(TokenType.Assign);
                    var num = start();
                    match(TokenType.Semicolon);
                    return new Ast.Base(cKeyWork, id, num);
                case TokenType.Variable:
                    var id2 = start();
                    if (match(TokenType.Assign))
                    {
                        var num2 = start();
                        match(TokenType.Semicolon);
                        return new Ast.Base(cKeyWork, id2, num2);
                    }
                    return new Ast.Base(cKeyWork, id2);
                case TokenType.Print:
                    progress();
                    var value = start();
                    var value_s = (Variable)value;
                    List<Ast.Ast> list2 = new List<Ast.Ast>();
                    var bb2 = new BaseList(list2);
                    if (function.ContainsKey(value_s.Str))
                    {
                        match(TokenType.LeftParen);
                        var fun = function[value_s.Str];
                        var cont = 0;
                        bb2.Baselist.Add(new Ast.Base(Keyword.variable, (Exp)fun.bl.Baselist[cont], start()));
                        while (match(TokenType.Comma))
                        {
                            cont++;
                            bb2.Baselist.Add(new Ast.Base(Keyword.variable, (Exp)fun.bl.Baselist[cont], start()));
                        }
                        match(TokenType.RightParen);
                        match(TokenType.Semicolon);
                        return new Ast.Base(cKeyWork, bb2, fun.function); 
                    }
                    match(TokenType.Semicolon);
                    return new Ast.Base(cKeyWork, value);
                case TokenType.Return:
                    progress();
                    var exp = start();
                    match(TokenType.Semicolon);
                    return new Ast.Base(cKeyWork, exp);
                case TokenType.Function:
                    List<Ast.Ast> list = new List<Ast.Ast>();
                    var bb = new BaseList(list);
                    progress();
                    var title = (Variable)start();
                    if (match(TokenType.LeftParen))
                    {
                        list.Add(start());
                        while (match(TokenType.Comma))
                        {
                            list.Add(start());
                        }
                        match(TokenType.RightParen);
                    }
                    match(TokenType.LeftBrace);
                    var b = Base();
                    match(TokenType.RightBrace);
                    function.Add(title.Str, new Ast.Base(cKeyWork, bb, b));
                    return new Ast.Base(cKeyWork,bb,b);
                default:
                    throw new Exception("Base fail!!");
            }

        }
        #region Comparison

        Ast.Exp compare()
        {
            var a = currentToken();
            if (a.Type == TokenType.Variable || a.Type == TokenType.Number)
            {
                start();
                var compare_type = currentToken().Type;
                if (compare_type == TokenType.Greater || compare_type == TokenType.GreaterEqrequal || compare_type == TokenType.Less || compare_type == TokenType.LessEqrequal || compare_type == TokenType.Equal)
                {

                }
            }
        }

        Ast.Exp compare_symbol()
        {

        }

        #endregion
        #region EXP Parser
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
        #endregion
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