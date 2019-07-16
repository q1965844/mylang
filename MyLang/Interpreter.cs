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
            return calculate((Exp)ast);
        }

        float calculate(Exp exp)
        {
            if (exp is BinOp)
            {
                var bin = (BinOp)exp;
                var lValue = calculate(bin.Lhs);
                var rValue = calculate(bin.Rhs);
                switch (bin.Operator)
                {
                    case BinOpType.Add:
                        return lValue + rValue;
                    case BinOpType.Sub:
                        return lValue - rValue;
                    case BinOpType.Multiply:
                        return lValue * rValue;
                    case BinOpType.Divide:
                        return lValue / rValue;
                    default:
                        throw new Exception("bug");
                }
            }
            else if (exp is Number)
            {
                var num = (Number)exp;
                return num.Value;
            }
            else
            {
                throw new Exception("bug");
            }
        }
    }
}