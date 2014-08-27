`define MultiStageOpcodes `Push_Const, `Add_R0_C, `Add_R1_C, `Add_R2_C, `Add_R3_C, `Add_R4_C, `Add_R5_C, `Add_R6_C, `Add_R7_C, `Sub_R0_C, `Sub_R1_C, `Sub_R2_C, `Sub_R3_C, `Sub_R4_C, `Sub_R5_C, `Sub_R6_C, `Sub_R7_C, `Mul_R0_C, `Mul_R1_C, `Mul_R2_C, `Mul_R3_C, `Mul_R4_C, `Mul_R5_C, `Mul_R6_C, `Mul_R7_C, `Div_R0_C, `Div_R1_C, `Div_R2_C, `Div_R3_C, `Div_R4_C, `Div_R5_C, `Div_R6_C, `Div_R7_C, `And_R0_C, `And_R1_C, `And_R2_C, `And_R3_C, `And_R4_C, `And_R5_C, `And_R6_C, `And_R7_C, `Or_R0_C, `Or_R1_C, `Or_R2_C, `Or_R3_C, `Or_R4_C, `Or_R5_C, `Or_R6_C, `Or_R7_C, `LeftShift_R0_C, `LeftShift_R1_C, `LeftShift_R2_C, `LeftShift_R3_C, `LeftShift_R4_C, `LeftShift_R5_C, `LeftShift_R6_C, `LeftShift_R7_C, `RightShift_R0_C, `RightShift_R1_C, `RightShift_R2_C, `RightShift_R3_C, `RightShift_R4_C, `RightShift_R5_C, `RightShift_R6_C, `RightShift_R7_C, `Xor_R0_C, `Xor_R1_C, `Xor_R2_C, `Xor_R3_C, `Xor_R4_C, `Xor_R5_C, `Xor_R6_C, `Xor_R7_C, `Call_Const, `Goto_Const, `Goto_Eq, `Goto_NotEq, `ExtMultiStage

`define DivideLatency 8'h1E
`define AddCases  \
`Add_R0_C: AddR0Constant(); \
`Add_R1_C: AddR1Constant(); \
`Add_R2_C: AddR2Constant(); \
`Add_R3_C: AddR3Constant(); \
`Add_R4_C: AddR4Constant(); \
`Add_R5_C: AddR5Constant(); \
`Add_R6_C: AddR6Constant(); \
`Add_R7_C: AddR7Constant();

`define SubtractCases  \
`Sub_R0_C: SubtractR0Constant(); \
`Sub_R1_C: SubtractR1Constant(); \
`Sub_R2_C: SubtractR2Constant(); \
`Sub_R3_C: SubtractR3Constant(); \
`Sub_R4_C: SubtractR4Constant(); \
`Sub_R5_C: SubtractR5Constant(); \
`Sub_R6_C: SubtractR6Constant(); \
`Sub_R7_C: SubtractR7Constant();

`define MultiplyCases  \
`Mul_R0_C: MultiplyR0Constant(); \
`Mul_R1_C: MultiplyR1Constant(); \
`Mul_R2_C: MultiplyR2Constant(); \
`Mul_R3_C: MultiplyR3Constant(); \
`Mul_R4_C: MultiplyR4Constant(); \
`Mul_R5_C: MultiplyR5Constant(); \
`Mul_R6_C: MultiplyR6Constant(); \
`Mul_R7_C: MultiplyR7Constant();

`define DivideCases  \
`Div_R0_C: DivideR0Constant(); \
`Div_R1_C: DivideR1Constant(); \
`Div_R2_C: DivideR2Constant(); \
`Div_R3_C: DivideR3Constant(); \
`Div_R4_C: DivideR4Constant(); \
`Div_R5_C: DivideR5Constant(); \
`Div_R6_C: DivideR6Constant(); \
`Div_R7_C: DivideR7Constant();

`define AndCases  \
`And_R0_C: AndR0Constant(); \
`And_R1_C: AndR1Constant(); \
`And_R2_C: AndR2Constant(); \
`And_R3_C: AndR3Constant(); \
`And_R4_C: AndR4Constant(); \
`And_R5_C: AndR5Constant(); \
`And_R6_C: AndR6Constant(); \
`And_R7_C: AndR7Constant();

`define OrCases  \
`Or_R0_C: OrR0Constant(); \
`Or_R1_C: OrR1Constant(); \
`Or_R2_C: OrR2Constant(); \
`Or_R3_C: OrR3Constant(); \
`Or_R4_C: OrR4Constant(); \
`Or_R5_C: OrR5Constant(); \
`Or_R6_C: OrR6Constant(); \
`Or_R7_C: OrR7Constant();

