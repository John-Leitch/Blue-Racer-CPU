using Components.Aphid.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class OpcodeTable
    {
        public static Dictionary<string, InstructionOpcode> RegisterAssignments =
            CreateOpcodeTable("Assign_R{0}_Const");

        public static Dictionary<string, InstructionOpcode> OutAssignments =
            new Dictionary<string, InstructionOpcode>
            {
                { "out0", InstructionOpcode.SetOut0_Const },
                { "out1", InstructionOpcode.SetOut1_Const },
                { "out2", InstructionOpcode.SetOut2_Const },
                { "out3", InstructionOpcode.SetOut3_Const },
            };

        public static Dictionary<string, InstructionOpcode> AddConstants =
            CreateOpcodeTable("Add_R{0}_C");

        public static Dictionary<string, InstructionOpcode> SubConstants =
            CreateOpcodeTable("Sub_R{0}_C");

        public static Dictionary<string, InstructionOpcode> MulConstants =
            CreateOpcodeTable("Mul_R{0}_C");

        public static Dictionary<string, InstructionOpcode> DivConstants =
            CreateOpcodeTable("Div_R{0}_C");

        public static Dictionary<string, InstructionOpcode> AndConstants =
            CreateOpcodeTable("And_R{0}_C");

        public static Dictionary<string, InstructionOpcode> OrConstants =
            CreateOpcodeTable("Or_R{0}_C");

        public static Dictionary<string, InstructionOpcode> XorConstants =
            CreateOpcodeTable("Xor_R{0}_C");

        public static Dictionary<string, InstructionOpcode> ShiftLeftConstants =
            CreateOpcodeTable("LeftShift_R{0}_C");

        public static Dictionary<string, InstructionOpcode> ShiftRightConstants =
            CreateOpcodeTable("RightShift_R{0}_C");

        public static Dictionary<string, InstructionOpcode> EqConstants = 
            CreateOpcodeTable("Eq_R{0}_C");

        public static Dictionary<string, InstructionOpcode> NotEqConstants = 
            CreateOpcodeTable("NotEq_R{0}_C");

        public static Dictionary<string, InstructionOpcode> LessThanConstants =
            CreateOpcodeTable("LessThan_R{0}_C");

        public static Dictionary<string, InstructionOpcode> LessThanEqConstants =
            CreateOpcodeTable("LessThanEq_R{0}_C");

        public static Dictionary<string, InstructionOpcode> GreaterThanConstants =
            CreateOpcodeTable("GreaterThan_R{0}_C");

        public static Dictionary<string, InstructionOpcode> GreaterThanEqConstants =
            CreateOpcodeTable("GreaterThanEq_R{0}_C");

        public static Dictionary<string, int> RegisterTable = Create(x => x);

        public static Dictionary<string, int> InTable = new Dictionary<string, int>
        {
            { "in0", 0 },
            { "in1", 1 },
            { "in2", 2 },
            { "in3", 3 },
        };

        public static Dictionary<string, int> OutTable = new Dictionary<string, int>
        {
            { "out0", 0 },
            { "out1", 1 },
            { "out2", 2 },
            { "out3", 3 },
        };

        public static Dictionary<AphidTokenType, InstructionSubOpcode> TokenRegistersSub = new Dictionary<AphidTokenType, InstructionSubOpcode>
        {
            { AphidTokenType.EqualityOperator, InstructionSubOpcode.Eq_R_R },
            { AphidTokenType.NotEqualOperator, InstructionSubOpcode.NotEq_R_R },
            { AphidTokenType.LessThanOperator, InstructionSubOpcode.LessThan_R_R },
            { AphidTokenType.LessThanOrEqualOperator, InstructionSubOpcode.LessThanEq_R_R },
            { AphidTokenType.GreaterThanOperator, InstructionSubOpcode.GreaterThan_R_R },
            { AphidTokenType.GreaterThanOrEqualOperator, InstructionSubOpcode.GreaterThanEq_R_R },
        };

        public static Dictionary<AphidTokenType, InstructionMultiStageSubOpcode> TokenRegistersMulti = new Dictionary<AphidTokenType, InstructionMultiStageSubOpcode>
        {
            { AphidTokenType.PlusEqualOperator, InstructionMultiStageSubOpcode.Add_R_R },
            { AphidTokenType.MinusEqualOperator, InstructionMultiStageSubOpcode.Sub_R_R },
            { AphidTokenType.MultiplicationEqualOperator, InstructionMultiStageSubOpcode.Mul_R_R },
            { AphidTokenType.DivisionEqualOperator, InstructionMultiStageSubOpcode.Div_R_R },
            { AphidTokenType.BinaryAndOperator, InstructionMultiStageSubOpcode.And_R_R },
            { AphidTokenType.OrEqualOperator, InstructionMultiStageSubOpcode.Or_R_R },
            { AphidTokenType.XorEqualOperator, InstructionMultiStageSubOpcode.Xor_R_R },
            { AphidTokenType.ShiftLeft, InstructionMultiStageSubOpcode.LeftShift_R_R },
            { AphidTokenType.ShiftRight, InstructionMultiStageSubOpcode.RightShift_R_R },
        };

        public static Dictionary<AphidTokenType, Dictionary<string, InstructionOpcode>> TokenConstants = new Dictionary<AphidTokenType,Dictionary<string,InstructionOpcode>>()
        {
            { AphidTokenType.PlusEqualOperator, AddConstants },
            { AphidTokenType.MinusEqualOperator, SubConstants },
            { AphidTokenType.MultiplicationEqualOperator, MulConstants },
            { AphidTokenType.DivisionEqualOperator, DivConstants },
            { AphidTokenType.BinaryAndOperator, AndConstants },
            { AphidTokenType.OrEqualOperator, OrConstants },
            { AphidTokenType.XorEqualOperator, XorConstants },
            { AphidTokenType.ShiftLeft, ShiftLeftConstants },
            { AphidTokenType.ShiftRight, ShiftRightConstants },
            { AphidTokenType.EqualityOperator, EqConstants },
            { AphidTokenType.NotEqualOperator, NotEqConstants },
            { AphidTokenType.LessThanOperator, LessThanConstants },
            { AphidTokenType.LessThanOrEqualOperator, LessThanEqConstants },
            { AphidTokenType.GreaterThanOperator, GreaterThanConstants },
            { AphidTokenType.GreaterThanOrEqualOperator, GreaterThanEqConstants },
        };

        private static Dictionary<string, TKey> Create<TKey>(Func<int, TKey> keyFunc)
        {
            return Enumerable.Range(0, Register.Count).ToDictionary(x => "r" + x, x => keyFunc(x));
        }

        private static Dictionary<string, InstructionOpcode> CreateOpcodeTable(string format)
        {
            return Create(
                x => (InstructionOpcode)Enum.Parse(typeof(InstructionOpcode), string.Format(format, x)));
        }
    }
}
