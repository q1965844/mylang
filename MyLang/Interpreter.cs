using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyLang.Ast;

namespace MyLang
{
    public class Interpreter
    {
        public Interpreter()
        {
        }

        static Dictionary<string, float> name = new Dictionary<string, float>();

        public float Run(Ast.Ast ast)
        {
            Interprete(ast);
            return 0;


        }
        public float Interprete(Ast.Ast ast)
        {
            var Blist = (Baselist)ast;
            var baselist = Blist.Base.ToArray();
            var c = 0;
            float ans = 0;
            while (c < baselist.Length)
            {
                var b = (Base)baselist[c];
                switch (b.statiment)
                {
                    case Keyword.let:
                        var b_id = b.id;
                        var b_num = (Number)b.exp;
                        for (int i = 0; i < 5; i++)
                        {
                            if (name.ContainsKey(b_id.ToString()))
                            {
                                break;
                            }
                            else
                            {
                                name.Add(String.Format("{0}", b_id, b_num.Value), i);
                            }

                        }
                        break;
                    case Keyword.print:
                        var b_exp = b.exp;
                        ans = sum(b_exp);
                        break;
                    case Keyword.function:
                        break;
                    case Keyword.return_:
                        break;
                    default:
                        break;
                }
                c++;
            }
            return ans;
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
            else if (exp is Variable)
            {
                var va = (Variable)exp;
                float f;
                if (name.TryGetValue(va.Str, out f))
                {
                    return f;
                }
                else
                {
                    throw new Exception("error");
                }
            }
            else
            {
                throw new Exception("error");
            }
        }
    }

}