﻿using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Components.Aphid.Compiler
{
    public abstract partial class AphidStringEmitter
    {
        private Dictionary<AphidTokenType, string> _unaryPrefixOperators = new Dictionary<AphidTokenType, string>
        {
            { AphidTokenType.retKeyword, "ret " },
            { AphidTokenType.NotOperator, "!" },
            { AphidTokenType.MinusOperator, "-" },
            { AphidTokenType.ComplementOperator, "~" },
        };

        private Dictionary<AphidTokenType, string> _unaryPostfixOperators = new Dictionary<AphidTokenType, string>
        {
            { AphidTokenType.IncrementOperator, "++" },
            { AphidTokenType.DecrementOperator, "--" },
        };

        private Dictionary<AphidTokenType, string> _binaryOperators = new Dictionary<AphidTokenType, string>
        {
            { AphidTokenType.AdditionOperator, " + " },
            { AphidTokenType.MinusOperator, " - " },
            { AphidTokenType.MultiplicationOperator, " * " },
            { AphidTokenType.DivisionOperator, " / " },
            { AphidTokenType.ModulusOperator, " % " },

            { AphidTokenType.AndOperator, " && " },
            { AphidTokenType.OrOperator, " || " },

            { AphidTokenType.BinaryAndOperator, " & " },
            { AphidTokenType.BinaryOrOperator, " | " },
            { AphidTokenType.XorOperator,  " ^ " },
            { AphidTokenType.ShiftLeft, " << " },
            { AphidTokenType.ShiftRight, " >> " },
            
            { AphidTokenType.MemberOperator, "." },
            
            { AphidTokenType.EqualityOperator, " == " },
            { AphidTokenType.NotEqualOperator, " != " },
            { AphidTokenType.LessThanOperator, " < " },
            { AphidTokenType.LessThanOrEqualOperator, " <= " },
            { AphidTokenType.GreaterThanOperator, " > " },
            { AphidTokenType.GreaterThanOrEqualOperator, " >= " },

            { AphidTokenType.Comma, ", " },

            { AphidTokenType.AssignmentOperator, " = " },
            { AphidTokenType.PlusEqualOperator, " += " },
            { AphidTokenType.MinusEqualOperator, " -= " },
            { AphidTokenType.MultiplicationEqualOperator, " *= " },
            { AphidTokenType.DivisionEqualOperator, " /= " },
            { AphidTokenType.ModulusEqualOperator, " %= " },
            { AphidTokenType.ShiftLeftEqualOperator, " <<= " },
            { AphidTokenType.ShiftRightEqualOperator, " >>= " },
            { AphidTokenType.BinaryAndEqualOperator, " &= " },
            { AphidTokenType.OrEqualOperator, " |= " },
            { AphidTokenType.XorEqualOperator, " ^= " },
        };

        private Stack<string> _tabs = new Stack<string>();

        protected StringBuilder _out = new StringBuilder();

        private Stack<List<AphidExpression>> _scope = new Stack<List<AphidExpression>>();

        protected Stack<List<AphidExpression>> Scope
        {
            get { return _scope; }
        }

        protected void Indent()
        {
            _tabs.Push("    ");
        }

        protected void Unindent()
        {
            _tabs.Pop();
        }

        protected string GetTabs()
        {
            return string.Join("", _tabs);
        }

        protected void AppendTabs()
        {
            Append(GetTabs());
        }

        protected void AppendLine()
        {
            Append("\r\n");
        }

        protected void Append(string format, params object[] args)
        {
            _out.AppendFormat(format, args);
        }

        protected void Append(string value)
        {
            _out.Append(value);
        }

        protected void Append(object value)
        {
            _out.Append(value.ToString());
        }

        protected virtual void EmitHeader() { }

        public virtual void Emit(List<AphidExpression> statements)
        {
            _scope.Push(statements);

            foreach (var stmt in statements)
            {
                BeginStatement(stmt);

                try
                {
                    Emit(stmt, isStatement: true);
                }
                catch (NotImplementedException)
                {
                    throw new InvalidOperationException(string.Format("{0} is not supported.", stmt.Type));
                }

                EndStatement(stmt);
            }

            _scope.Pop();
        }

        protected virtual void BeginExpression(AphidExpression expression) { }

        protected virtual void EndExpression(AphidExpression expression) { }

        [DebuggerStepThrough]
        public virtual string Compile(string filename, List<AphidExpression> ast)
        {
            var buildNumber = new BuildNumberRepository().NextBuildNumber(filename);
            ast = new BuildConstMutator(buildNumber).Mutate(ast);
            _out.Clear();
            EmitHeader();
            Emit(ast);

            return _out.ToString();
        }

        protected string GetOperator(Dictionary<AphidTokenType, string> table, AphidTokenType op)
        {
            string s;

            if (!table.TryGetValue(op, out s))
            {
                throw new NotImplementedException();
            }

            return s;
        }

        protected virtual string GetUnaryPrefixOperator(AphidTokenType op)
        {
            return GetOperator(_unaryPrefixOperators, op);
        }

        protected virtual string GetUnaryPostfixOperator(AphidTokenType op)
        {
            return GetOperator(_unaryPostfixOperators, op);
        }

        protected virtual string GetBinaryOperator(AphidTokenType op)
        {
            return GetOperator(_binaryOperators, op);
        }

        [DebuggerStepThrough]
        protected virtual void EmitUnaryOperatorExpression(UnaryOperatorExpression expression, bool isStatement = false)
        {
            if (expression.IsPostfix)
            {
                Emit(expression.Operand);
                Append(GetUnaryPostfixOperator(expression.Operator));
            }
            else
            {
                Append(GetUnaryPrefixOperator(expression.Operator));
                Emit(expression.Operand);
            }
        }

        [DebuggerStepThrough]
        protected virtual void EmitBinaryOperatorExpression(BinaryOperatorExpression expression, bool isStatement = false)
        {
            if (!isStatement)
            {
                Append("(");
            }

            Emit(expression.LeftOperand);
            Append(GetBinaryOperator(expression.Operator));
            Emit(expression.RightOperand);

            if (!isStatement)
            {
                Append(")");
            }
        }

        [DebuggerStepThrough]
        protected virtual void EmitStringExpression(StringExpression expression, bool isStatement = false)
        {
            var escaped = StringParser
                .Parse(expression.Value)
                .Replace("\\", "\\\\")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\"", "\\\"");

            Append("\"{0}\"", escaped);
        }

        protected virtual void EmitNumberExpression(NumberExpression expression, bool isStatement = false)
        {
            Append(expression.Value);
        }

        [DebuggerStepThrough]
        protected virtual void EmitNullExpression(NullExpression expression, bool isStatement = false)
        {
            Append("null");
        }

        protected virtual void EmitBooleanExpression(BooleanExpression expression, bool isStatement = false)
        {
            Append(expression.Value ? "true" : "false");
        }

        protected virtual void EmitCallExpression(CallExpression expression, bool isStatement = false)
        {
            Emit(expression.FunctionExpression);
            Append("(");
            EmitTuple(expression.Args);
            Append(")");
        }

        protected virtual void EmitArrayExpression(ArrayExpression expression, bool isStatement = false)
        {
            Append("[");
            EmitTuple(expression.Elements);
            Append("]");
        }

        protected virtual void EmitArrayAccessExpression(ArrayAccessExpression expression, bool isStatement = false)
        {
            Emit(expression.ArrayExpression);
            Append("[");
            Emit(expression.KeyExpression);
            Append("]");
        }

        protected void EmitTuple(IEnumerable<AphidExpression> items, string prefix = "", string suffix = "")
        {
            var first = true;

            foreach (var arg in items)
            {
                if (!first)
                {
                    Append(", ");
                }
                else
                {
                    first = false;
                }

                Append(prefix);
                Emit(arg);
                Append(suffix);
            }
        }
    }
}
