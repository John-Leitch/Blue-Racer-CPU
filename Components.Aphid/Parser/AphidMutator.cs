﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public abstract class AphidMutator
    {
        public bool HasMutated { get; private set; }

        protected abstract List<AphidExpression> MutateCore(AphidExpression expression, out bool hasChanged);

        protected virtual List<AphidExpression> OnMutate(List<AphidExpression> ast)
        {
            return ast;
        }

        public List<AphidExpression> Mutate(List<AphidExpression> ast)
        {
            if (ast == null)
            {
                return null;
            }

            ast = OnMutate(ast);

            var ast2 = new List<AphidExpression>();

            foreach (var exp in ast)
            {
                ast2.AddRange(Mutate(exp));
            }

            return ast2;
        }

        public List<AphidExpression> Mutate(AphidExpression expression)
        {
            bool hasChanged;
            var mutated = MutateCore(expression, out hasChanged);

            if (hasChanged)
            {
                HasMutated = true;
                return mutated.SelectMany(Mutate).ToList();
            }

            var expanded = new List<AphidExpression>();

            switch (expression.Type)
            {
                case AphidNodeType.IdentifierExpression:
                    var id = (IdentifierExpression)expression;

                    expanded.Add(new IdentifierExpression(
                        id.Identifier,
                        id.Attributes
                            .Select(x => (IdentifierExpression)Mutate(x).Single())
                            .ToList()));
                    break;

                case AphidNodeType.CallExpression:
                    var call = (CallExpression)expression;

                    expanded.Add(new CallExpression(
                        Mutate(call.FunctionExpression).Single(),
                        call.Args.Select(x => Mutate(x).Single()).ToArray()));

                    break;

                case AphidNodeType.UnaryOperatorExpression:
                    var unOp = (UnaryOperatorExpression)expression;

                    expanded.Add(new UnaryOperatorExpression(
                        unOp.Operator,
                        Mutate(unOp.Operand).Single())
                        {
                            IsPostfix = unOp.IsPostfix
                        });

                    break;

                case AphidNodeType.BinaryOperatorExpression:
                    var binOp = (BinaryOperatorExpression)expression;

                    expanded.Add(new BinaryOperatorExpression(
                        Mutate(binOp.LeftOperand).Single(),
                        binOp.Operator,
                        Mutate(binOp.RightOperand).Single()));

                    break;

                case AphidNodeType.IfExpression:
                    var ifExp = (IfExpression)expression;

                    expanded.Add(new IfExpression(
                        Mutate(ifExp.Condition).Single(),
                        Mutate(ifExp.Body),
                        Mutate(ifExp.ElseBody)));

                    break;

                case AphidNodeType.ForExpression:
                    var forExp = (ForExpression)expression;

                    expanded.Add(new ForExpression(
                        Mutate(forExp.Initialization).Single(),
                        Mutate(forExp.Condition).Single(),
                        Mutate(forExp.Afterthought).Single(),
                        Mutate(forExp.Body)));

                    break;

                case AphidNodeType.ForEachExpression:
                    var forEachExp = (ForEachExpression)expression;

                    expanded.Add(
                        new ForEachExpression(
                            Mutate(forEachExp.Collection).Single(),
                            Mutate(forEachExp.Element).Single(),
                            Mutate(forEachExp.Body)));

                    break;

                case AphidNodeType.WhileExpression:
                    var cfExp = (WhileExpression)expression;
                    expanded.Add(new WhileExpression(Mutate(cfExp.Condition).Single(), Mutate(cfExp.Body)));
                    break;

                case AphidNodeType.LoadScriptExpression:
                    var lsExp = (LoadScriptExpression)expression;
                    expanded.Add(new LoadScriptExpression(Mutate(lsExp.FileExpression).Single()));
                    break;

                case AphidNodeType.LoadLibraryExpression:
                    var llExp = (LoadLibraryExpression)expression;
                    expanded.Add(new LoadLibraryExpression(Mutate(llExp.LibraryExpression).Single()));
                    break;

                case AphidNodeType.FunctionExpression:
                    var funcExp = (FunctionExpression)expression;

                    expanded.Add(new FunctionExpression()
                    {
                        Args = funcExp.Args.Select(x => Mutate(x).Single()).ToList(),
                        Body = funcExp.Body.SelectMany(x => Mutate(x)).ToList()
                    });

                    break;

                case AphidNodeType.ArrayExpression:
                    var arrayExp = (ArrayExpression)expression;

                    expanded.Add(new ArrayExpression()
                    {
                        Elements = arrayExp.Elements.Select(x => Mutate(x).Single()).ToList()
                    });

                    break;

                case AphidNodeType.ArrayAccessExpression:
                    var arrayAccessExp = (ArrayAccessExpression)expression;

                    expanded.Add(new ArrayAccessExpression(
                        Mutate(arrayAccessExp.ArrayExpression).Single(),
                        Mutate(arrayAccessExp.KeyExpression).Single()));

                    break;

                case AphidNodeType.ObjectExpression:
                    var pairs = ((ObjectExpression)expression).Pairs
                        .Select(x => (BinaryOperatorExpression)Mutate(x).Single())
                        .ToList();

                    expanded.Add(new ObjectExpression(pairs));
                    break;

                case AphidNodeType.ExtendExpression:
                    var extendExp = (ExtendExpression)expression;

                    expanded.Add(new ExtendExpression(
                        extendExp.ExtendType,
                        (ObjectExpression)Mutate(extendExp.Object).Single()));

                    break;

                case AphidNodeType.TernaryOperatorExpression:
                    var terExp = (TernaryOperatorExpression)expression;

                    expanded.Add(
                        new TernaryOperatorExpression(
                            terExp.Operator,
                            Mutate(terExp.FirstOperand).Single(),
                            Mutate(terExp.SecondOperand).Single(),
                            Mutate(terExp.ThirdOperand).Single()));

                    break;

                case AphidNodeType.DynamicMemberExpression:
                    var dynExp = (DynamicMemberExpression)expression;

                    expanded.Add(
                        new DynamicMemberExpression(
                            Mutate(dynExp.MemberExpression).Single()));

                    break;

                default:
                    if (expression is IParentNode)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        expanded.Add(expression);
                    }

                    break;
            }

            return expanded;
        }

        public List<AphidExpression> MutateRecursively(List<AphidExpression> expression)
        {
            List<AphidExpression> ast = expression;

            var anyMutations = false;

            do
            {
                Reset();
                ast = Mutate(ast);

                if (HasMutated)
                {
                    anyMutations = true;
                }
            } while (HasMutated);

            HasMutated = anyMutations;

            return ast;
        }

        public void Reset()
        {
            HasMutated = false;
        }
    }
}
