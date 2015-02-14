﻿using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Components.Aphid.Parser
{
    public class WhileExpression : ControlFlowExpression
    {
        public override AphidNodeType Type
        {
            get { return AphidNodeType.WhileExpression; }
        }

        public WhileExpression(AphidExpression condition, List<AphidExpression> body)
            : base(AphidTokenType.whileKeyword, condition, body)
        {
        }
    }
}
