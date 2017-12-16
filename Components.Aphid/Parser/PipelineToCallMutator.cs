﻿using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class PipelineToCallMutator : AphidMutator
    {
        protected override List<AphidExpression> MutateCore(AphidExpression expression, out bool hasChanged)
        {
            BinaryOperatorExpression binOpExp;

            if (expression.Type != AphidExpressionType.BinaryOperatorExpression ||
                (binOpExp = (BinaryOperatorExpression)expression).Operator != AphidTokenType.PipelineOperator)
            {
                hasChanged = false;

                return null;
            }

            hasChanged = true;

            return new List<AphidExpression> 
            { 
                new CallExpression(binOpExp.RightOperand, binOpExp.LeftOperand) 
            };
        }
    }
}
