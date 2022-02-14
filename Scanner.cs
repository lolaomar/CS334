using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Compiler
{
    public enum TokenType
    {
        Int,
        Float,
        String,
        Double,
        Bool,
        Char,
        TextChar,
        TextString,
        True,
        False,

        Case,
        Do,
        Break,
        Continue,
        While,
        Switch,

        Var,
        Num,
        floatnum,

        DoubleEqual,
        NotEqual,
        LessOrEqual,
        GreaterOrEqual,
        And,
        Or,

        Plus = '+',
        Dash = '-',
        Slash = '/',
        percent = '%',
        Star = '*',
        OpenBracket = '(',
        CloseBracket = ')',
        OpenCurlyBracket = '}',
        CloseCurlyBracket = '{',
        Comma = ',',
        Semicolon = ';',
        Lessthan = '<',
        Greaterthan = '>',
        Equal = '=',
        Not = '!',
        DoubleQuote = '\'',
        Quote = '"',
        Colon = ':',
    }

    public class Token
    {
        public TokenType type;
        public string value;

        public Token(TokenType type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }

    public class Scanner
    {
        private int curr = 0;
        private string text;
        public string error = "";
        public bool unkown = false;

        public Scanner(string text)
        {
            this.text = text;
        }

        public Dictionary<string, Token> KeyWords = new Dictionary<string, Token>
        {
            {"int",         new Token(TokenType.Int, "int")},
            {"float",       new Token(TokenType.Float, "float")},
            {"string",      new Token(TokenType.String, "string")},
            {"double",      new Token(TokenType.Double, "double")},
            {"bool",        new Token(TokenType.Bool, "bool")},
            {"char",         new Token(TokenType.Char, "char")},
            {"case",        new Token(TokenType.Case, "case")},
            {"do",          new Token(TokenType.Do, "do")},
            {"break",       new Token(TokenType.Break, "break")},
            {"continue",    new Token(TokenType.Continue, "continue")},
            {"while",       new Token(TokenType.While, "while")},
            {"switch",      new Token(TokenType.Switch, "switch")},
            {"true",      new Token(TokenType.True, "true")},
            {"false",      new Token(TokenType.False, "false")},
        };

        public List<Token> Scan()
        {
            List<Token> tokens = new List<Token>();
            while (curr < text.Length)
            {
                switch (text[curr])
                {
                    case '+':
                        tokens.Add(new Token(TokenType.Plus, "+")); curr++;
                        break;
                    case ':':
                        tokens.Add(new Token(TokenType.Colon, ":")); curr++;
                        break;
                    case '-':
                        tokens.Add(new Token(TokenType.Dash, "-")); curr++;
                        break;
                    case '/':
                        tokens.Add(new Token(TokenType.Slash, "/")); curr++;
                        break;
                    case '%':
                        tokens.Add(new Token(TokenType.percent, "%")); curr++;
                        break;
                    case '*':
                        tokens.Add(new Token(TokenType.Star, "*")); curr++;
                        break;
                    case '(':
                        tokens.Add(new Token(TokenType.OpenBracket, "(")); curr++;
                        break;
                    case ')':
                        tokens.Add(new Token(TokenType.CloseBracket, ")")); curr++;
                        break;
                    case '{':
                        tokens.Add(new Token(TokenType.OpenCurlyBracket, "{")); curr++;
                        break;
                    case '}':
                        tokens.Add(new Token(TokenType.CloseCurlyBracket, "}")); curr++;
                        break;
                    case ',':
                        tokens.Add(new Token(TokenType.Comma, ",")); curr++;
                        break;
                    case ';':
                        tokens.Add(new Token(TokenType.Semicolon, ";")); curr++;
                        break;
                    case '&':
                        if (curr + 1 < text.Length && text[curr + 1] == '&')
                        {
                            tokens.Add(new Token(TokenType.And, "&&"));
                            curr += 2;
                        }
                        else { curr++; error += "Missing '&' "; }
                        break;
                    case '|':
                        if (curr + 1 < text.Length && text[curr + 1] == '|')
                        {
                            tokens.Add(new Token(TokenType.Or, "||"));
                            curr += 2;
                        }
                        else { curr++; error += "Missing '|' "; }
                        break;
                    case '<':
                        if (curr + 1 < text.Length && text[curr + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.LessOrEqual, "<="));
                            curr += 2;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Lessthan, "<")); curr++;
                        }
                        break;
                    case '>':
                        if (curr + 1 < text.Length && text[curr + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.GreaterOrEqual, ">="));
                            curr += 2;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Greaterthan, ">")); curr++;
                        }
                        break;
                    case '=':
                        if (curr + 1 < text.Length && text[curr + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.DoubleEqual, "=="));
                            curr += 2;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Equal, "=")); curr++;
                        }
                        break;
                    case '!':
                        if (curr + 1 < text.Length && text[curr + 1] == '=')
                        {
                            tokens.Add(new Token(TokenType.NotEqual, "!="));
                            curr += 2;
                        }
                        else
                        {
                            tokens.Add(new Token(TokenType.Not, "!")); curr++;
                        }
                        break;
                    case '"':
                        string str = "";
                        curr++;
                        while (curr < text.Length && text[curr] != '"')
                        {
                            str += text[curr];
                            curr++;
                        }
                        tokens.Add(new Token(TokenType.TextString, str));
                        curr++;
                        break;
                    case '\'':
                        string str1 = "";
                        curr++;
                        while (curr < text.Length && text[curr] != '\'')
                        {
                            str1 += text[curr];
                            curr++;
                        }
                        tokens.Add(new Token(TokenType.TextChar, str1));
                        curr++;
                        break;
                    default:
                        string word = "";

                        if (char.IsWhiteSpace(text[curr]))
                            curr++;

                        else if (char.IsLetter(text[curr]))
                        {
                            while (curr < text.Length && char.IsLetter(text[curr]))
                            {
                                word += text[curr];
                                curr++;
                            }
                            if (KeyWords.ContainsKey(word))
                            {
                                tokens.Add(KeyWords[word]);
                            }
                            else
                            {
                                tokens.Add(new Token(TokenType.Var, word));
                            }
                        }
                        else if (char.IsDigit(text[curr]))
                        {
                            if (curr < text.Length)
                            {
                                while (curr < text.Length && char.IsDigit(text[curr]))
                                {
                                    word += text[curr];
                                    curr++;
                                }

                                if (curr < text.Length && text[curr] == '.')
                                {
                                    word += '.';
                                    curr++;
                                    while (curr < text.Length && char.IsDigit(text[curr]))
                                    {
                                        word += text[curr];
                                        curr++;
                                    }
                                    tokens.Add(new Token(TokenType.floatnum, word));
                                }
                                else { tokens.Add(new Token(TokenType.Num, word)); }
                            }
                        }
                        else
                        {
                            unkown = true;
                            curr++;
                        }
                        break;
                }
            }
            return tokens;
        }

    }
}