using Components;
using Components.Aphid.Lexer;
using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class AphidAssembler
    {
        private uint _currentAddress;

        private Dictionary<string, uint> _labels = new Dictionary<string, uint>();

        private List<KeyValuePair<string, byte[]>> _needLabelAddresses = new List<KeyValuePair<string, byte[]>>();

        private AphidPreprocessor _preprocessor = new AphidPreprocessor();

        private byte[] GetBytes(uint u)
        {
            return BitConverter.GetBytes(u).Reverse().ToArray();
        }

        public byte[] AssembleNumber(NumberExpression expression)
        {
            return GetBytes((uint)(expression).Value);
        }

        private string GetLeftId(BinaryOperatorExpression exp)
        {
            return ((IdentifierExpression)exp.LeftOperand).Identifier;
        }

        private string GetRightId(BinaryOperatorExpression exp)
        {
            return ((IdentifierExpression)exp.RightOperand).Identifier;
        }

        private string GetReadWriteId(AphidExpression exp)
        {
            if (exp is IdentifierExpression)
            {
                return ((IdentifierExpression)exp).Identifier;
            }
            else if (exp is UnaryOperatorExpression)
            {
                var unExp = (UnaryOperatorExpression)exp;

                if (unExp.IsPostfix || unExp.Operator != AphidTokenType.MultiplicationOperator)
                {
                    throw new InvalidOperationException();
                }

                if (unExp.Operand is IdentifierExpression)
                {
                    return ((IdentifierExpression)unExp.Operand).Identifier;
                }
                else if (unExp.Operand is BinaryOperatorExpression)
                {
                    var binOp = (BinaryOperatorExpression)unExp.Operand;

                    switch (binOp.Operator)
                    {
                        case AphidTokenType.AdditionOperator:
                        case AphidTokenType.MinusEqualOperator:
                            if (binOp.LeftOperand is IdentifierExpression)
                            {
                                return GetLeftId(binOp);
                            }
                            else if (binOp.RightOperand is IdentifierExpression)
                            {
                                return GetRightId(binOp);
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }

                        default:
                            throw new NotImplementedException();
                    }
                }
                else 
                {
                    throw new NotImplementedException();
                }


            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private uint GetUInt(AphidExpression expression)
        {
            return (uint)((NumberExpression)expression).Value;
        }

        private int GetOffset(AphidExpression expression)
        {
            if (!(expression is UnaryOperatorExpression))
            {
                return 0;
            }

            var unOp = (UnaryOperatorExpression)expression;

            if (!(unOp.Operand is BinaryOperatorExpression))
            {
                return 0;
            }

            var binOp = (BinaryOperatorExpression)unOp.Operand;
            
            int offset;

            if (binOp.LeftOperand is NumberExpression)
            {
                offset = (int)GetUInt(binOp.LeftOperand);
            }
            else if (binOp.RightOperand is NumberExpression)
            {
                offset = (int)GetUInt(binOp.RightOperand);
            }
            else
            {
                throw new InvalidOperationException();
            }

            if (binOp.Operator == AphidTokenType.MinusOperator)
            {
                offset *= -1;
            }
            else if (binOp.Operator != AphidTokenType.AdditionOperator)
            {
                throw new NotImplementedException();
            }

            return offset;
        }

        private byte[] AssembleReadWrite(BinaryOperatorExpression expression)
        {
            if (expression.LeftOperand is UnaryOperatorExpression &&
                expression.RightOperand is UnaryOperatorExpression)
            {
                throw new InvalidOperationException();
            }

            var b = new byte[5];
            var leftId = GetReadWriteId(expression.LeftOperand);
            var rightId = GetReadWriteId(expression.RightOperand);
            var leftOffset = GetOffset(expression.LeftOperand);
            var rightOffset = GetOffset(expression.RightOperand);
            
            if (leftOffset != 0 && rightOffset != 0)
            {
                throw new InvalidOperationException();
            }

            var offset = leftOffset != 0 ? leftOffset : rightOffset;

            if (offset > byte.MaxValue)
            {
                throw new InvalidOperationException();
            }

            b[0] = (byte)InstructionOpcode.ExtMultiStage;
            b[2] = (byte)OpcodeTable.RegisterTable[leftId];
            b[3] = (byte)OpcodeTable.RegisterTable[rightId];   

            if (expression.LeftOperand is UnaryOperatorExpression)
            {
                b[1] = (byte)InstructionMultiStageSubOpcode.Write;

                if (offset > 0)
                {
                    b[2] += Register.Count;
                }
                else if (offset < 0)
                {
                    b[2] += Register.Count * 2;
                }
            }
            else if (expression.RightOperand is UnaryOperatorExpression)
            {
                b[1] = (byte)InstructionMultiStageSubOpcode.Read;

                if (offset > 0)
                {
                    b[3] += Register.Count;
                }
                else if (offset < 0)
                {
                    b[3] += Register.Count * 2;
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            b[4] = (byte)offset;         

            return b;
        }

        public byte[] AssembleBinaryOperation(BinaryOperatorExpression expression)
        {
            var b = new byte[5];

            switch (expression.Operator)
            {
                case AphidTokenType.AssignmentOperator:

                    if (expression.LeftOperand is UnaryOperatorExpression ||
                        expression.RightOperand is UnaryOperatorExpression)
                    {
                        return AssembleReadWrite(expression);
                    }

                    var id = ((IdentifierExpression)expression.LeftOperand).Identifier;

                    if (expression.RightOperand is NumberExpression)
                    {
                        if (OpcodeTable.RegisterAssignments.ContainsKey(id))
                        {
                            b[0] = (byte)OpcodeTable.RegisterAssignments[id];
                        }
                        else if (OpcodeTable.OutAssignments.ContainsKey(id))
                        {
                            b[0] = (byte)OpcodeTable.OutAssignments[id];
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        AssembleNumber((NumberExpression)expression.RightOperand).CopyTo(b, 1);
                    }
                    else if (expression.RightOperand is IdentifierExpression)
                    {
                        var rightId = ((IdentifierExpression)expression.RightOperand).Identifier;

                        if (OpcodeTable.RegisterTable.ContainsKey(rightId))
                        {
                            b[0] = (byte)InstructionOpcode.Ext;

                            if (OpcodeTable.RegisterTable.ContainsKey(id))
                            {
                                b[1] = (byte)InstructionSubOpcode.Assign;
                                b[2] = (byte)OpcodeTable.RegisterTable[id];
                            }
                            else if (OpcodeTable.OutTable.ContainsKey(id))
                            {
                                b[1] = (byte)InstructionSubOpcode.Out;
                                b[2] = (byte)OpcodeTable.OutTable[id];
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }

                            b[3] = (byte)OpcodeTable.RegisterTable[rightId];
                        }
                        else if (OpcodeTable.InTable.ContainsKey(rightId))
                        {
                            b[0] = (byte)InstructionOpcode.Ext;
                            b[1] = (byte)InstructionSubOpcode.In;
                            b[2] = (byte)OpcodeTable.RegisterTable[id];
                            b[3] = (byte)OpcodeTable.InTable[rightId];
                        }
                        else
                        {
                            b[0] = (byte)OpcodeTable.RegisterAssignments[id];

                            if (_labels.ContainsKey(rightId))
                            {
                                GetBytes(_labels[rightId]).CopyTo(b, 1);
                            }
                            else
                            {
                                _needLabelAddresses.Add(new KeyValuePair<string, byte[]>(rightId, b));
                            }
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    break;

                case AphidTokenType.PlusEqualOperator:
                case AphidTokenType.MinusEqualOperator:
                case AphidTokenType.MultiplicationEqualOperator:
                case AphidTokenType.DivisionEqualOperator:
                case AphidTokenType.BinaryAndEqualOperator:
                case AphidTokenType.ShiftLeftEqualOperator:
                case AphidTokenType.ShiftRightEqualOperator:
                case AphidTokenType.OrEqualOperator:
                case AphidTokenType.XorEqualOperator:
                    if (expression.LeftOperand is IdentifierExpression)
                    {
                        if (expression.RightOperand is IdentifierExpression)
                        {
                            b[0] = (byte)InstructionOpcode.ExtMultiStage;
                            b[1] = (byte)OpcodeTable.TokenRegistersMulti[expression.Operator];
                            b[2] = (byte)OpcodeTable.RegisterTable[GetLeftId(expression)];
                            b[3] = (byte)OpcodeTable.RegisterTable[GetRightId(expression)];
                        }
                        else if (expression.RightOperand is NumberExpression)
                        {
                            var opTable = OpcodeTable.TokenConstants[expression.Operator];
                            b[0] = (byte)opTable[GetLeftId(expression)];
                            GetBytes((uint)((NumberExpression)expression.RightOperand).Value).CopyTo(b, 1);
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    break;

                case AphidTokenType.EqualityOperator:
                case AphidTokenType.NotEqualOperator:
                case AphidTokenType.LessThanOperator:
                case AphidTokenType.LessThanOrEqualOperator:
                case AphidTokenType.GreaterThanOperator:
                case AphidTokenType.GreaterThanOrEqualOperator:
                    if (expression.LeftOperand is IdentifierExpression)
                    {
                        if (expression.RightOperand is NumberExpression)
                        {
                            var table = OpcodeTable.TokenConstants[expression.Operator];
                            b[0] = (byte)table[GetLeftId(expression)];
                            AssembleNumber((NumberExpression)expression.RightOperand).CopyTo(b, 1);
                        }
                        else if (expression.RightOperand is IdentifierExpression)
                        {
                            b[0] = (byte)InstructionOpcode.Ext;
                            b[1] = (byte)OpcodeTable.TokenRegistersSub[expression.Operator];
                            b[2] = (byte)OpcodeTable.RegisterTable[GetLeftId(expression)];
                            b[3] = (byte)OpcodeTable.RegisterTable[GetRightId(expression)];
                        }
                        else
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                    break;

                default:
                    throw new InvalidOperationException();
            }

            return b;
        }

        public byte[] AssembleUnaryOperation(UnaryOperatorExpression expression)
        {
            var b = new byte[5];

            switch (expression.Operator)
            {
                case AphidTokenType.IncrementOperator:
                case AphidTokenType.DecrementOperator:
                    b[0] = (byte)InstructionOpcode.Ext;
                    b[1] = expression.Operator == AphidTokenType.IncrementOperator ?
                        (byte)InstructionSubOpcode.Inc_R : (byte)InstructionSubOpcode.Dec_R;

                    b[2] = (byte)OpcodeTable.RegisterTable[((IdentifierExpression)expression.Operand).Identifier];
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return b;
        }

        public void AddLabel(IdentifierExpression exp)
        {
            _labels.Add(exp.Identifier, _currentAddress);

            var needed = _needLabelAddresses.Where(x => x.Key == exp.Identifier).ToArray();
            foreach (var inst in needed)
            {
                GetBytes(_currentAddress).CopyTo(inst.Value, 1);
            }

            foreach (var n in needed)
            {
                _needLabelAddresses.Remove(n);
            }
        }

        public byte[] AssembleCallExpression(CallExpression expression)
        {
            var b = new byte[5];
            var funcName = ((IdentifierExpression)expression.FunctionExpression).Identifier;

            switch (funcName)
            {
                case InstructionMnemonic.Out0:
                case InstructionMnemonic.Out1:
                case InstructionMnemonic.Out2:
                case InstructionMnemonic.Out3:
                    var id = expression.Args.Cast<IdentifierExpression>().Single().Identifier;
                    b[0] = (byte)InstructionOpcode.Ext;
                    b[1] = (byte)InstructionSubOpcode.Out;
                    b[2] = byte.Parse(funcName[3].ToString());
                    b[3] = (byte)OpcodeTable.RegisterTable[id];

                    break;

                case InstructionMnemonic.Goto:
                case InstructionMnemonic.GotoTrue:
                case InstructionMnemonic.GotoFalse:
                    id = expression.Args.Cast<IdentifierExpression>().Single().Identifier;



                    InstructionOpcode oc;

                    switch (funcName)
                    {
                        case InstructionMnemonic.Goto:
                            oc = InstructionOpcode.Goto_Const;
                            break;

                        case InstructionMnemonic.GotoTrue:
                            oc = InstructionOpcode.Goto_Eq;
                            break;

                        case InstructionMnemonic.GotoFalse:
                            oc = InstructionOpcode.Goto_NotEq;
                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    b[0] = (byte)oc;

                    if (!_labels.ContainsKey(id))
                    {
                        _needLabelAddresses.Add(new KeyValuePair<string, byte[]>(id, b));
                        //throw new InvalidOperationException();
                    }
                    else
                    {
                        GetBytes(_labels[id]).CopyTo(b, 1);
                    }

                    break;

                case InstructionMnemonic.Read:
                case InstructionMnemonic.Write:
                    var id2 = ((IdentifierExpression)expression.Args.ToArray()[0]).Identifier;
                    var id3 = ((IdentifierExpression)expression.Args.ToArray()[1]).Identifier;
                    b[0] = (byte)InstructionOpcode.ExtMultiStage;

                    switch (funcName)
                    {
                        case InstructionMnemonic.Read:
                            b[1] = (byte)InstructionMultiStageSubOpcode.Read;
                            break;

                        case InstructionMnemonic.Write:
                            b[1] = (byte)InstructionMultiStageSubOpcode.Write;
                            break;

                        default:
                            throw new InvalidOperationException();
                    }

                    b[2] = (byte)OpcodeTable.RegisterTable[id2];
                    b[3] = (byte)OpcodeTable.RegisterTable[id3];
                    break;

                case InstructionMnemonic.Error:
                    b = new[] 
                    { 
                        (byte)InstructionOpcode.Error,
                        (byte)InstructionOpcode.Error,
                        (byte)InstructionOpcode.Error,
                        (byte)InstructionOpcode.Error,
                        (byte)InstructionOpcode.Error,
                    };
                    break;

                case InstructionMnemonic.Data:
                    var exp = expression.Args.Single();

                    if (exp is StringExpression)
                    {
                        return StringParser
                            .Parse(((StringExpression)exp).Value)
                            .Select(x => (byte)x)
                            .ToArray();
                    }
                    else
                    {
                        throw new InvalidOperationException();
                    }

                case InstructionMnemonic.Push:
                case InstructionMnemonic.Pop:
                    exp = expression.Args.Single();                    

                    if (exp is IdentifierExpression)
                    {
                        var ppId = ((IdentifierExpression)exp).Identifier;

                        if (OpcodeTable.RegisterTable.ContainsKey(ppId))
                        {
                            b[0] = (byte)InstructionOpcode.ExtMultiStage;

                            switch (funcName)
                            {
                                case InstructionMnemonic.Push:
                                    b[1] = (byte)InstructionMultiStageSubOpcode.Push_R;
                                    break;

                                case InstructionMnemonic.Pop:
                                    b[1] = (byte)InstructionMultiStageSubOpcode.Pop_R;
                                    break;

                                default:
                                    throw new InvalidOperationException();
                            }

                            b[2] = (byte)OpcodeTable.RegisterTable[ppId];
                        }
                        else
                        {
                            switch (funcName)
                            {
                                case InstructionMnemonic.Push:
                                    b[0] = (byte)InstructionOpcode.Push_Const;
                                    break;                                

                                default:
                                    throw new InvalidOperationException();
                            }

                            if (_labels.ContainsKey(ppId))
                            {
                                GetBytes(_labels[ppId]).CopyTo(b, 1);
                            }
                            else
                            {
                                _needLabelAddresses.Add(new KeyValuePair<string, byte[]>(ppId, b));
                            }
                        }
                    }
                    else if (exp is NumberExpression)
                    {
                        b[0] = (byte)InstructionOpcode.Push_Const;
                        
                        BitConverter
                            .GetBytes(GetUInt(exp))
                            .Reverse()
                            .ToArray()
                            .CopyTo(b, 1);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    break;

                case InstructionMnemonic.Call:
                    exp = expression.Args.Single();
                    var idArg = ((IdentifierExpression)exp).Identifier;

                    if (OpcodeTable.RegisterTable.ContainsKey(idArg))
                    {
                        b[0] = (byte)InstructionOpcode.ExtMultiStage;
                        b[1] = (byte)InstructionMultiStageSubOpcode.Call_R;
                        b[2] = (byte)OpcodeTable.RegisterTable[idArg];
                    }
                    else
                    {
                        b[0] = (byte)InstructionOpcode.Call_Const;

                        if (_labels.ContainsKey(idArg))
                        {
                            GetBytes(_labels[idArg]).CopyTo(b, 1);
                        }
                        else
                        {
                            _needLabelAddresses.Add(new KeyValuePair<string, byte[]>(
                                idArg, b));
                        }
                    }

                    break;

                case InstructionMnemonic.Return:
                    if (expression.Args.Any())
                    {
                        throw new NotImplementedException();
                    }

                    b[0] = (byte)InstructionOpcode.ExtMultiStage;
                    b[1] = (byte)InstructionMultiStageSubOpcode.Return;

                    break;

                default:
                    throw new InvalidOperationException();
            }

            return b;
        }

        public byte[] Assemble(AphidExpression expression)
        {
            if (expression is BinaryOperatorExpression)
            {
                return AssembleBinaryOperation(expression as BinaryOperatorExpression);
            }
            else if (expression is IdentifierExpression)
            {
                AddLabel(expression as IdentifierExpression);

                return null;
            }
            else if (expression is UnaryOperatorExpression)
            {
                return AssembleUnaryOperation(expression as UnaryOperatorExpression);
            }
            else if (expression is CallExpression)
            {
                return AssembleCallExpression(expression as CallExpression);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public List<byte[]> Assemble(string asm)
        {
            _currentAddress = 0;
            _labels.Clear();

            var ast = AphidParser.Parse(asm);

            ast = new AphidIncludeMutator().MutateRecursively(ast);
            
            //ast = _preprocessor.ExpandControlFlowExpressions(ast);
            for (int i = 0; i < 4; i++)
                ast = _preprocessor.ExpandMacros(ast);

            

            AphidControlFlowMutator cfMutator;
            do
            {
                cfMutator = new AphidControlFlowMutator();
                ast = cfMutator.Mutate(ast);
            } while (cfMutator.HasMutated);

            ast = new AphidCallMutator().Mutate(ast);
            ast = new ThreeOperandMutator().Mutate(ast);

            var instructions = new List<byte[]>();

            foreach (var exp in ast)
            {
                var b = Assemble(exp);

                if (b != null)
                {
                    instructions.Add(b);
                    _currentAddress += (uint)b.Length;
                }
            }

            var bytes = instructions
                .SelectMany(x => x)
                .ToArray()
                .Select(x => string.Format("{0:X2}", x))
                .Join();

            if (_needLabelAddresses.Any())
            {
                throw new InvalidOperationException();
            }

            return instructions;
        }

        public void AssembleToVerilog(string asm, string verilogFile)
        {
            var bytes = Assemble(asm).SelectMany(x => x).ToArray();
            var padding = 5 - (bytes.Length % 5);

            if (padding == 5)
            {
                padding = 0;
            }

            Array.Resize(ref bytes, bytes.Length + padding);

            if (bytes.Length > 0xffff)
            {
                throw new InvalidOperationException();
            }

            var asmStr = bytes
                .GroupEvery(5)
                .Select((x, i) => string.Format(
                    "{0}: writeInstructionBuffer <= 40'h{1};",
                    i * 5,
                    ByteHelper.ToHex(x.ToArray())))
                .Join("\r\n");

            var doc3 = new VerilogDocument();
            doc3.AddDefineString("Program", asmStr);
            doc3.AddDefine16("ProgramSize", (ushort)bytes.Length);
            File.WriteAllText(verilogFile, doc3.ToString());

            //return null;
            //return 
            //    .Select((x, i) => string.Format(
            //        "{0}: writeInstructionBuffer <= 40'h{1};",
            //        i * 5,
            //        ByteHelper.ToHex(x)))
            //    .Join("\r\n");
        }

        public void AssembleToBin(string asm, string binFile)
        {
            File.WriteAllBytes(binFile, Assemble(asm).SelectMany(x => x).ToArray());
        }

        public void AssembleFileToBin(string file, string binFile)
        {
            AssembleToBin(File.ReadAllText(file), binFile);
        }
    }
}
