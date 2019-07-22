using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyLang
{
    namespace Ast
    {
        /// <summary>
        /// AST(Abstract Syntax Tree)のベースクラス
        /// </summary>
        public abstract class Ast
        {
            /// <summary>
            /// 文字列表現を作成するための情報を取得する.
            /// 
            /// string は 文字列でのそのASTの種類を表す
            /// Ast[] は、子供のASTを返す。子供を取らないASTの場合は、nullが入る。
            /// </summary>
            /// <returns>文字列表現のための情報</returns>
            public abstract Tuple<string, Ast[]> GetDisplayInfo();
        }

        /// <summary>
        /// 式(Expression) のベースクラス
        /// </summary>
        public abstract class Exp : Ast { }
        public abstract class Stat : Ast { }
        /// <summary>
        /// ２項演算子の種類
        /// </summary>
        public enum BinOpType
        { 
            Add, // +
            Sub, // -
            Multiply, // *
            Divide, // /
        }

        public enum SymbolType
        {
            equal,

        }
        public enum Keyword
        {
            let,
            print,
            function,
            return_,
            variable,
        }

        #region Expression //式

        /// <summary>
        /// 二項演算子(Binary Operator)を表すAST
        /// </summary>
        public class BinOp : Exp
        {
            public readonly BinOpType Operator;
            public readonly Exp Lhs;
            public readonly Exp Rhs;
            public BinOp(BinOpType op, Exp lhs, Exp rhs)
            {
                Operator = op;
                Lhs = lhs;
                Rhs = rhs;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Operator.ToString(), new Ast[] { Lhs, Rhs });
            }
        }

        /// <summary>
        /// 数値を表すAST
        /// </summary>
        public class Number : Exp
        {
            public readonly float Value;
            public Number(float value)
            {
                Value = value;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Value.ToString(), (Ast[])null);
            }
        }

        public class Variable : Exp
        {
            public readonly string Str;
            public Variable(string value)
            {
                Str = value;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create(Str, (Ast[])null);
            }
        }
        #endregion

        public class Baselist : Ast
        {
            public readonly List<Ast> Base;
            public Baselist(List<Ast> _base)
            {
                Base = _base;
            }

            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                return Tuple.Create("Baselist", Base.ToArray());
            }
        }

        public class Base : Stat
        {
            public readonly Keyword statiment;
            public readonly string function_name;
            public readonly Exp id;
            public readonly Exp exp;
            public readonly Stat function;
            public readonly Exp f_name;
            public readonly Baselist bl;
            public Base(Keyword _stat, Baselist _bl, Stat _function)
            {
                statiment = _stat;
                bl = _bl;
                function = _function;
            }
            public Base(Keyword _stat,Exp _id,Exp _exp)
            {
                statiment = _stat;
                id = _id;
                exp = _exp;
            }

            public Base(Keyword _stat,Exp _exp)
            {
                statiment = _stat;
                exp = _exp;
            }
            public override Tuple<string, Ast[]> GetDisplayInfo()
            {
                if (statiment == Keyword.let || (statiment == Keyword.variable && id!=null))
                {   
                    
                    return Tuple.Create(statiment.ToString(), new Ast[] { id, exp });
                }
                else if(statiment == Keyword.function || statiment == Keyword.print)
                {
                    return Tuple.Create(statiment.ToString(), new Ast[] {bl,function });
                }
                else
                {
                    return Tuple.Create(statiment.ToString(), new Ast[] { exp });
                }
            }
        }

        #region 文字列表現

        /// <summary>
        /// ASTを文字列表現に変換するクラス
        /// </summary>
        public class AstDisplayer
        {
            List<Tuple<int, string>> list_;

            public AstDisplayer() { }

            /// <summary>
            /// ASTから、文字列表現に変換する.
            /// 
            /// prettyPrintにtrueを指定すると、改行やインデントを挟んだ読みやすい表現になる
            /// 
            /// BuildString(1 + 2 * 3 の AST, false) => "Add( 1 Multiply( 2 3 ) )"
            /// 
            /// BuildString(1 + 2 * 3 の AST, true) => 
            ///   "Add( 
            ///     1 
            ///     Multiply(
            ///       2
            ///       3
            ///     )
            ///    )"
            /// </summary>
            /// <param name="ast">対象のAST</param>
            /// <param name="prettyPrint">Pretty pring をするかどうか</param>
            /// <returns></returns>
            public string BuildString(Ast ast, bool prettyPrint = true)
            {
                list_ = new List<Tuple<int, string>>();
                build(0, ast);
                if( prettyPrint)
                {
                    return string.Join("\n", list_.Select(s => new string(' ', s.Item1 * 2) + s.Item2).ToArray());
                }
                else
                {
                    return string.Join(" ", list_.Select(s => s.Item2).ToArray());
                }
            }

            void build(int level, Ast ast)
            {
                var displayInfo = ast.GetDisplayInfo();
                if (displayInfo.Item2 == null)
                {
                    add(level, displayInfo.Item1);
                }
                else
                {
                    add(level, displayInfo.Item1 + "(");
                    foreach( var child in displayInfo.Item2)
                    {
                        build(level + 1, child);
                    }
                    add(level, ")");
                }
            }

            void add(int level, string text)
            {
                list_.Add(Tuple.Create(level, text));
            }
        }
        #endregion
    }

}
