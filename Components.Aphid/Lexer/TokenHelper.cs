﻿using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Components.Aphid.Lexer
{
    public static class TokenHelper
    {
        private enum LineState
        {
            RegularChar,
            ReturnCarriage,
        }

        public static Tuple<int, int> GetIndexPosition(string text, int index)
        {
            var state = LineState.RegularChar;
            var preceding = text.Remove(index);
            var line = 0;
            var col = 0;

            for (int i = 0; i < preceding.Length; i++)
            {
                var c = preceding[i];
                switch (state)
                {
                    case LineState.RegularChar:
                        switch (c)
                        {
                            case '\r':
                                line++;
                                col = 0;
                                state = LineState.ReturnCarriage;
                                break;
                            case '\n':
                                line++;
                                col = 0;
                                break;
                            default:
                                col++;
                                break;
                        }
                        break;

                    case LineState.ReturnCarriage:
                        switch (c)
                        {
                            case '\r':
                                line++;
                                col = 0;
                                break;
                            case '\n':
                                col = 0;
                                state = LineState.RegularChar;
                                break;
                            default:
                                col++;
                                state = LineState.RegularChar;
                                break;
                        }
                        break;
                }
            }

            return Tuple.Create(line, col);
        }

        public static int GetIndex(string text, int line, int col)
        {
            if (line == 0)
            {
                return col;
            }

            var state = LineState.RegularChar;
            int curLine = 0;
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                switch (state)
                {
                    case LineState.RegularChar:
                        switch (c)
                        {
                            case '\r':
                                state = LineState.ReturnCarriage;
                                break;

                            case '\n':
                                curLine++;

                                if (curLine == line)
                                {
                                    return i + col + 1;
                                }

                                break;
                        }
                        break;

                    case LineState.ReturnCarriage:
                        switch (c)
                        {
                            case '\r':
                                for (int j = 0; j < 2; j++)
                                {
                                    curLine++;

                                    if (curLine == line)
                                    {
                                        return i + col + j;
                                    }
                                }

                                break;

                            case '\n':
                                curLine++;

                                if (curLine == line)
                                {
                                    return i + col + 1;
                                }

                                state = LineState.RegularChar;
                                break;

                            default:
                                state = LineState.RegularChar;
                                break;
                        }
                        break;
                }
            }

            return -1;
        }

        public static int[][] GetBraces(string text, int line, int col)
        {
            var index = TokenHelper.GetIndex(text, line, col);
            var str = text.Substring(index);
            var tokens = new AphidLexer(str).GetTokens();
            var startBrace = tokens[0].TokenType;

            AphidTokenType[] braces;

            switch (tokens[0].TokenType)
            {
                case AphidTokenType.LeftBrace:
                case AphidTokenType.RightBrace:
                    braces = new[] { AphidTokenType.LeftBrace, AphidTokenType.RightBrace };
                    break;

                case AphidTokenType.LeftParenthesis:
                case AphidTokenType.RightParenthesis:
                    braces = new[] { AphidTokenType.LeftParenthesis, AphidTokenType.RightParenthesis };
                    break;

                case AphidTokenType.LeftBracket:
                case AphidTokenType.RightBracket:
                    braces = new[] { AphidTokenType.LeftBracket, AphidTokenType.RightBracket };
                    break;

                default:
                    throw new InvalidOperationException();
            }

            var depth = 1;

            if (startBrace == braces[0])
            {
                var rightBraceIndex = -1;
                for (int i = 1; i < tokens.Count; i++)
                {
                    if (tokens[i].TokenType == braces[0])
                    {
                        depth++;
                    }
                    else if (tokens[i].TokenType == braces[1])
                    {
                        depth--;
                    }

                    if (depth == 0)
                    {
                        rightBraceIndex = index + tokens[i].Index;
                        break;
                    }
                }

                if (rightBraceIndex != -1)
                {
                    var rightLineCol = TokenHelper.GetIndexPosition(text, rightBraceIndex);

                    return new[]
                    {
                        new[] { line, col },
                        new[] { rightLineCol.Item1, rightLineCol.Item2 },
                    };
                }
            }
            else if (startBrace == braces[1])
            {
                depth = -1;
                var leftBraceIndex = -1;

                str = text.Remove(index);
                tokens = new AphidLexer(str).GetTokens();
                for (int i = tokens.Count - 1; i >= 0; i--)
                {
                    if (tokens[i].TokenType == braces[0])
                    {
                        depth++;
                    }
                    else if (tokens[i].TokenType == braces[1])
                    {
                        depth--;
                    }

                    if (depth == 0)
                    {
                        leftBraceIndex = tokens[i].Index;
                        break;
                    }
                }

                if (leftBraceIndex != -1)
                {
                    var leftLineCol = TokenHelper.GetIndexPosition(text, leftBraceIndex);

                    return new[]
                    {
                        new[] { leftLineCol.Item1, leftLineCol.Item2 },
                        new[] { line, col },                        
                    };
                }

            }
            else
            {
                throw new InvalidOperationException();
            }

            return null;
        }

        public static string GetCodeExcerpt(string code, AphidToken token, int surroundingLines = 4)
        {
            var matches = Regex.Matches(code, @"(\r\n)|\r|\n").OfType<Match>().ToArray();
            var firstAfter = matches.FirstOrDefault(x => x.Index > token.Index);

            int line;

            if (firstAfter != null)
            {
                line = Array.IndexOf(matches, firstAfter);
            }
            else
            {
                line = matches.Count();
            }

            var lines = code.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "\r\n").Split(new[] { "\r\n" }, StringSplitOptions.None);
            var sb = new StringBuilder();

            for (int i = line - surroundingLines; i < line + surroundingLines + 1; i++)
            {
                if (i > 0 && i < lines.Length)
                {
                    sb.AppendLine(string.Format("({0}) {1}", i, lines[i]));
                }
            }

            return sb.ToString();
        }
    }
}
