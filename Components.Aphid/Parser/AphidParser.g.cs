namespace Components.Aphid.Parser {
    
    
    public partial class AphidParser {
        
        private Components.Aphid.Parser.AphidExpression ParseTerm() {
            Components.Aphid.Parser.AphidExpression operand = this.ParsePrefixUnaryOperatorExpression();
            for (
            ; (((this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.MultiplicationOperator) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.DivisionOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.ModulusOperator)); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParsePrefixUnaryOperatorExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseAdditionExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseTerm();
            for (
            ; ((this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.AdditionOperator) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.MinusOperator)); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseTerm());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseShiftExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseAdditionExpression();
            for (
            ; ((this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.ShiftLeft) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.ShiftRight)); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseAdditionExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseBinaryAndExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseShiftExpression();
            for (
            ; (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.BinaryAndOperator); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseShiftExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseXorExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseBinaryAndExpression();
            for (
            ; (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.XorOperator); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseBinaryAndExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseBinaryOrExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseXorExpression();
            for (
            ; (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.BinaryOrOperator); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseXorExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseComparisonExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParsePostfixUnaryOperationExpression();
            for (
            ; ((((((this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.EqualityOperator) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.NotEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.LessThanOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.LessThanOrEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.GreaterThanOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.GreaterThanOrEqualOperator)); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParsePostfixUnaryOperationExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseLogicalExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseComparisonExpression();
            for (
            ; ((this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.AndOperator) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.OrOperator)); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseComparisonExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseRangeExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseConditionalExpression();
            for (
            ; (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.RangeOperator); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseConditionalExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParsePipelineExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParseQueryExpression();
            for (
            ; (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.PipelineOperator); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParseQueryExpression());
            }
            return operand;
        }
        
        private Components.Aphid.Parser.AphidExpression ParseAssignmentExpression() {
            Components.Aphid.Parser.AphidExpression operand = this.ParsePipelineExpression();
            for (
            ; (((((((((((this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.AssignmentOperator) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.PlusEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.MinusEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.MultiplicationEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.DivisionEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.ModulusEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.BinaryAndEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.OrEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.XorEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.ShiftLeftEqualOperator)) 
                        || (this._currentToken.TokenType == Components.Aphid.Lexer.AphidTokenType.ShiftRightEqualOperator)); 
            ) {
                Components.Aphid.Lexer.AphidTokenType op = this._currentToken.TokenType;
                this.NextToken();
                operand = new Components.Aphid.Parser.BinaryOperatorExpression(operand, op, this.ParsePipelineExpression());
            }
            return operand;
        }
    }
}
