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

            return Interprete(ast);


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
                baseInterpreter(b);
                c++;
            }
            void baseInterpreter(Base b)
            {
                switch (b.statiment)
                {
                    case Keyword.let:
                        var b_id = (Variable)b.id;
                        var b_num = (Number)b.exp;

                        if (name.ContainsKey(b_id.Str))
                        {
                            name[b_id.Str] = b_num.Value;
                        }
                        else
                        {
                            name.Add(String.Format("{0}", b_id.Str), b_num.Value);
                        }
                        
                        break;
                    case Keyword.variable:
                        var b_id2 = (Variable)b.id;
                        var b_num2 = (Exp) b.exp;

                        if (name.ContainsKey(b_id2.Str))
                        {
                            name[b_id2.Str] = sum(b_num2);
                        }
                        else
                        {
                            name.Add(String.Format("{0}", b_id2.Str), sum(b_num2));
                        }
                        break;
                    case Keyword.print:
                        var b_exp = b.exp;
                        ans = sum(b_exp);
                        break;
                    case Keyword.function:
                        baseInterpreter((Base)b.function);
                        break;
                    case Keyword.return_:
                        var r_exp = b.exp;
                        ans = sum(r_exp);
                        break;
                    default:
                        break;
                }
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
                if (name.ContainsKey(va.Str))
                {
                    name.TryGetValue(va.Str, out f);
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