`define XorCases  \
`Xor_R0_C: XorR0Constant(); \
`Xor_R1_C: XorR1Constant(); \
`Xor_R2_C: XorR2Constant(); \
`Xor_R3_C: XorR3Constant(); \
`Xor_R4_C: XorR4Constant(); \
`Xor_R5_C: XorR5Constant(); \
`Xor_R6_C: XorR6Constant(); \
`Xor_R7_C: XorR7Constant();

`define LeftShiftCases  \
`LeftShift_R0_C: LeftShiftR0Constant(); \
`LeftShift_R1_C: LeftShiftR1Constant(); \
`LeftShift_R2_C: LeftShiftR2Constant(); \
`LeftShift_R3_C: LeftShiftR3Constant(); \
`LeftShift_R4_C: LeftShiftR4Constant(); \
`LeftShift_R5_C: LeftShiftR5Constant(); \
`LeftShift_R6_C: LeftShiftR6Constant(); \
`LeftShift_R7_C: LeftShiftR7Constant();

`define RightShiftCases  \
`RightShift_R0_C: RightShiftR0Constant(); \
`RightShift_R1_C: RightShiftR1Constant(); \
`RightShift_R2_C: RightShiftR2Constant(); \
`RightShift_R3_C: RightShiftR3Constant(); \
`RightShift_R4_C: RightShiftR4Constant(); \
`RightShift_R5_C: RightShiftR5Constant(); \
`RightShift_R6_C: RightShiftR6Constant(); \
`RightShift_R7_C: RightShiftR7Constant();

`define AddCases  \
`Add_R0_C: AddR0Constant(); \
`Add_R1_C: AddR1Constant(); \
`Add_R2_C: AddR2Constant(); \
`Add_R3_C: AddR3Constant(); \
`Add_R4_C: AddR4Constant(); \
`Add_R5_C: AddR5Constant(); \
`Add_R6_C: AddR6Constant(); \
`Add_R7_C: AddR7Constant();

`define SubtractCases  \
`Sub_R0_C: SubtractR0Constant(); \
`Sub_R1_C: SubtractR1Constant(); \
`Sub_R2_C: SubtractR2Constant(); \
`Sub_R3_C: SubtractR3Constant(); \
`Sub_R4_C: SubtractR4Constant(); \
`Sub_R5_C: SubtractR5Constant(); \
`Sub_R6_C: SubtractR6Constant(); \
`Sub_R7_C: SubtractR7Constant();

`define MultiplyCases  \
`Mul_R0_C: MultiplyR0Constant(); \
`Mul_R1_C: MultiplyR1Constant(); \
`Mul_R2_C: MultiplyR2Constant(); \
`Mul_R3_C: MultiplyR3Constant(); \
`Mul_R4_C: MultiplyR4Constant(); \
`Mul_R5_C: MultiplyR5Constant(); \
`Mul_R6_C: MultiplyR6Constant(); \
`Mul_R7_C: MultiplyR7Constant();

`define DivideCases  \
`Div_R0_C: DivideR0Constant(); \
`Div_R1_C: DivideR1Constant(); \
`Div_R2_C: DivideR2Constant(); \
`Div_R3_C: DivideR3Constant(); \
`Div_R4_C: DivideR4Constant(); \
`Div_R5_C: DivideR5Constant(); \
`Div_R6_C: DivideR6Constant(); \
`Div_R7_C: DivideR7Constant();

`define AndCases  \
`And_R0_C: AndR0Constant(); \
`And_R1_C: AndR1Constant(); \
`And_R2_C: AndR2Constant(); \
`And_R3_C: AndR3Constant(); \
`And_R4_C: AndR4Constant(); \
`And_R5_C: AndR5Constant(); \
`And_R6_C: AndR6Constant(); \
`And_R7_C: AndR7Constant();

`define OrCases  \
`Or_R0_C: OrR0Constant(); \
`Or_R1_C: OrR1Constant(); \
`Or_R2_C: OrR2Constant(); \
`Or_R3_C: OrR3Constant(); \
`Or_R4_C: OrR4Constant(); \
`Or_R5_C: OrR5Constant(); \
`Or_R6_C: OrR6Constant(); \
`Or_R7_C: OrR7Constant();

`define XorCases  \
`Xor_R0_C: XorR0Constant(); \
`Xor_R1_C: XorR1Constant(); \
`Xor_R2_C: XorR2Constant(); \
`Xor_R3_C: XorR3Constant(); \
`Xor_R4_C: XorR4Constant(); \
`Xor_R5_C: XorR5Constant(); \
`Xor_R6_C: XorR6Constant(); \
`Xor_R7_C: XorR7Constant();

`define LeftShiftCases  \
`LeftShift_R0_C: LeftShiftR0Constant(); \
`LeftShift_R1_C: LeftShiftR1Constant(); \
`LeftShift_R2_C: LeftShiftR2Constant(); \
`LeftShift_R3_C: LeftShiftR3Constant(); \
`LeftShift_R4_C: LeftShiftR4Constant(); \
`LeftShift_R5_C: LeftShiftR5Constant(); \
`LeftShift_R6_C: LeftShiftR6Constant(); \
`LeftShift_R7_C: LeftShiftR7Constant();

`define RightShiftCases  \
`RightShift_R0_C: RightShiftR0Constant(); \
`RightShift_R1_C: RightShiftR1Constant(); \
`RightShift_R2_C: RightShiftR2Constant(); \
`RightShift_R3_C: RightShiftR3Constant(); \
`RightShift_R4_C: RightShiftR4Constant(); \
`RightShift_R5_C: RightShiftR5Constant(); \
`RightShift_R6_C: RightShiftR6Constant(); \
`RightShift_R7_C: RightShiftR7Constant();

