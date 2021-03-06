﻿using System;
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

        Token consume(TokenType expected)
        {
            var t = currentToken();
            if( t.Type != expected)
            {
                throw new Exception($"Syntax error, expected {expected} but {t.Text}");
            }
            progress();
            return t;
        }

        public Ast.Ast Parse(IList<Token> tokens)
        {
            tokens_ = tokens;
            pos_ = 0;
            return parseProgram();
        }

        Ast.Program parseProgram()
        {
            var statements = new List<Ast.Statement>();
            while(!currentToken().IsTerminate)
            {
                statements.Add(parseStatement());
            }
            return new Ast.Program(statements);
        }

        #region Statement
        Ast.Statement parseStatement()
        {
            var t = currentToken();
            if( t.Type == TokenType.Let)
            {
                return parseAssignStatement();
            }
            else if( t.Type == TokenType.Print)
            {
                return parsePrintStatement();
            }
            else if (t.Type == TokenType.Function)
            {
                return parseFunctionStatement();
            }
            else if (t.Type == TokenType.Return)
            {
                return parseReturnStatement();
            }
            else
            {
                throw new Exception("BUG");
                return null;
                //return parseExpStatement();
            }
        }

        Ast.Statement parseAssignStatement()
        {
            consume(TokenType.Let);

            var variable = parseExp();

            consume(TokenType.Equal);

            var exp = parseExp();

            consume(TokenType.Semicolon);

            return new Ast.AssignStatement(variable, exp);
        }

        Ast.Statement parsePrintStatement()
        {
            consume(TokenType.Print);

            var exp = parseExp();

            consume(TokenType.Semicolon);

            return new Ast.PrintStatement(exp);
        }

        Ast.Statement parseReturnStatement()
        {
            consume(TokenType.Return);

            var exp = parseExp();

            consume(TokenType.Semicolon);

            return new Ast.ReturnStatement(exp);
        }


        Ast.Statement parseFunctionStatement()
        {
            consume(TokenType.Function);

            var name = parseSymbol();

            consume(TokenType.LParen);

            var parameters = new List<Ast.Symbol>();
            if( currentToken().Type != TokenType.RParen)
            {
                while (true)
                {
                    var arg = parseSymbol();
                    parameters.Add(arg);
                    if (currentToken().Type != TokenType.Comma)
                    {
                        break;
                    }
                    consume(TokenType.Comma);
                }
            }

            consume(TokenType.RParen);

            var body = parseBlock();

            return new Ast.FunctionStatement(name, parameters.ToArray(), body);
        }

        Ast.Symbol parseSymbol()
        {
            var t = consume(TokenType.Symbol);
            return new Ast.Symbol(t.Text);
        }

        Ast.Statement[] parseBlock()
        {
            consume(TokenType.LBraket);

            var statements = new List<Ast.Statement>();
            while( currentToken().Type != TokenType.RBraket)
            {
                var stat = parseStatement();
                statements.Add(stat);
            }

            consume(TokenType.RBraket);

            return statements.ToArray();
        }

        #endregion

        #region Expression

        Ast.Exp parseExp()
        {
            return parseExp1();
        }

        Ast.Exp parseExp1()
        {
            var lhs = parseExp2();
            if (lhs == null)
            {
                return null;
            }

            return parseExp1Rest(lhs);
        }

        /// <summary>
        /// 左結合のために、Exp1を分割したもの
        /// </summary>
        /// <param name="lhs">パース済みの左辺項</param>
        Ast.Exp parseExp1Rest(Ast.Exp lhs)
        {
            var t = currentToken();
            if (t.Type == TokenType.Plus || t.Type == TokenType.Minus)
            {
                var binopType = BinOpMap[t.Type];
                progress();

                var rhs = parseExp2();
                if (rhs == null)
                {
                    throw new Exception("No rhs parsed");
                }

                var exp = new Ast.BinOp(binopType, lhs, rhs);

                return parseExp1Rest(exp);
            }
            else
            {
                return lhs;
            }
        }

        /// <summary>
        /// "*", "/" の演算子をパースする
        /// </summary>
        Ast.Exp parseExp2()
        {
            var lhs = parseValue();
            if( lhs == null)
            {
                return null;
            }

            return parseExp2Rest(lhs);
        }

        /// <summary>
        /// 左結合のために、Exp2を分割したもの
        /// </summary>
        /// <param name="lhs">パース済みの左辺項</param>
        Ast.Exp parseExp2Rest(Ast.Exp lhs)
        {
            var t = currentToken();
            if (t.Type == TokenType.Star || t.Type == TokenType.Slash)
            {
                var binopType = BinOpMap[t.Type];
                progress();

                var rhs = parseValue();
                if (rhs == null)
                {
                    throw new Exception("No rhs parsed");
                }

                var exp = new Ast.BinOp(binopType, lhs, rhs);

                return parseExp2Rest(exp);
            }
            else
            {
                return lhs;
            }
        }

        /// <summary>
        /// 値をパースする
        /// </summary>
        /// <returns></returns>
        Ast.Exp parseValue()
        {
            var t = currentToken();
            if (t.IsNumber)
            {
                progress();
                return new Ast.Number(float.Parse(t.Text));
            }
            else if (t.IsSymbol)
            {
                var name = parseSymbol();

                if (currentToken().Type != TokenType.LParen)
                {
                    // 変数参照
                    return name;
                }
                else
                {
                    // 関数適用
                    consume(TokenType.LParen);

                    // 引数のリストをパースする
                    var args = new List<Ast.Exp>();
                    while(true)
                    {
                        args.Add(parseExp());
                        if( currentToken().Type != TokenType.Comma)
                        {
                            break;
                        }
                        consume(TokenType.Comma);
                    }

                    consume(TokenType.RParen);

                    return new Ast.ApplyFunction(name, args.ToArray());
                }
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
