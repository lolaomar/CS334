using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public interface IStmt
    {
        void Check(Dictionary<string, TokenType> symbols);
        void Interpret(Dictionary<string, dynamic> values);
    }

    public class Break : IStmt
    {
        public void Check(Dictionary<string, TokenType> symbols)
        {

        }

        public void Interpret(Dictionary<string, dynamic> values)
        {

        }
    }

    public class Exec : IStmt
    {
        public IExpr V;

        public Exec(IExpr v)
        {
            V = v;
        }

        public void Check(Dictionary<string, TokenType> symbols)
        {
            V.Check(symbols);
        }

        public void Interpret(Dictionary<string, dynamic> values)
        {
            V.Interpret(values);
        }
    }
    public class Continue : IStmt
    {
        public void Check(Dictionary<string, TokenType> symbols)
        {

        }

        public void Interpret(Dictionary<string, dynamic> values)
        {

        }
    }

    public class Define : IStmt
    {
        public string Name;
        public IExpr Value;
        public TokenType Type;

        public Define(string name, TokenType type, IExpr value)
        {
            Name = name;
            Value = value;
            Type = type;
        }

        public void Check(Dictionary<string, TokenType> symbols)
        {
            TokenType rhsType = Value.Check(symbols);
            if (rhsType != Type)
            {
                Error.haderror = true;
                Parser.error += $"Can't assign value of {rhsType} to {Type}.";
            }
            else symbols[Name] = Type;
        }

        public void Interpret(Dictionary<string, dynamic> values)
        {
            values[Name] = Value.Interpret(values);
        }
    }

    public class DoWhile : IStmt
    {
        public List<IStmt> Stmts;
        public IExpr Cond;

        public DoWhile(List<IStmt> stmts, IExpr cond)
        {
            Stmts = stmts;
            Cond = cond;
        }

        public void Check(Dictionary<string, TokenType> symbols)
        {
            foreach (var item in Stmts)
            {
                item.Check(symbols);
            }
            var L = Cond.Check(symbols);
            if (L != TokenType.Bool)
            {
                Error.haderror = true;
                Parser.error += "invalid condition";
            }
        }

        public void Interpret(Dictionary<string, dynamic> values)
        {
            bool brk= false;
            bool con = false;
            do
            {
                foreach (var item in Stmts)
                {
                    item.Interpret(values);
                    if (item is Break)
                    {
                        brk = true;
                        break;
                    }
                    else if (item is Continue)
                    {
                        con = true;
                        break;
                    }
                    
                }
                if (brk)
                {
                    brk = false;
                    break;
                }
                else if (con)
                {
                    con = false;
                    continue;
                }

            } while (Cond.Interpret(values));
        }
    }

    public class SwitchCase : IStmt
    {
        public IExpr Expr;
        public List<(IExpr, List<IStmt>)> Cases;

        public SwitchCase(IExpr expr, List<(IExpr, List<IStmt>)> cases)
        {
            Expr = expr;
            Cases = cases;
        }

        public void Check(Dictionary<string, TokenType> symbols)
        {
            var Bloz = Expr.Check(symbols);

            foreach (var (i, j) in Cases)
            {
                if (Bloz == i.Check(symbols))
                {
                    foreach (var item in j)
                    {
                        item.Check(symbols);
                    }
                }
                else
                {
                    Error.haderror = true;
                    Parser.error += "invalid case";
                }
            }
        }

        public void Interpret(Dictionary<string, dynamic> values)
        {
            var L = Expr.Interpret(values);
            foreach (var (i, j) in Cases)
            {
                if (i.Interpret(values) == L)
                {
                    foreach (var item in j)
                    {
                        item.Interpret(values);
                    }
                    break;
                }
            }
        }
    }
}