`define Error_None 32'h00000000
`define Error_General 32'h10000000
`define Error_Opcode 32'h10000001
`define Error_SubOpcode 32'h10000002
`define Error_SubOpcode2 32'h10000003
`define Error_Operand 32'h10000004
`define Error_Read 32'h20000000
`define Error_Write 32'h20000001
`define Error_Execute 32'h20000002
`define Error_InvalidState 32'h30000000
`define Error_InvalidCommand 32'h30000001
`define Error_InvalidCommandState 32'h30000002
`define Error_ArithmeticOperand0 32'h40000000
`define Error_ArithmeticOperand1 32'h40000001

`define Command_Nop 32'h00000000
`define Command_Break 32'h52300000
`define Command_Continue 32'h52300001
`define Command_Restart 32'h52300002
`define Command_Reset 32'h52300005
`define Command_ReadAddress 32'h52300010
`define Command_Read4 32'h52300020
`define Command_WriteAddress 32'h52300030
`define Command_Write4 32'h52300040
`define Command_GetProgramCounter 32'h52300050
`define Command_GetInstructionAddress 32'h52300051
`define Command_GetError 32'h52300052
`define Command_GetErrorCode 32'h52300053
`define Command_GetOpcode 32'h52300054
`define Command_GetOperands 32'h52300055
`define Command_GetRegisters 32'h52300056
`define Command_GetPageFlags 32'h52300057
`define Command_SetPageFlags 32'h52300100

`define CommandState_Waiting 8'h00
`define CommandState_ReadAddress 8'h10
`define CommandState_WriteAddress 8'h30
`define CommandState_Write4 8'h40
`define CommandState_WaitingGetPageNumber 8'h50
`define CommandState_WaitingSetPageNumber 8'h51
`define CommandState_WaitingSetPageFlags 8'h52

`define RamSize 32'h0000E000
`define PageSize 32'h00000100
`define Pages 32'h000000E0
`define RegisterDeclarations reg [Architecture-1:0] _r0 = 0, _r1 = 0, _r2 = 0, _r3 = 0, _r4 = 0, _r5 = 0, _r6 = 0, _r7 = 0, _programCounter = 0;

`define ReadRegisterCases  \
0: read4Address <= _r0; \
8: read4Address <= _r0 + `Operand8_3; \
16: read4Address <= _r0 - `Operand8_3; \
1: read4Address <= _r1; \
9: read4Address <= _r1 + `Operand8_3; \
17: read4Address <= _r1 - `Operand8_3; \
2: read4Address <= _r2; \
10: read4Address <= _r2 + `Operand8_3; \
18: read4Address <= _r2 - `Operand8_3; \
3: read4Address <= _r3; \
11: read4Address <= _r3 + `Operand8_3; \
19: read4Address <= _r3 - `Operand8_3; \
4: read4Address <= _r4; \
12: read4Address <= _r4 + `Operand8_3; \
20: read4Address <= _r4 - `Operand8_3; \
5: read4Address <= _r5; \
13: read4Address <= _r5 + `Operand8_3; \
21: read4Address <= _r5 - `Operand8_3; \
6: read4Address <= _r6; \
14: read4Address <= _r6 + `Operand8_3; \
22: read4Address <= _r6 - `Operand8_3; \
7: read4Address <= _r7; \
15: read4Address <= _r7 + `Operand8_3; \
23: read4Address <= _r7 - `Operand8_3;

