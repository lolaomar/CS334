using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public class Parser
    {
        private int curr = 0;
        private List<Token> tokens;
        public static string error = "";

        private Dictionary<TokenType, IExpr> DefaultValues = new Dictionary<TokenType, IExpr>()
        {
            { TokenType.Int, new Constant(0) },
            { TokenType.Float, new Constant(0.0) },
            { TokenType.String, new Constant("") },
            { TokenType.Double, new Constant(0.0) },
            { TokenType.Bool, new Constant(false) },
            { TokenType.Char, new Constant('\0') },
        };
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public bool NotAtEnd()
        {
            return curr < tokens.Count;
        }

        public IStmt Stmt(bool matchbreak = true)
        {
            if (NotAtEnd())
            {
                var token = tokens[curr];
                switch (token.type)
                {
                    case TokenType.Break:
                        if (matchbreak)
                        {
                            Match(tokens[curr].type);
                            Match(TokenType.Semicolon);
                        }
                        return new Break();
                    case TokenType.Continue:
                        Match(tokens[curr].type);
                        Match(TokenType.Semicolon);
                        return new Continue();
                    case TokenType.Int:
                    case TokenType.Bool:
                    case TokenType.Float:
                    case TokenType.Double:
                    case TokenType.String:
                    case TokenType.Char:
                        Match(tokens[curr].type);
                        var name = tokens[curr];
                        Match(TokenType.Var);
                        if (NotAtEnd())
                        {
                            switch (tokens[curr].type)
                            {
                                case TokenType.Semicolon:
                                    Match(TokenType.Semicolon);
                                    var def = DefaultValues[token.type];
                                    return new Define(name.value, token.type, def);
                                case TokenType.Equal:
                                    Match(TokenType.Equal);
                                    var expr = Exp();
                                    Match(TokenType.Semicolon);
                                    return new Define(name.value, token.type, expr);
                            }
                        }
                        else error += "Expecting either a Semicolon or Equal.";
                        break;
                    case TokenType.Do:
                        Match(TokenType.Do);
                        Match(TokenType.OpenCurlyBracket);

                        var stmts = new List<IStmt>();

                        if (NotAtEnd())
                        {
                            while (curr < tokens.Count && tokens[curr].type != TokenType.CloseCurlyBracket)
                            {
                                stmts.Add(Stmt());
                            }
                        }
                        Match(TokenType.CloseCurlyBracket);
                        Match(TokenType.While);
                        Match(TokenType.OpenBracket);
                        var cond = Exp();
                        Match(TokenType.CloseBracket);
                        Match(TokenType.Semicolon);
                        return new DoWhile(stmts, cond);

                    case TokenType.Switch:
                        Match(TokenType.Switch);
                        Match(TokenType.OpenBracket);
                        var expr2 = Exp();
                        var cases = new List<(IExpr, List<IStmt>)>();

                        Match(TokenType.CloseBracket);
                        Match(TokenType.OpenCurlyBracket);
                        if (NotAtEnd())
                        {
                            while (curr < tokens.Count && tokens[curr].type != TokenType.CloseCurlyBracket)
                            {
                                var _case = Cases();
                                if (_case != null)
                                {
                                    cases.Add(((IExpr, List<IStmt>))_case);
                                }
                            }
                            Match(TokenType.CloseCurlyBracket);
                        }
                        return new SwitchCase(expr2, cases);
                    case TokenType.Case:
                        Match(TokenType.Case);
                        error += "Unexpected case in statement ";
                        break;
                    default:
                        var V = Exp();
                        Match(TokenType.Semicolon);
                        
                        return new Exec(V);
                }
            }
            return null;
        }

        private (IExpr, List<IStmt>)? Cases()
        {
            if (NotAtEnd())
            {
                switch (tokens[curr].type)
                {
                    case TokenType.Case:
                        Match(TokenType.Case);
                        var expr = Exp();
                        Match(TokenType.Colon);
                        var stmts = new List<IStmt>();
                        if (NotAtEnd())
                        {
                            while (curr < tokens.Count && tokens[curr].type != TokenType.Break)
                            {
                                stmts.Add(Stmt(false));
                            }
                        }
                        Match(TokenType.Break);
                        Match(TokenType.Semicolon);
                        return (expr, stmts);
                }
            }
            return null;
        }

        public List<IStmt> Program()
        {
            var stmt = new List<IStmt>();
            while (curr < tokens.Count)
            {
                stmt.Add(Stmt());
            }
            return stmt;
        }

        private void Match(TokenType expected)
        {
            if (curr >= tokens.Count)
            {
                error += $"Error: Expected {expected}.";
                return;
            }
            else if (tokens[curr].type == expected)
            {
                curr++;
            }
            else
            {
                error += $"Error: Expected {expected}, but got {tokens[curr].type}.";
                curr++;
            }
        }

        private IExpr Factor()
        {
            if (NotAtEnd())
            {
                var token = tokens[curr];
                switch (token.type)
                {
                    case TokenType.Not:
                    case TokenType.Dash:
                        Match(token.type);
                        return new Single(token.type, Factor());
                    case TokenType.True:
                        Match(TokenType.True);
                        return new Constant(true);
                    case TokenType.False:
                        Match(TokenType.False);
                        return new Constant(false);
                    case TokenType.Num:
                        Match(token.type);
                        return new Constant(int.Parse(token.value));

                    case TokenType.floatnum:
                        Match(token.type);

                        return new Constant(float.Parse(token.value));
                    case TokenType.TextString:
                        Match(token.type);
                        return new Constant(token.value);
                    case TokenType.TextChar:
                        if (token.value.Length > 1)
                        {
                            error += "Char Can't handle more than 1 character";
                            Match(token.type);
                            return null;
                        }
                        Match(token.type);
                        return new Constant(token.value[0]);
                    case TokenType.Var:
                        Match(token.type);
                        return new Variable(token.value);
                    case TokenType.OpenBracket:
                        Match(token.type);
                        var result = Exp();
                        Match(TokenType.CloseBracket);
                        return result;
                    default:
                        return null;
                }
            }return null;
        }

        private IExpr Term()
        {
            bool done = false;
            var Lhs = Factor();
            while (!done)
            {
                if (NotAtEnd())
                {
                    switch (tokens[curr].type)
                    {
                        case TokenType.DoubleEqual:
                        case TokenType.NotEqual:
                        case TokenType.LessOrEqual:
                        case TokenType.GreaterOrEqual:
                        case TokenType.And:
                        case TokenType.Or:
                        case TokenType.Plus:
                        case TokenType.Dash:
                        case TokenType.Slash:
                        case TokenType.percent:
                        case TokenType.Star:
                        case TokenType.Comma:
                        case TokenType.Lessthan:
                        case TokenType.Greaterthan:
                            var token = (tokens[curr].type);
                            Match(tokens[curr].type);
                            var Rhs = Factor();
                            Lhs = new Binary(token, Lhs, Rhs);
                            break;
                        default:
                            done = true;
                            break;
                    }
                }
                else
                {
                    done = true;
                }
            }
            return Lhs;
        }
        private IExpr Exp()
        {
            bool done = false;
            var Lhs = Term();
            while (!done)
            {
                if (NotAtEnd())
                {
                    switch (tokens[curr].type)
                    {
                        case TokenType.Equal:
                            var token = (tokens[curr].type);
                            Match(tokens[curr].type);
                            var Rhs = Exp();
                            Lhs = new Binary(token, Lhs, Rhs);
                            break;
                        default:
                            done = true;
                            break;
                    }
                }
                else { done = true; }
            }
            return Lhs;
        }
    }
}