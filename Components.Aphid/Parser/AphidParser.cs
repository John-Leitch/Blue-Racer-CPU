using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Aphid.Parser
{
    public partial class AphidParser
    {
        private List<AphidToken> _tokens;

        private int _tokenIndex = -1;

        private AphidToken _currentToken;

        public AphidParser(List<AphidToken> tokens)
        {
            _tokens = tokens;
        }

        [System.Diagnostics.DebuggerStepThrough]
        private bool Match(AphidTokenType tokenType)
        {
            if (_currentToken.TokenType == tokenType)
            {
                NextToken();
                return true;
            }
            else
            {
                throw new AphidParserException(_currentToken, tokenType);
            }
        }
        
        [System.Diagnostics.DebuggerStepThrough]
        public bool NextToken()
        {
            _tokenIndex++;

            if (_tokenIndex < _tokens.Count)
            {
                _currentToken = _tokens[_tokenIndex];
                return true;
            }
            else
            {
                _currentToken = default(AphidToken);
                return false;
            }
        }

        public Expression ParseExpression()
        {
            return ParseAssignmentExpression();
        }

        public Expression ParseQueryExpression()
        {
            Expression exp = ParseRangeExpression();
            var inQuery = true;

            do
            {
                switch (_currentToken.TokenType)
                {
                    case AphidTokenType.AggregateOperator:
                    case AphidTokenType.AnyOperator:                    
                    case AphidTokenType.SelectManyOperator:
                    case AphidTokenType.SelectOperator:
                    case AphidTokenType.WhereOperator:
                        var t = _currentToken.TokenType;
                        NextToken();
                        exp = new BinaryOperatorExpression(exp, t, ParseRangeExpression());
                        break;

                    case AphidTokenType.DistinctOperator:
                        exp = new UnaryOperatorExpression(_currentToken.TokenType, exp);
                        NextToken();
                        break;   
                 
                    default:
                        inQuery = false;
                        break;
                }
            } 
            while (inQuery);

            return exp;
        }

        private Expression ParseConditionalExpression()
        {
            var exp = ParseLogicalExpression();

            if (_currentToken.TokenType != AphidTokenType.ConditionalOperator)
            {
                return exp;
            }
            else
            {
                NextToken();
                var trueExpression = ParseExpression();
                Match(AphidTokenType.ColonOperator);
                var falseExpression = ParseExpression();

                return new TernaryOperatorExpression(
                    AphidTokenType.ConditionalOperator,
                    exp,
                    trueExpression,
                    falseExpression);
            }
        }

        private Expression ParsePostfixUnaryOperationExpression()
        {
            var term = ParseBinaryOrExpression();

            switch (_currentToken.TokenType)
            {
                case AphidTokenType.IncrementOperator:
                case AphidTokenType.DecrementOperator:
                    var op = _currentToken.TokenType;
                    NextToken();
                    return new UnaryOperatorExpression(op, term) { IsPostfix = true };

                default:
                    return term;
            }
        }

        public Expression ParsePrefixUnaryOperatorExpression()
        {
            switch (_currentToken.TokenType)
            {
                case AphidTokenType.NotOperator:
                case AphidTokenType.IncrementOperator:
                case AphidTokenType.DecrementOperator:
                case AphidTokenType.MultiplicationOperator:
                    var t = _currentToken.TokenType;
                    NextToken();
                    return new UnaryOperatorExpression(t, ParseArrayAccessExpression());

                default:
                    return ParseArrayAccessExpression();
            }
        }

        public Expression ParseArrayAccessExpression()
        {
            var exp = ParseCallExpression();

            while (_currentToken.TokenType == AphidTokenType.LeftBracket)
            {
                NextToken();
                var key = ParseExpression();
                Match(AphidTokenType.RightBracket);
                exp = new ArrayAccessExpression(exp, key);
            }

            return exp;
        }

        public Expression ParseCallExpression()
        {
            var function = ParseMemberExpression();

            while (_currentToken.TokenType == AphidTokenType.LeftParenthesis)
            {
                NextToken();
                if (_currentToken.TokenType == AphidTokenType.RightParenthesis)
                {
                    NextToken();
                    function = new CallExpression(function);
                }
                else
                {
                    var args = ParseTuple();
                    Match(AphidTokenType.RightParenthesis);
                    function = new CallExpression(function, args);
                }
            }

            return function;
        }

        public Expression ParseCallExpression(Expression expression)
        {
            while (_currentToken.TokenType == AphidTokenType.LeftParenthesis)
            {
                NextToken();
                if (_currentToken.TokenType == AphidTokenType.RightParenthesis)
                {
                    NextToken();
                    expression = new CallExpression(expression);
                }
                else
                {
                    var args = ParseTuple();
                    Match(AphidTokenType.RightParenthesis);
                    expression = new CallExpression(expression, args);
                }
            }

            return expression;
        }

        public Expression ParseMemberExpression()
        {
            Expression factor = ParseCallExpression(ParseFactor());

            while (_currentToken.TokenType == AphidTokenType.MemberOperator)
            {
                NextToken();

                Expression exp;

                switch (_currentToken.TokenType)
                {
                    case AphidTokenType.Identifier:
                        exp = new IdentifierExpression(_currentToken.Lexeme);
                        NextToken();

                        break;

                    case AphidTokenType.String:
                        exp = ParseStringExpression();
                        break;

                    case AphidTokenType.LeftBrace:
                        NextToken();
                        exp = new DynamicMemberExpression(ParseExpression());
                        Match(AphidTokenType.RightBrace);
                        break;

                    default:
                        throw new AphidParserException(_currentToken);
                }

                factor = ParseCallExpression(new BinaryOperatorExpression(factor, AphidTokenType.MemberOperator, exp));

                if (_currentToken.TokenType == AphidTokenType.definedKeyword)
                {
                    NextToken();

                    return new UnaryOperatorExpression(AphidTokenType.definedKeyword, factor) { IsPostfix = true };
                }
            }

            return factor;
        }

        public Expression ParseFactor()
        {
            Expression exp;
            switch (_currentToken.TokenType)
            {
                case AphidTokenType.LeftBrace:
                    exp = ParseObjectExpression();
                    break;

                case AphidTokenType.LeftBracket:
                    exp = ParseArrayExpression();
                    break;

                case AphidTokenType.LeftParenthesis:
                    NextToken();
                    exp = ParseExpression();
                    Match(AphidTokenType.RightParenthesis);
                    break;

                case AphidTokenType.String:
                    exp = ParseStringExpression();
                    break;

                case AphidTokenType.Number:
                    exp = ParseNumberExpression();
                    break;

                case AphidTokenType.MinusOperator:
                    NextToken();
                    var numExp = ParseNumberExpression();
                    numExp.Value *= -1;
                    exp = numExp;
                    break;

                case AphidTokenType.Identifier:
                    exp = ParseIdentifierExpression();

                    if (_currentToken.TokenType == AphidTokenType.definedKeyword)
                    {
                        NextToken();
                        exp = new UnaryOperatorExpression(AphidTokenType.definedKeyword, exp) { IsPostfix = true };
                    }

                    break;

                case AphidTokenType.functionOperator:
                    exp = ParseFunctionExpression();
                    break;

                //case AphidTokenType.forKeyword:
                //    exp = ParseForExpression();
                //    break;

                case AphidTokenType.retKeyword:
                case AphidTokenType.deleteKeyword:
                    exp = ParseUnaryExpression();
                    break;

                case AphidTokenType.trueKeyword:
                    exp = new BooleanExpression(true);
                    NextToken();
                    break;

                case AphidTokenType.falseKeyword:
                    exp = new BooleanExpression(false);
                    NextToken();
                    break;

                case AphidTokenType.thisKeyword:
                    exp = new ThisExpression();
                    NextToken();
                    break;

                //case AphidTokenType.extendKeyword:
                //    exp = ParseExtendExpression();
                //    break;

                //case AphidTokenType.ifKeyword:
                //    exp = ParseIfExpression();
                //    break;

                case AphidTokenType.LoadScriptOperator:
                    exp = ParseLoadScriptExpression();
                    break;

                case AphidTokenType.LoadLibraryOperator:
                    exp = ParseLoadLibraryExpression();
                    break;

                case AphidTokenType.nullKeyword:
                    exp = new NullExpression();
                    NextToken();
                    break;

                case AphidTokenType.breakKeyword:
                    exp = new BreakExpression();
                    NextToken();
                    break;

                case AphidTokenType.HexNumber:
                    exp = new NumberExpression((decimal)Convert.ToInt64(_currentToken.Lexeme.Substring(2), 16));
                    NextToken();
                    break;

                case AphidTokenType.MultiplicationOperator:


                case AphidTokenType.PatternMatchingOperator:
                    var matchExp = new PatternMatchingExpression();
                    NextToken();
                    Match(AphidTokenType.LeftParenthesis);
                    matchExp.TestExpression = ParseExpression();
                    Match(AphidTokenType.RightParenthesis);

                    while (true)
                    {
                        var tests = new List<Expression>();

                        while (true)
                        {
                            tests.Add(ParseExpression());

                            if (_currentToken.TokenType == AphidTokenType.Comma)
                            {
                                NextToken();
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (_currentToken.TokenType == AphidTokenType.ColonOperator)
                        {
                            NextToken();

                            var b = ParseExpression();

                            foreach (var t in tests)
                            {
                                matchExp.Patterns.Add(new Tuple<Expression, Expression>(t, b));
                            }
                        }
                        else
                        {
                            matchExp.Patterns.Add(new Tuple<Expression, Expression>(null, tests[0]));
                        }

                        if (_currentToken.TokenType == AphidTokenType.Comma)
                        {
                            NextToken();
                        }
                        else
                        {
                            break;
                        }
                    }

                    exp = matchExp;
                    break;

                default:
                    throw new AphidParserException(_currentToken);
            }

            return exp;
        }

        private Expression ParseFunctionExpression()
        {
            Expression exp;

            NextToken();

            switch (_currentToken.TokenType)
            {
                case AphidTokenType.LeftParenthesis:
                    var funcExp = new FunctionExpression()
                    {
                        Args = new List<IdentifierExpression>()
                    };

                    NextToken();

                    if (_currentToken.TokenType != AphidTokenType.RightParenthesis)
                    {
                        while (true)
                        {
                            if (_currentToken.TokenType == AphidTokenType.Identifier)
                            {
                                funcExp.Args.Add(ParseIdentifierExpression() as IdentifierExpression);

                                if (_currentToken.TokenType == AphidTokenType.Comma)
                                {
                                    NextToken();
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                throw new AphidParserException(_currentToken);
                            }
                        }
                    }

                    Match(AphidTokenType.RightParenthesis);

                    var isSingleLine = _currentToken.TokenType != AphidTokenType.LeftBrace;

                    var body = ParseBlock(false);

                    if (isSingleLine)
                    {
                        funcExp.Body = new List<Expression> { new UnaryOperatorExpression(AphidTokenType.retKeyword, body[0]) };
                    }
                    else
                    {
                        funcExp.Body = body;
                    }

                    exp = funcExp;

                    break;

                default:

                    exp = new PartialFunctionExpression((CallExpression)ParseCallExpression());
                    break;
            }

            return exp;
        }

        private BinaryOperatorExpression ParseKeyValuePairExpression()
        {
            var id = new IdentifierExpression(_currentToken.Lexeme);
            NextToken();
            Expression exp;

            if (_currentToken.TokenType == AphidTokenType.ColonOperator)
            {
                NextToken();
                exp = ParseExpression();
            }
            else
            {
                exp = id;
            }

            return new BinaryOperatorExpression(id, AphidTokenType.ColonOperator, exp);
        }

        private StringExpression ParseStringExpression()
        {
            var exp = new StringExpression(_currentToken.Lexeme);
            NextToken();
            return exp;
        }

        private ObjectExpression ParseObjectExpression()
        {
            NextToken();

            var inNode = true;

            var childNodes = new List<BinaryOperatorExpression>();

            while (inNode)
            {
                switch (_currentToken.TokenType)
                {
                    case AphidTokenType.Identifier:
                        childNodes.Add(ParseKeyValuePairExpression());

                        switch (_currentToken.TokenType)
                        {
                            case AphidTokenType.Comma:
                                NextToken();
                                break;

                            case AphidTokenType.RightBrace:
                                NextToken();
                                inNode = false;
                                break;

                            default:
                                throw new AphidParserException(_currentToken);
                        }

                        break;

                    case AphidTokenType.RightBrace: // empty object
                        NextToken();
                        inNode = false;
                        break;

                    default:
                        throw new AphidParserException(_currentToken);
                }
            }

            return new ObjectExpression() { Pairs = childNodes };
        }

        public NumberExpression ParseNumberExpression()
        {
            var exp = new NumberExpression(decimal.Parse(_currentToken.Lexeme));
            NextToken();
            return exp;
        }

        public ArrayExpression ParseArrayExpression()
        {
            NextToken();

            var inNode = true;

            var childNodes = new List<Expression>();

            if (_currentToken.TokenType != AphidTokenType.RightBracket)
            {
                while (inNode)
                {
                    childNodes.Add(ParseExpression());

                    switch (_currentToken.TokenType)
                    {
                        case AphidTokenType.Comma:
                            NextToken();

                            if (_currentToken.TokenType == AphidTokenType.RightBracket)
                            {
                                NextToken();
                                inNode = false;
                            }

                            break;

                        case AphidTokenType.RightBracket:
                            NextToken();
                            inNode = false;
                            break;

                        default:
                            throw new AphidParserException(_currentToken);
                    }
                }
            }
            else
            {
                NextToken();
            }

            return new ArrayExpression() { Elements = childNodes };
        }

        private IdentifierExpression ParseIdentifierExpression()
        {
            var exp = new IdentifierExpression(_currentToken.Lexeme);
            NextToken();

            return exp;
        }

        public UnaryOperatorExpression ParseUnaryExpression()
        {
            var t = _currentToken.TokenType;
            NextToken();
            return new UnaryOperatorExpression(t, ParseExpression());
        }

        private Expression ParseCondition()
        {
            Match(AphidTokenType.LeftParenthesis);
            var condition = ParseExpression();
            Match(AphidTokenType.RightParenthesis);
            return condition;
        }

        public IfExpression ParseIfExpression()
        {
            NextToken();
            var condition = ParseCondition();
            var body = ParseBlock();
            List<Expression> elseBody = null;
            if (_currentToken.TokenType == AphidTokenType.elseKeyword)
            {
                NextToken();
                elseBody = ParseBlock();
            }
            return new IfExpression(condition, body, elseBody);
        }

        public ExtendExpression ParseExtendExpression()
        {
            NextToken();
            var id = ((IdentifierExpression)ParseIdentifierExpression()).Identifier;
            var exp = new ExtendExpression(id, ParseObjectExpression());
            return exp;
            //var condition = ParseCondition();
            //var body = ParseBlock();
            //List<Expression> elseBody = null;
            //if (_currentToken.TokenType == AphidTokenType.elseKeyword)
            //{
            //    NextToken();
            //    elseBody = ParseBlock();
            //}
            //return new IfExpression(condition, body, elseBody);
        }

        public Expression ParseForExpression()
        {
            NextToken();
            Match(AphidTokenType.LeftParenthesis);
            var initOrElement = ParseExpression();

            if (_currentToken.TokenType == AphidTokenType.inKeyword)
            {
                NextToken();
                var collection = ParseExpression();
                Match(AphidTokenType.RightParenthesis);
                var body = ParseBlock();
                return new ForEachExpression(collection, initOrElement, body);
            }
            else
            {
                Match(AphidTokenType.EndOfStatement);
                var condition = ParseExpression();
                Match(AphidTokenType.EndOfStatement);
                var afterthought = ParseExpression();
                Match(AphidTokenType.RightParenthesis);
                var body = ParseBlock();
                return new ForExpression(initOrElement, condition, afterthought, body);
            }
        }

        public ControlFlowExpression ParseWhileExpression()
        {
            NextToken();

            return new ControlFlowExpression(
                AphidTokenType.whileKeyword,
                ParseCondition(),
                ParseBlock());
        }

        private List<Expression> ParseTuple()
        {
            var tuple = new List<Expression>();

            while (true)
            {
                tuple.Add(ParseExpression());

                if (_currentToken.TokenType == AphidTokenType.Comma)
                {
                    NextToken();
                }
                else
                {
                    return tuple;
                }
            }
        }

        private List<Expression> ParseBlock(bool requireSingleExpEos = true)
        {
            var statements = new List<Expression>();

            if (_currentToken.TokenType == AphidTokenType.LeftBrace)
            {
                NextToken();

                while (_currentToken.TokenType != AphidTokenType.RightBrace)
                {
                    statements.Add(ParseStatement());
                }

                NextToken();
            }
            else
            {
                statements.Add(ParseStatement(requireSingleExpEos));
            }

            return statements;
        }

        private LoadScriptExpression ParseLoadScriptExpression()
        {
            NextToken();
            return new LoadScriptExpression(ParseExpression());
        }

        private LoadLibraryExpression ParseLoadLibraryExpression()
        {
            NextToken();
            return new LoadLibraryExpression(ParseExpression());
        }

        private Expression ParseTryExpression()
        {
            NextToken();
            var tryExp = new TryExpression() { TryBody = ParseBlock() };

            switch (_currentToken.TokenType)
            {
                case AphidTokenType.catchKeyword:
                    NextToken();
                    Match(AphidTokenType.LeftParenthesis);
                    tryExp.CatchArg = ParseIdentifierExpression();
                    Match(AphidTokenType.RightParenthesis);
                    tryExp.CatchBody = ParseBlock();

                    if (_currentToken.TokenType == AphidTokenType.finallyKeyword)
                    {
                        NextToken();
                        tryExp.FinallyBody = ParseBlock();
                    }

                    break;

                case AphidTokenType.finallyKeyword:
                    NextToken();
                    tryExp.FinallyBody = ParseBlock();
                    break;

                default:
                    throw new AphidParserException(_currentToken);
            }

            return tryExp;
        }

        private Expression ParseSwitchExpression()
        {
            NextToken();
            Match(AphidTokenType.LeftParenthesis);
            var exp = ParseExpression();
            Match(AphidTokenType.RightParenthesis);
            Match(AphidTokenType.LeftBrace);

            var switchExp = new SwitchExpression()
            {
                Expression = exp,
                Cases = new List<SwitchCase>(),
            };

            while (_currentToken.TokenType != AphidTokenType.RightBrace)
            {
                if (_currentToken.TokenType != AphidTokenType.defaultKeyword)
                {
                    var caseTuple = ParseTuple();
                    Match(AphidTokenType.ColonOperator);
                    var block = ParseBlock();
                    switchExp.Cases.Add(new SwitchCase()
                    {
                        Cases = caseTuple,
                        Body = block,
                    });
                }
                else
                {
                    NextToken();
                    Match(AphidTokenType.ColonOperator);
                    switchExp.DefaultCase = ParseBlock();
                }
            }

            NextToken();

            return switchExp;
        }

        private Expression ParseStatement(bool requireEos = true)
        {
            switch (_currentToken.TokenType)
            {
                case AphidTokenType.ifKeyword:
                    return ParseIfExpression();

                case AphidTokenType.forKeyword:
                    return ParseForExpression();

                case AphidTokenType.whileKeyword:
                    return ParseWhileExpression();

                case AphidTokenType.extendKeyword:
                    return ParseExtendExpression();

                case AphidTokenType.tryKeyword:
                    return ParseTryExpression();

                case AphidTokenType.switchKeyword:
                    return ParseSwitchExpression();

                default:
                    var exp = ParseExpression();

                    if (requireEos)
                    {
                        Match(AphidTokenType.EndOfStatement);
                    }

                    return exp;
            }
        }

        public List<Expression> Parse()
        {
            var expressionSequence = new List<Expression>();
            NextToken();

            while (_currentToken.Lexeme != null)
            {
                expressionSequence.Add(ParseStatement());
            }

            return expressionSequence;
        }

        public static List<Expression> Parse(string code)
        {
            return new AphidParser(new AphidLexer(code).GetTokens()).Parse();
        }
    }
}