`define ResetRegisters _r0 <= 0; _r1 <= 0; _r2 <= 0; _r3 <= 0; _r4 <= 0; _r5 <= 0; _r6 <= 0; _r7 <= 0;

`define AssignRRCases  \
0: begin case (`Operand8_2) 0: begin _r0 <= _r0; end 1: begin _r0 <= _r1; end 2: begin _r0 <= _r2; end 3: begin _r0 <= _r3; end 4: begin _r0 <= _r4; end 5: begin _r0 <= _r5; end 6: begin _r0 <= _r6; end 7: begin _r0 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end \
1: begin case (`Operand8_2) 0: begin _r1 <= _r0; end 1: begin _r1 <= _r1; end 2: begin _r1 <= _r2; end 3: begin _r1 <= _r3; end 4: begin _r1 <= _r4; end 5: begin _r1 <= _r5; end 6: begin _r1 <= _r6; end 7: begin _r1 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end \
2: begin case (`Operand8_2) 0: begin _r2 <= _r0; end 1: begin _r2 <= _r1; end 2: begin _r2 <= _r2; end 3: begin _r2 <= _r3; end 4: begin _r2 <= _r4; end 5: begin _r2 <= _r5; end 6: begin _r2 <= _r6; end 7: begin _r2 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end \
3: begin case (`Operand8_2) 0: begin _r3 <= _r0; end 1: begin _r3 <= _r1; end 2: begin _r3 <= _r2; end 3: begin _r3 <= _r3; end 4: begin _r3 <= _r4; end 5: begin _r3 <= _r5; end 6: begin _r3 <= _r6; end 7: begin _r3 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end \
4: begin case (`Operand8_2) 0: begin _r4 <= _r0; end 1: begin _r4 <= _r1; end 2: begin _r4 <= _r2; end 3: begin _r4 <= _r3; end 4: begin _r4 <= _r4; end 5: begin _r4 <= _r5; end 6: begin _r4 <= _r6; end 7: begin _r4 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end \
5: begin case (`Operand8_2) 0: begin _r5 <= _r0; end 1: begin _r5 <= _r1; end 2: begin _r5 <= _r2; end 3: begin _r5 <= _r3; end 4: begin _r5 <= _r4; end 5: begin _r5 <= _r5; end 6: begin _r5 <= _r6; end 7: begin _r5 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end \
6: begin case (`Operand8_2) 0: begin _r6 <= _r0; end 1: begin _r6 <= _r1; end 2: begin _r6 <= _r2; end 3: begin _r6 <= _r3; end 4: begin _r6 <= _r4; end 5: begin _r6 <= _r5; end 6: begin _r6 <= _r6; end 7: begin _r6 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end \
7: begin case (`Operand8_2) 0: begin _r7 <= _r0; end 1: begin _r7 <= _r1; end 2: begin _r7 <= _r2; end 3: begin _r7 <= _r3; end 4: begin _r7 <= _r4; end 5: begin _r7 <= _r5; end 6: begin _r7 <= _r6; end 7: begin _r7 <= _r7; end default: begin `SetError(`Error_Operand) end endcase end

