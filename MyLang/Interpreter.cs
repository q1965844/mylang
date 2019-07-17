using System;
using System.Collections.Generic;
using System.Text;
using MyLang.Ast;

namespace MyLang
{
    public class Interpreter
    {
        public Interpreter()
        {
        }

        public float Run(Ast.Ast ast)
        {
            Exp exp = (Exp)ast;
            return sum(exp);
        }

        public float sum(Exp exp)
        {
            if (exp is BinOp)
            {
                var exp_b = (BinOp)exp;
                var exp_teyp = exp_b.Operator;
                var left = sum(exp_b.Lhs);
                var right = sum(exp_b.Rhs);
                switch (exp_teyp)
                {
                    case BinOpType.Add:
                        return left + right;
                    case BinOpType.Sub:
                        return left - right;
                    case BinOpType.Multiply:
                        return left * right;
                    case BinOpType.Divide:
                        return left / right;
                    default:
                        throw new Exception("error");
                }
            }
            else if (exp is Number)
            {
                var num = (Number)exp;
                return num.Value;
            }
            else
            {
                throw new Exception("error");
            }
        }
    }
}