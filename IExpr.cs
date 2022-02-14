using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public interface IExpr
    {
        TokenType Check(Dictionary<string, TokenType> symbols);
        dynamic Interpret(Dictionary<string, dynamic> values);
    }

    public class Error
    {
        public static bool haderror = false;
    }

    public class Variable : IExpr
    {
        public string Name;

        public Variable(string name)
        {
            Name = name;
        }

        public TokenType Check(Dictionary<string, TokenType> symbols)
        {
            if (symbols.ContainsKey(Name))
                return symbols[Name];
            else
            {
                Error.haderror = true;
                Parser.error += "Undefined variable";
            }
            return TokenType.Int;
        }

        public dynamic Interpret(Dictionary<string, dynamic> values)
        {
            return values[Name];
        }
    }

    public class Constant : IExpr
    {
        public dynamic Value;

        public Constant(dynamic value)
        {
            Value = value;
        }

        public TokenType Check(Dictionary<string, TokenType> symbols)
        {
            if (Value is int)
            {
                return (TokenType.Int);
            }
            if (Value is char)
            {
                return (TokenType.Char);
            }
            if (Value is bool)
            {
                return (TokenType.Bool);
            }
            if (Value is float)
            {
                return (TokenType.Float);
            }

            if (Value is double)
            {
                return (TokenType.Double);
            }
            return (TokenType.String);
        }

        public dynamic Interpret(Dictionary<string, dynamic> values)
        {
            return Value;
        }
    }

    public class Single : IExpr
    {
        public TokenType Operator;
        public IExpr Ohs;

        public Single(TokenType @operator, IExpr ohs)
        {
            Operator = @operator;
            Ohs = ohs;
        }

        public TokenType Check(Dictionary<string, TokenType> symbols)
        {
            var o = Ohs.Check(symbols);
            switch (Operator)
            {
                case TokenType.Not:

                    if (o == TokenType.Bool)
                    {
                        return TokenType.Bool;
                    }
                    Error.haderror = true;
                    Parser.error += "Error";
                    break;
                case TokenType.Dash:

                    if (o == TokenType.Int || o == TokenType.Float || o == TokenType.Double)
                    {
                        return o;
                    }
                    Error.haderror = true;
                    Parser.error += "invalid program";
                    return TokenType.Double;
            }

            return TokenType.Int;
        }

        public dynamic Interpret(Dictionary<string, dynamic> values)
        {
            
            switch (Operator)
            {
                case TokenType.Dash:
                    return -1 * Ohs.Interpret(values);

                case TokenType.Not:


                    return !Ohs.Interpret(values);
            
            }
            return 0;

        }
    }

    

    public class Binary : IExpr
    {
        public TokenType Operator;
        public IExpr Lhs;
        public IExpr Rhs;

        public Binary(TokenType @operator, IExpr lhs, IExpr rhs)
        {
            Operator = @operator;
            Lhs = lhs;
            Rhs = rhs;
        }

        public TokenType Check(Dictionary<string, TokenType> symbols)
        {
            var L = Lhs.Check(symbols);
            var R = Rhs.Check(symbols);
            switch (Operator)
            {
                case TokenType.NotEqual:

                case TokenType.LessOrEqual:

                case TokenType.GreaterOrEqual:

                case TokenType.DoubleEqual:

                case TokenType.Lessthan:

                case TokenType.Greaterthan:
                    if (L == R)
                    {
                        return TokenType.Bool;
                    }
                    Error.haderror = true;
                    Parser.error += "error";
                    return TokenType.Bool;
                case TokenType.And:
                case TokenType.Or:
                    if (L != TokenType.Bool)
                    {
                        Error.haderror = true;
                        Parser.error += "Error";
                    }
                    return TokenType.Bool;
                  

                case TokenType.Plus:
                case TokenType.Dash:
                case TokenType.Slash:
                case TokenType.percent:
                case TokenType.Star:
                    if ((L != TokenType.Int || L != TokenType.Float || L != TokenType.Double) && L != R)
                    {
                        Error.haderror = true;
                        Parser.error += "Error";
                    }
                    else
                    {
                        return L;
                    }
                    break;
                case TokenType.Equal:
                    if (L == R)
                    {
                        return L;
                    }
                    else
                    {
                        Error.haderror = true;
                        Parser.error += "Types are not equal";
                    }
                    break;
            }
            throw new Exception();
        }

        public dynamic Interpret(Dictionary<string, dynamic> values)
        {
            var L = Lhs.Interpret(values);
            var R = Rhs.Interpret(values);
            switch (Operator)
            {
                case TokenType.DoubleEqual:
                    return L == R;
                case TokenType.NotEqual:
                    return L != R;
                case TokenType.LessOrEqual:
                    return L <= R;
                case TokenType.GreaterOrEqual:
                    return L >= R;
                case TokenType.And:
                    return L && R;
                case TokenType.Or:
                    return L || R;
                case TokenType.Plus:
                    return L + R;
                case TokenType.Dash:
                    return L - R;
                case TokenType.Slash:
                    return L / R;
                case TokenType.percent:
                    return L % R;
                case TokenType.Star:
                    return L * R;
                case TokenType.Lessthan:
                    return L < R;
                case TokenType.Greaterthan:
                    return L > R;
                case TokenType.Equal:
                    if (Lhs is Variable)
                    {
                        values[((Variable)Lhs).Name] = R;
                    }
                    return R;
            }
            Error.haderror = true;
            throw new Exception();
        }
    }
}