`define CompareRRCases  \
`Eq_R_R: begin case (`Operand8_1) 0: begin case (`Operand8_2) 0: begin _isEq <= _r0 == _r0; end 1: begin _isEq <= _r0 == _r1; end 2: begin _isEq <= _r0 == _r2; end 3: begin _isEq <= _r0 == _r3; end 4: begin _isEq <= _r0 == _r4; end 5: begin _isEq <= _r0 == _r5; end 6: begin _isEq <= _r0 == _r6; end 7: begin _isEq <= _r0 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  1: begin case (`Operand8_2) 0: begin _isEq <= _r1 == _r0; end 1: begin _isEq <= _r1 == _r1; end 2: begin _isEq <= _r1 == _r2; end 3: begin _isEq <= _r1 == _r3; end 4: begin _isEq <= _r1 == _r4; end 5: begin _isEq <= _r1 == _r5; end 6: begin _isEq <= _r1 == _r6; end 7: begin _isEq <= _r1 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  2: begin case (`Operand8_2) 0: begin _isEq <= _r2 == _r0; end 1: begin _isEq <= _r2 == _r1; end 2: begin _isEq <= _r2 == _r2; end 3: begin _isEq <= _r2 == _r3; end 4: begin _isEq <= _r2 == _r4; end 5: begin _isEq <= _r2 == _r5; end 6: begin _isEq <= _r2 == _r6; end 7: begin _isEq <= _r2 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  3: begin case (`Operand8_2) 0: begin _isEq <= _r3 == _r0; end 1: begin _isEq <= _r3 == _r1; end 2: begin _isEq <= _r3 == _r2; end 3: begin _isEq <= _r3 == _r3; end 4: begin _isEq <= _r3 == _r4; end 5: begin _isEq <= _r3 == _r5; end 6: begin _isEq <= _r3 == _r6; end 7: begin _isEq <= _r3 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  4: begin case (`Operand8_2) 0: begin _isEq <= _r4 == _r0; end 1: begin _isEq <= _r4 == _r1; end 2: begin _isEq <= _r4 == _r2; end 3: begin _isEq <= _r4 == _r3; end 4: begin _isEq <= _r4 == _r4; end 5: begin _isEq <= _r4 == _r5; end 6: begin _isEq <= _r4 == _r6; end 7: begin _isEq <= _r4 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  5: begin case (`Operand8_2) 0: begin _isEq <= _r5 == _r0; end 1: begin _isEq <= _r5 == _r1; end 2: begin _isEq <= _r5 == _r2; end 3: begin _isEq <= _r5 == _r3; end 4: begin _isEq <= _r5 == _r4; end 5: begin _isEq <= _r5 == _r5; end 6: begin _isEq <= _r5 == _r6; end 7: begin _isEq <= _r5 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  6: begin case (`Operand8_2) 0: begin _isEq <= _r6 == _r0; end 1: begin _isEq <= _r6 == _r1; end 2: begin _isEq <= _r6 == _r2; end 3: begin _isEq <= _r6 == _r3; end 4: begin _isEq <= _r6 == _r4; end 5: begin _isEq <= _r6 == _r5; end 6: begin _isEq <= _r6 == _r6; end 7: begin _isEq <= _r6 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  7: begin case (`Operand8_2) 0: begin _isEq <= _r7 == _r0; end 1: begin _isEq <= _r7 == _r1; end 2: begin _isEq <= _r7 == _r2; end 3: begin _isEq <= _r7 == _r3; end 4: begin _isEq <= _r7 == _r4; end 5: begin _isEq <= _r7 == _r5; end 6: begin _isEq <= _r7 == _r6; end 7: begin _isEq <= _r7 == _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  default: begin `SetError(`Error_Opcode) end  endcase AdvancePC(); end  \
`NotEq_R_R: begin case (`Operand8_1) 0: begin case (`Operand8_2) 0: begin _isEq <= _r0 != _r0; end 1: begin _isEq <= _r0 != _r1; end 2: begin _isEq <= _r0 != _r2; end 3: begin _isEq <= _r0 != _r3; end 4: begin _isEq <= _r0 != _r4; end 5: begin _isEq <= _r0 != _r5; end 6: begin _isEq <= _r0 != _r6; end 7: begin _isEq <= _r0 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  1: begin case (`Operand8_2) 0: begin _isEq <= _r1 != _r0; end 1: begin _isEq <= _r1 != _r1; end 2: begin _isEq <= _r1 != _r2; end 3: begin _isEq <= _r1 != _r3; end 4: begin _isEq <= _r1 != _r4; end 5: begin _isEq <= _r1 != _r5; end 6: begin _isEq <= _r1 != _r6; end 7: begin _isEq <= _r1 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  2: begin case (`Operand8_2) 0: begin _isEq <= _r2 != _r0; end 1: begin _isEq <= _r2 != _r1; end 2: begin _isEq <= _r2 != _r2; end 3: begin _isEq <= _r2 != _r3; end 4: begin _isEq <= _r2 != _r4; end 5: begin _isEq <= _r2 != _r5; end 6: begin _isEq <= _r2 != _r6; end 7: begin _isEq <= _r2 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  3: begin case (`Operand8_2) 0: begin _isEq <= _r3 != _r0; end 1: begin _isEq <= _r3 != _r1; end 2: begin _isEq <= _r3 != _r2; end 3: begin _isEq <= _r3 != _r3; end 4: begin _isEq <= _r3 != _r4; end 5: begin _isEq <= _r3 != _r5; end 6: begin _isEq <= _r3 != _r6; end 7: begin _isEq <= _r3 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  4: begin case (`Operand8_2) 0: begin _isEq <= _r4 != _r0; end 1: begin _isEq <= _r4 != _r1; end 2: begin _isEq <= _r4 != _r2; end 3: begin _isEq <= _r4 != _r3; end 4: begin _isEq <= _r4 != _r4; end 5: begin _isEq <= _r4 != _r5; end 6: begin _isEq <= _r4 != _r6; end 7: begin _isEq <= _r4 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  5: begin case (`Operand8_2) 0: begin _isEq <= _r5 != _r0; end 1: begin _isEq <= _r5 != _r1; end 2: begin _isEq <= _r5 != _r2; end 3: begin _isEq <= _r5 != _r3; end 4: begin _isEq <= _r5 != _r4; end 5: begin _isEq <= _r5 != _r5; end 6: begin _isEq <= _r5 != _r6; end 7: begin _isEq <= _r5 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  6: begin case (`Operand8_2) 0: begin _isEq <= _r6 != _r0; end 1: begin _isEq <= _r6 != _r1; end 2: begin _isEq <= _r6 != _r2; end 3: begin _isEq <= _r6 != _r3; end 4: begin _isEq <= _r6 != _r4; end 5: begin _isEq <= _r6 != _r5; end 6: begin _isEq <= _r6 != _r6; end 7: begin _isEq <= _r6 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  7: begin case (`Operand8_2) 0: begin _isEq <= _r7 != _r0; end 1: begin _isEq <= _r7 != _r1; end 2: begin _isEq <= _r7 != _r2; end 3: begin _isEq <= _r7 != _r3; end 4: begin _isEq <= _r7 != _r4; end 5: begin _isEq <= _r7 != _r5; end 6: begin _isEq <= _r7 != _r6; end 7: begin _isEq <= _r7 != _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  default: begin `SetError(`Error_Opcode) end  endcase AdvancePC(); end  \
`LessThan_R_R: begin case (`Operand8_1) 0: begin case (`Operand8_2) 0: begin _isEq <= _r0 < _r0; end 1: begin _isEq <= _r0 < _r1; end 2: begin _isEq <= _r0 < _r2; end 3: begin _isEq <= _r0 < _r3; end 4: begin _isEq <= _r0 < _r4; end 5: begin _isEq <= _r0 < _r5; end 6: begin _isEq <= _r0 < _r6; end 7: begin _isEq <= _r0 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  1: begin case (`Operand8_2) 0: begin _isEq <= _r1 < _r0; end 1: begin _isEq <= _r1 < _r1; end 2: begin _isEq <= _r1 < _r2; end 3: begin _isEq <= _r1 < _r3; end 4: begin _isEq <= _r1 < _r4; end 5: begin _isEq <= _r1 < _r5; end 6: begin _isEq <= _r1 < _r6; end 7: begin _isEq <= _r1 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  2: begin case (`Operand8_2) 0: begin _isEq <= _r2 < _r0; end 1: begin _isEq <= _r2 < _r1; end 2: begin _isEq <= _r2 < _r2; end 3: begin _isEq <= _r2 < _r3; end 4: begin _isEq <= _r2 < _r4; end 5: begin _isEq <= _r2 < _r5; end 6: begin _isEq <= _r2 < _r6; end 7: begin _isEq <= _r2 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  3: begin case (`Operand8_2) 0: begin _isEq <= _r3 < _r0; end 1: begin _isEq <= _r3 < _r1; end 2: begin _isEq <= _r3 < _r2; end 3: begin _isEq <= _r3 < _r3; end 4: begin _isEq <= _r3 < _r4; end 5: begin _isEq <= _r3 < _r5; end 6: begin _isEq <= _r3 < _r6; end 7: begin _isEq <= _r3 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  4: begin case (`Operand8_2) 0: begin _isEq <= _r4 < _r0; end 1: begin _isEq <= _r4 < _r1; end 2: begin _isEq <= _r4 < _r2; end 3: begin _isEq <= _r4 < _r3; end 4: begin _isEq <= _r4 < _r4; end 5: begin _isEq <= _r4 < _r5; end 6: begin _isEq <= _r4 < _r6; end 7: begin _isEq <= _r4 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  5: begin case (`Operand8_2) 0: begin _isEq <= _r5 < _r0; end 1: begin _isEq <= _r5 < _r1; end 2: begin _isEq <= _r5 < _r2; end 3: begin _isEq <= _r5 < _r3; end 4: begin _isEq <= _r5 < _r4; end 5: begin _isEq <= _r5 < _r5; end 6: begin _isEq <= _r5 < _r6; end 7: begin _isEq <= _r5 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  6: begin case (`Operand8_2) 0: begin _isEq <= _r6 < _r0; end 1: begin _isEq <= _r6 < _r1; end 2: begin _isEq <= _r6 < _r2; end 3: begin _isEq <= _r6 < _r3; end 4: begin _isEq <= _r6 < _r4; end 5: begin _isEq <= _r6 < _r5; end 6: begin _isEq <= _r6 < _r6; end 7: begin _isEq <= _r6 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  7: begin case (`Operand8_2) 0: begin _isEq <= _r7 < _r0; end 1: begin _isEq <= _r7 < _r1; end 2: begin _isEq <= _r7 < _r2; end 3: begin _isEq <= _r7 < _r3; end 4: begin _isEq <= _r7 < _r4; end 5: begin _isEq <= _r7 < _r5; end 6: begin _isEq <= _r7 < _r6; end 7: begin _isEq <= _r7 < _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  default: begin `SetError(`Error_Opcode) end  endcase AdvancePC(); end  \
`LessThanEq_R_R: begin case (`Operand8_1) 0: begin case (`Operand8_2) 0: begin _isEq <= _r0 <= _r0; end 1: begin _isEq <= _r0 <= _r1; end 2: begin _isEq <= _r0 <= _r2; end 3: begin _isEq <= _r0 <= _r3; end 4: begin _isEq <= _r0 <= _r4; end 5: begin _isEq <= _r0 <= _r5; end 6: begin _isEq <= _r0 <= _r6; end 7: begin _isEq <= _r0 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  1: begin case (`Operand8_2) 0: begin _isEq <= _r1 <= _r0; end 1: begin _isEq <= _r1 <= _r1; end 2: begin _isEq <= _r1 <= _r2; end 3: begin _isEq <= _r1 <= _r3; end 4: begin _isEq <= _r1 <= _r4; end 5: begin _isEq <= _r1 <= _r5; end 6: begin _isEq <= _r1 <= _r6; end 7: begin _isEq <= _r1 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  2: begin case (`Operand8_2) 0: begin _isEq <= _r2 <= _r0; end 1: begin _isEq <= _r2 <= _r1; end 2: begin _isEq <= _r2 <= _r2; end 3: begin _isEq <= _r2 <= _r3; end 4: begin _isEq <= _r2 <= _r4; end 5: begin _isEq <= _r2 <= _r5; end 6: begin _isEq <= _r2 <= _r6; end 7: begin _isEq <= _r2 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  3: begin case (`Operand8_2) 0: begin _isEq <= _r3 <= _r0; end 1: begin _isEq <= _r3 <= _r1; end 2: begin _isEq <= _r3 <= _r2; end 3: begin _isEq <= _r3 <= _r3; end 4: begin _isEq <= _r3 <= _r4; end 5: begin _isEq <= _r3 <= _r5; end 6: begin _isEq <= _r3 <= _r6; end 7: begin _isEq <= _r3 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  4: begin case (`Operand8_2) 0: begin _isEq <= _r4 <= _r0; end 1: begin _isEq <= _r4 <= _r1; end 2: begin _isEq <= _r4 <= _r2; end 3: begin _isEq <= _r4 <= _r3; end 4: begin _isEq <= _r4 <= _r4; end 5: begin _isEq <= _r4 <= _r5; end 6: begin _isEq <= _r4 <= _r6; end 7: begin _isEq <= _r4 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  5: begin case (`Operand8_2) 0: begin _isEq <= _r5 <= _r0; end 1: begin _isEq <= _r5 <= _r1; end 2: begin _isEq <= _r5 <= _r2; end 3: begin _isEq <= _r5 <= _r3; end 4: begin _isEq <= _r5 <= _r4; end 5: begin _isEq <= _r5 <= _r5; end 6: begin _isEq <= _r5 <= _r6; end 7: begin _isEq <= _r5 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  6: begin case (`Operand8_2) 0: begin _isEq <= _r6 <= _r0; end 1: begin _isEq <= _r6 <= _r1; end 2: begin _isEq <= _r6 <= _r2; end 3: begin _isEq <= _r6 <= _r3; end 4: begin _isEq <= _r6 <= _r4; end 5: begin _isEq <= _r6 <= _r5; end 6: begin _isEq <= _r6 <= _r6; end 7: begin _isEq <= _r6 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  7: begin case (`Operand8_2) 0: begin _isEq <= _r7 <= _r0; end 1: begin _isEq <= _r7 <= _r1; end 2: begin _isEq <= _r7 <= _r2; end 3: begin _isEq <= _r7 <= _r3; end 4: begin _isEq <= _r7 <= _r4; end 5: begin _isEq <= _r7 <= _r5; end 6: begin _isEq <= _r7 <= _r6; end 7: begin _isEq <= _r7 <= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  default: begin `SetError(`Error_Opcode) end  endcase AdvancePC(); end  \
`GreaterThan_R_R: begin case (`Operand8_1) 0: begin case (`Operand8_2) 0: begin _isEq <= _r0 > _r0; end 1: begin _isEq <= _r0 > _r1; end 2: begin _isEq <= _r0 > _r2; end 3: begin _isEq <= _r0 > _r3; end 4: begin _isEq <= _r0 > _r4; end 5: begin _isEq <= _r0 > _r5; end 6: begin _isEq <= _r0 > _r6; end 7: begin _isEq <= _r0 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  1: begin case (`Operand8_2) 0: begin _isEq <= _r1 > _r0; end 1: begin _isEq <= _r1 > _r1; end 2: begin _isEq <= _r1 > _r2; end 3: begin _isEq <= _r1 > _r3; end 4: begin _isEq <= _r1 > _r4; end 5: begin _isEq <= _r1 > _r5; end 6: begin _isEq <= _r1 > _r6; end 7: begin _isEq <= _r1 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  2: begin case (`Operand8_2) 0: begin _isEq <= _r2 > _r0; end 1: begin _isEq <= _r2 > _r1; end 2: begin _isEq <= _r2 > _r2; end 3: begin _isEq <= _r2 > _r3; end 4: begin _isEq <= _r2 > _r4; end 5: begin _isEq <= _r2 > _r5; end 6: begin _isEq <= _r2 > _r6; end 7: begin _isEq <= _r2 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  3: begin case (`Operand8_2) 0: begin _isEq <= _r3 > _r0; end 1: begin _isEq <= _r3 > _r1; end 2: begin _isEq <= _r3 > _r2; end 3: begin _isEq <= _r3 > _r3; end 4: begin _isEq <= _r3 > _r4; end 5: begin _isEq <= _r3 > _r5; end 6: begin _isEq <= _r3 > _r6; end 7: begin _isEq <= _r3 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  4: begin case (`Operand8_2) 0: begin _isEq <= _r4 > _r0; end 1: begin _isEq <= _r4 > _r1; end 2: begin _isEq <= _r4 > _r2; end 3: begin _isEq <= _r4 > _r3; end 4: begin _isEq <= _r4 > _r4; end 5: begin _isEq <= _r4 > _r5; end 6: begin _isEq <= _r4 > _r6; end 7: begin _isEq <= _r4 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  5: begin case (`Operand8_2) 0: begin _isEq <= _r5 > _r0; end 1: begin _isEq <= _r5 > _r1; end 2: begin _isEq <= _r5 > _r2; end 3: begin _isEq <= _r5 > _r3; end 4: begin _isEq <= _r5 > _r4; end 5: begin _isEq <= _r5 > _r5; end 6: begin _isEq <= _r5 > _r6; end 7: begin _isEq <= _r5 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  6: begin case (`Operand8_2) 0: begin _isEq <= _r6 > _r0; end 1: begin _isEq <= _r6 > _r1; end 2: begin _isEq <= _r6 > _r2; end 3: begin _isEq <= _r6 > _r3; end 4: begin _isEq <= _r6 > _r4; end 5: begin _isEq <= _r6 > _r5; end 6: begin _isEq <= _r6 > _r6; end 7: begin _isEq <= _r6 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  7: begin case (`Operand8_2) 0: begin _isEq <= _r7 > _r0; end 1: begin _isEq <= _r7 > _r1; end 2: begin _isEq <= _r7 > _r2; end 3: begin _isEq <= _r7 > _r3; end 4: begin _isEq <= _r7 > _r4; end 5: begin _isEq <= _r7 > _r5; end 6: begin _isEq <= _r7 > _r6; end 7: begin _isEq <= _r7 > _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  default: begin `SetError(`Error_Opcode) end  endcase AdvancePC(); end  \
`GreaterThanEq_R_R: begin case (`Operand8_1) 0: begin case (`Operand8_2) 0: begin _isEq <= _r0 >= _r0; end 1: begin _isEq <= _r0 >= _r1; end 2: begin _isEq <= _r0 >= _r2; end 3: begin _isEq <= _r0 >= _r3; end 4: begin _isEq <= _r0 >= _r4; end 5: begin _isEq <= _r0 >= _r5; end 6: begin _isEq <= _r0 >= _r6; end 7: begin _isEq <= _r0 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  1: begin case (`Operand8_2) 0: begin _isEq <= _r1 >= _r0; end 1: begin _isEq <= _r1 >= _r1; end 2: begin _isEq <= _r1 >= _r2; end 3: begin _isEq <= _r1 >= _r3; end 4: begin _isEq <= _r1 >= _r4; end 5: begin _isEq <= _r1 >= _r5; end 6: begin _isEq <= _r1 >= _r6; end 7: begin _isEq <= _r1 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  2: begin case (`Operand8_2) 0: begin _isEq <= _r2 >= _r0; end 1: begin _isEq <= _r2 >= _r1; end 2: begin _isEq <= _r2 >= _r2; end 3: begin _isEq <= _r2 >= _r3; end 4: begin _isEq <= _r2 >= _r4; end 5: begin _isEq <= _r2 >= _r5; end 6: begin _isEq <= _r2 >= _r6; end 7: begin _isEq <= _r2 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  3: begin case (`Operand8_2) 0: begin _isEq <= _r3 >= _r0; end 1: begin _isEq <= _r3 >= _r1; end 2: begin _isEq <= _r3 >= _r2; end 3: begin _isEq <= _r3 >= _r3; end 4: begin _isEq <= _r3 >= _r4; end 5: begin _isEq <= _r3 >= _r5; end 6: begin _isEq <= _r3 >= _r6; end 7: begin _isEq <= _r3 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  4: begin case (`Operand8_2) 0: begin _isEq <= _r4 >= _r0; end 1: begin _isEq <= _r4 >= _r1; end 2: begin _isEq <= _r4 >= _r2; end 3: begin _isEq <= _r4 >= _r3; end 4: begin _isEq <= _r4 >= _r4; end 5: begin _isEq <= _r4 >= _r5; end 6: begin _isEq <= _r4 >= _r6; end 7: begin _isEq <= _r4 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  5: begin case (`Operand8_2) 0: begin _isEq <= _r5 >= _r0; end 1: begin _isEq <= _r5 >= _r1; end 2: begin _isEq <= _r5 >= _r2; end 3: begin _isEq <= _r5 >= _r3; end 4: begin _isEq <= _r5 >= _r4; end 5: begin _isEq <= _r5 >= _r5; end 6: begin _isEq <= _r5 >= _r6; end 7: begin _isEq <= _r5 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  6: begin case (`Operand8_2) 0: begin _isEq <= _r6 >= _r0; end 1: begin _isEq <= _r6 >= _r1; end 2: begin _isEq <= _r6 >= _r2; end 3: begin _isEq <= _r6 >= _r3; end 4: begin _isEq <= _r6 >= _r4; end 5: begin _isEq <= _r6 >= _r5; end 6: begin _isEq <= _r6 >= _r6; end 7: begin _isEq <= _r6 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  7: begin case (`Operand8_2) 0: begin _isEq <= _r7 >= _r0; end 1: begin _isEq <= _r7 >= _r1; end 2: begin _isEq <= _r7 >= _r2; end 3: begin _isEq <= _r7 >= _r3; end 4: begin _isEq <= _r7 >= _r4; end 5: begin _isEq <= _r7 >= _r5; end 6: begin _isEq <= _r7 >= _r6; end 7: begin _isEq <= _r7 >= _r7; end  default: begin `SetError(`Error_Opcode) end endcase end  default: begin `SetError(`Error_Opcode) end  endcase AdvancePC(); end 

