`define ArithmeticModules  \
 \
  reg [Architecture-1:0] adderA = 0, adderB = 0; \
  wire [Architecture-1:0] adderC; \
  Adder adder(.clk(clk), .a(adderA), .b(adderB), .c(adderC)); \
  reg [7:0] adderStage = 0; \
 \
  task AddRegisters; \
    begin \
       \
      case (adderStage) \
        0: \
          begin \
            adderStage <= 1; \
            case (`Operand8_1) \
              0: adderA <= _r0; \
              1: adderA <= _r1; \
              2: adderA <= _r2; \
              3: adderA <= _r3; \
              4: adderA <= _r4; \
              5: adderA <= _r5; \
              6: adderA <= _r6; \
              7: adderA <= _r7; \
              default: begin adderA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: adderB <= _r0; \
              1: adderB <= _r1; \
              2: adderB <= _r2; \
              3: adderB <= _r3; \
              4: adderB <= _r4; \
              5: adderB <= _r5; \
              6: adderB <= _r6; \
              7: adderB <= _r7; \
              default: begin adderB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            adderStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= adderC; \
              1: _r1 <= adderC; \
              2: _r2 <= adderC; \
              3: _r3 <= adderC; \
              4: _r4 <= adderC; \
              5: _r5 <= adderC; \
              6: _r6 <= adderC; \
              7: _r7 <= adderC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] subtracterA = 0, subtracterB = 0; \
  wire [Architecture-1:0] subtracterC; \
  Subtracter subtracter(.clk(clk), .a(subtracterA), .b(subtracterB), .c(subtracterC)); \
  reg [7:0] subtracterStage = 0; \
 \
  task SubtractRegisters; \
    begin \
       \
      case (subtracterStage) \
        0: \
          begin \
            subtracterStage <= 1; \
            case (`Operand8_1) \
              0: subtracterA <= _r0; \
              1: subtracterA <= _r1; \
              2: subtracterA <= _r2; \
              3: subtracterA <= _r3; \
              4: subtracterA <= _r4; \
              5: subtracterA <= _r5; \
              6: subtracterA <= _r6; \
              7: subtracterA <= _r7; \
              default: begin subtracterA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: subtracterB <= _r0; \
              1: subtracterB <= _r1; \
              2: subtracterB <= _r2; \
              3: subtracterB <= _r3; \
              4: subtracterB <= _r4; \
              5: subtracterB <= _r5; \
              6: subtracterB <= _r6; \
              7: subtracterB <= _r7; \
              default: begin subtracterB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            subtracterStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= subtracterC; \
              1: _r1 <= subtracterC; \
              2: _r2 <= subtracterC; \
              3: _r3 <= subtracterC; \
              4: _r4 <= subtracterC; \
              5: _r5 <= subtracterC; \
              6: _r6 <= subtracterC; \
              7: _r7 <= subtracterC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] multiplierA = 0, multiplierB = 0; \
  wire [Architecture-1:0] multiplierC; \
  Multiplier multiplier(.clk(clk), .a(multiplierA), .b(multiplierB), .c(multiplierC)); \
  reg [7:0] multiplierStage = 0; \
 \
  task MultiplyRegisters; \
    begin \
       \
      case (multiplierStage) \
        0: \
          begin \
            multiplierStage <= 1; \
            case (`Operand8_1) \
              0: multiplierA <= _r0; \
              1: multiplierA <= _r1; \
              2: multiplierA <= _r2; \
              3: multiplierA <= _r3; \
              4: multiplierA <= _r4; \
              5: multiplierA <= _r5; \
              6: multiplierA <= _r6; \
              7: multiplierA <= _r7; \
              default: begin multiplierA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: multiplierB <= _r0; \
              1: multiplierB <= _r1; \
              2: multiplierB <= _r2; \
              3: multiplierB <= _r3; \
              4: multiplierB <= _r4; \
              5: multiplierB <= _r5; \
              6: multiplierB <= _r6; \
              7: multiplierB <= _r7; \
              default: begin multiplierB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            multiplierStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= multiplierC; \
              1: _r1 <= multiplierC; \
              2: _r2 <= multiplierC; \
              3: _r3 <= multiplierC; \
              4: _r4 <= multiplierC; \
              5: _r5 <= multiplierC; \
              6: _r6 <= multiplierC; \
              7: _r7 <= multiplierC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] dividerA = 0, dividerB = 0; \
  wire [Architecture-1:0] dividerC; \
  Divider divider(.clk(clk), .a(dividerA), .b(dividerB), .c(dividerC)); \
  reg [7:0] dividerStage = 0; \
 \
  task DivideRegisters; \
    begin \
       \
      case (dividerStage) \
        0: \
          begin \
            dividerStage <= 1; \
            case (`Operand8_1) \
              0: dividerA <= _r0; \
              1: dividerA <= _r1; \
              2: dividerA <= _r2; \
              3: dividerA <= _r3; \
              4: dividerA <= _r4; \
              5: dividerA <= _r5; \
              6: dividerA <= _r6; \
              7: dividerA <= _r7; \
              default: begin dividerA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: dividerB <= _r0; \
              1: dividerB <= _r1; \
              2: dividerB <= _r2; \
              3: dividerB <= _r3; \
              4: dividerB <= _r4; \
              5: dividerB <= _r5; \
              6: dividerB <= _r6; \
              7: dividerB <= _r7; \
              default: begin dividerB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
             \
          end \
        1: dividerStage <= 2; \
        2: dividerStage <= 3; \
        3: dividerStage <= 4; \
        4: dividerStage <= 5; \
        5: dividerStage <= 6; \
        6: dividerStage <= 7; \
        7: dividerStage <= 8; \
        8: dividerStage <= 9; \
        9: dividerStage <= 10; \
        10: dividerStage <= 11; \
        11: dividerStage <= 12; \
        12: dividerStage <= 13; \
        13: dividerStage <= 14; \
        14: dividerStage <= 15; \
        15: dividerStage <= 16; \
        16: dividerStage <= 17; \
        17: dividerStage <= 18; \
        18: dividerStage <= 19; \
        19: dividerStage <= 20; \
        20: dividerStage <= 21; \
        21: dividerStage <= 22; \
        22: dividerStage <= 23; \
        23: dividerStage <= 24; \
        24: dividerStage <= 25; \
        25: dividerStage <= 26; \
        26: dividerStage <= 27; \
        27: dividerStage <= 28; \
        28: dividerStage <= 29; \
        29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
        31: \
          begin       \
            dividerStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= dividerC; \
              1: _r1 <= dividerC; \
              2: _r2 <= dividerC; \
              3: _r3 <= dividerC; \
              4: _r4 <= dividerC; \
              5: _r5 <= dividerC; \
              6: _r6 <= dividerC; \
              7: _r7 <= dividerC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] anderA = 0, anderB = 0; \
  wire [Architecture-1:0] anderC; \
  Ander ander(.clk(clk), .a(anderA), .b(anderB), .c(anderC)); \
  reg [7:0] anderStage = 0; \
 \
  task AndRegisters; \
    begin \
       \
      case (anderStage) \
        0: \
          begin \
            anderStage <= 1; \
            case (`Operand8_1) \
              0: anderA <= _r0; \
              1: anderA <= _r1; \
              2: anderA <= _r2; \
              3: anderA <= _r3; \
              4: anderA <= _r4; \
              5: anderA <= _r5; \
              6: anderA <= _r6; \
              7: anderA <= _r7; \
              default: begin anderA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: anderB <= _r0; \
              1: anderB <= _r1; \
              2: anderB <= _r2; \
              3: anderB <= _r3; \
              4: anderB <= _r4; \
              5: anderB <= _r5; \
              6: anderB <= _r6; \
              7: anderB <= _r7; \
              default: begin anderB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            anderStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= anderC; \
              1: _r1 <= anderC; \
              2: _r2 <= anderC; \
              3: _r3 <= anderC; \
              4: _r4 <= anderC; \
              5: _r5 <= anderC; \
              6: _r6 <= anderC; \
              7: _r7 <= anderC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] orerA = 0, orerB = 0; \
  wire [Architecture-1:0] orerC; \
  Orer orer(.clk(clk), .a(orerA), .b(orerB), .c(orerC)); \
  reg [7:0] orerStage = 0; \
 \
  task OrRegisters; \
    begin \
       \
      case (orerStage) \
        0: \
          begin \
            orerStage <= 1; \
            case (`Operand8_1) \
              0: orerA <= _r0; \
              1: orerA <= _r1; \
              2: orerA <= _r2; \
              3: orerA <= _r3; \
              4: orerA <= _r4; \
              5: orerA <= _r5; \
              6: orerA <= _r6; \
              7: orerA <= _r7; \
              default: begin orerA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: orerB <= _r0; \
              1: orerB <= _r1; \
              2: orerB <= _r2; \
              3: orerB <= _r3; \
              4: orerB <= _r4; \
              5: orerB <= _r5; \
              6: orerB <= _r6; \
              7: orerB <= _r7; \
              default: begin orerB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            orerStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= orerC; \
              1: _r1 <= orerC; \
              2: _r2 <= orerC; \
              3: _r3 <= orerC; \
              4: _r4 <= orerC; \
              5: _r5 <= orerC; \
              6: _r6 <= orerC; \
              7: _r7 <= orerC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] xorerA = 0, xorerB = 0; \
  wire [Architecture-1:0] xorerC; \
  Xorer xorer(.clk(clk), .a(xorerA), .b(xorerB), .c(xorerC)); \
  reg [7:0] xorerStage = 0; \
 \
  task XorRegisters; \
    begin \
       \
      case (xorerStage) \
        0: \
          begin \
            xorerStage <= 1; \
            case (`Operand8_1) \
              0: xorerA <= _r0; \
              1: xorerA <= _r1; \
              2: xorerA <= _r2; \
              3: xorerA <= _r3; \
              4: xorerA <= _r4; \
              5: xorerA <= _r5; \
              6: xorerA <= _r6; \
              7: xorerA <= _r7; \
              default: begin xorerA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: xorerB <= _r0; \
              1: xorerB <= _r1; \
              2: xorerB <= _r2; \
              3: xorerB <= _r3; \
              4: xorerB <= _r4; \
              5: xorerB <= _r5; \
              6: xorerB <= _r6; \
              7: xorerB <= _r7; \
              default: begin xorerB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            xorerStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= xorerC; \
              1: _r1 <= xorerC; \
              2: _r2 <= xorerC; \
              3: _r3 <= xorerC; \
              4: _r4 <= xorerC; \
              5: _r5 <= xorerC; \
              6: _r6 <= xorerC; \
              7: _r7 <= xorerC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] leftShifterA = 0, leftShifterB = 0; \
  wire [Architecture-1:0] leftShifterC; \
  LeftShifter leftShifter(.clk(clk), .a(leftShifterA), .b(leftShifterB), .c(leftShifterC)); \
  reg [7:0] leftShifterStage = 0; \
 \
  task LeftShiftRegisters; \
    begin \
       \
      case (leftShifterStage) \
        0: \
          begin \
            leftShifterStage <= 1; \
            case (`Operand8_1) \
              0: leftShifterA <= _r0; \
              1: leftShifterA <= _r1; \
              2: leftShifterA <= _r2; \
              3: leftShifterA <= _r3; \
              4: leftShifterA <= _r4; \
              5: leftShifterA <= _r5; \
              6: leftShifterA <= _r6; \
              7: leftShifterA <= _r7; \
              default: begin leftShifterA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: leftShifterB <= _r0; \
              1: leftShifterB <= _r1; \
              2: leftShifterB <= _r2; \
              3: leftShifterB <= _r3; \
              4: leftShifterB <= _r4; \
              5: leftShifterB <= _r5; \
              6: leftShifterB <= _r6; \
              7: leftShifterB <= _r7; \
              default: begin leftShifterB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            leftShifterStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= leftShifterC; \
              1: _r1 <= leftShifterC; \
              2: _r2 <= leftShifterC; \
              3: _r3 <= leftShifterC; \
              4: _r4 <= leftShifterC; \
              5: _r5 <= leftShifterC; \
              6: _r6 <= leftShifterC; \
              7: _r7 <= leftShifterC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
  reg [Architecture-1:0] rightShifterA = 0, rightShifterB = 0; \
  wire [Architecture-1:0] rightShifterC; \
  RightShifter rightShifter(.clk(clk), .a(rightShifterA), .b(rightShifterB), .c(rightShifterC)); \
  reg [7:0] rightShifterStage = 0; \
 \
  task RightShiftRegisters; \
    begin \
       \
      case (rightShifterStage) \
        0: \
          begin \
            rightShifterStage <= 1; \
            case (`Operand8_1) \
              0: rightShifterA <= _r0; \
              1: rightShifterA <= _r1; \
              2: rightShifterA <= _r2; \
              3: rightShifterA <= _r3; \
              4: rightShifterA <= _r4; \
              5: rightShifterA <= _r5; \
              6: rightShifterA <= _r6; \
              7: rightShifterA <= _r7; \
              default: begin rightShifterA <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase \
       \
            case (`Operand8_2) \
              0: rightShifterB <= _r0; \
              1: rightShifterB <= _r1; \
              2: rightShifterB <= _r2; \
              3: rightShifterB <= _r3; \
              4: rightShifterB <= _r4; \
              5: rightShifterB <= _r5; \
              6: rightShifterB <= _r6; \
              7: rightShifterB <= _r7; \
              default: begin rightShifterB <= 0; _error <= 1; _errorCode <= `Error_ArithmeticOperand1; end \
            endcase \
 \
            readInstructionEnable <= 1; \
          end \
 \
        1: \
          begin       \
            rightShifterStage <= 0;       \
            case (`Operand8_1) \
              0: _r0 <= rightShifterC; \
              1: _r1 <= rightShifterC; \
              2: _r2 <= rightShifterC; \
              3: _r3 <= rightShifterC; \
              4: _r4 <= rightShifterC; \
              5: _r5 <= rightShifterC; \
              6: _r6 <= rightShifterC; \
              7: _r7 <= rightShifterC; \
              default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
            endcase    \
             \
            AdvancePC(); \
          end \
           \
      endcase \
       \
    end \
  endtask \
 \
 \
   \
task AddR0Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r0; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r0 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AddR1Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r1; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r1 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AddR2Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r2; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r2 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AddR3Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r3; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r3 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AddR4Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r4; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r4 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AddR5Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r5; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r5 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AddR6Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r6; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r6 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AddR7Constant; \
  begin       \
    case (adderStage) \
      0: \
        begin \
          adderStage <= 1; \
          adderA <= _r7; \
          adderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          adderStage <= 0; \
          _r7 <= adderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR0Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r0; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r0 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR1Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r1; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r1 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR2Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r2; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r2 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR3Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r3; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r3 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR4Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r4; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r4 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR5Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r5; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r5 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR6Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r6; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r6 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task SubtractR7Constant; \
  begin       \
    case (subtracterStage) \
      0: \
        begin \
          subtracterStage <= 1; \
          subtracterA <= _r7; \
          subtracterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          subtracterStage <= 0; \
          _r7 <= subtracterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR0Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r0; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r0 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR1Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r1; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r1 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR2Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r2; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r2 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR3Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r3; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r3 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR4Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r4; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r4 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR5Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r5; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r5 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR6Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r6; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r6 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task MultiplyR7Constant; \
  begin       \
    case (multiplierStage) \
      0: \
        begin \
          multiplierStage <= 1; \
          multiplierA <= _r7; \
          multiplierB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          multiplierStage <= 0; \
          _r7 <= multiplierC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR0Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r0; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r0 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR1Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r1; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r1 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR2Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r2; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r2 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR3Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r3; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r3 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR4Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r4; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r4 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR5Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r5; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r5 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR6Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r6; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r6 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task DivideR7Constant; \
  begin       \
    case (dividerStage) \
      0: \
        begin \
          dividerStage <= 1; \
          dividerA <= _r7; \
          dividerB <= `Operand32; \
           \
        end \
      1: dividerStage <= 2; \
      2: dividerStage <= 3; \
      3: dividerStage <= 4; \
      4: dividerStage <= 5; \
      5: dividerStage <= 6; \
      6: dividerStage <= 7; \
      7: dividerStage <= 8; \
      8: dividerStage <= 9; \
      9: dividerStage <= 10; \
      10: dividerStage <= 11; \
      11: dividerStage <= 12; \
      12: dividerStage <= 13; \
      13: dividerStage <= 14; \
      14: dividerStage <= 15; \
      15: dividerStage <= 16; \
      16: dividerStage <= 17; \
      17: dividerStage <= 18; \
      18: dividerStage <= 19; \
      19: dividerStage <= 20; \
      20: dividerStage <= 21; \
      21: dividerStage <= 22; \
      22: dividerStage <= 23; \
      23: dividerStage <= 24; \
      24: dividerStage <= 25; \
      25: dividerStage <= 26; \
      26: dividerStage <= 27; \
      27: dividerStage <= 28; \
      28: dividerStage <= 29; \
      29: dividerStage <= 30; \
 \
        30: \
          begin \
            dividerStage <= 31; \
            readInstructionEnable <= 1; \
          end \
 \
      31: \
        begin       \
          dividerStage <= 0; \
          _r7 <= dividerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR0Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r0; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r0 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR1Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r1; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r1 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR2Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r2; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r2 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR3Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r3; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r3 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR4Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r4; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r4 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR5Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r5; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r5 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR6Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r6; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r6 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task AndR7Constant; \
  begin       \
    case (anderStage) \
      0: \
        begin \
          anderStage <= 1; \
          anderA <= _r7; \
          anderB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          anderStage <= 0; \
          _r7 <= anderC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR0Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r0; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r0 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR1Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r1; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r1 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR2Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r2; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r2 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR3Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r3; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r3 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR4Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r4; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r4 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR5Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r5; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r5 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR6Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r6; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r6 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task OrR7Constant; \
  begin       \
    case (orerStage) \
      0: \
        begin \
          orerStage <= 1; \
          orerA <= _r7; \
          orerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          orerStage <= 0; \
          _r7 <= orerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR0Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r0; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r0 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR1Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r1; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r1 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR2Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r2; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r2 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR3Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r3; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r3 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR4Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r4; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r4 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR5Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r5; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r5 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR6Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r6; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r6 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task XorR7Constant; \
  begin       \
    case (xorerStage) \
      0: \
        begin \
          xorerStage <= 1; \
          xorerA <= _r7; \
          xorerB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          xorerStage <= 0; \
          _r7 <= xorerC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR0Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r0; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r0 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR1Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r1; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r1 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR2Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r2; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r2 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR3Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r3; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r3 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR4Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r4; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r4 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR5Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r5; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r5 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR6Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r6; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r6 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task LeftShiftR7Constant; \
  begin       \
    case (leftShifterStage) \
      0: \
        begin \
          leftShifterStage <= 1; \
          leftShifterA <= _r7; \
          leftShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          leftShifterStage <= 0; \
          _r7 <= leftShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR0Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r0; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r0 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR1Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r1; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r1 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR2Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r2; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r2 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR3Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r3; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r3 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR4Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r4; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r4 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR5Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r5; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r5 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR6Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r6; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r6 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
   \
task RightShiftR7Constant; \
  begin       \
    case (rightShifterStage) \
      0: \
        begin \
          rightShifterStage <= 1; \
          rightShifterA <= _r7; \
          rightShifterB <= `Operand32; \
          readInstructionEnable <= 1; \
        end \
 \
      1: \
        begin       \
          rightShifterStage <= 0; \
          _r7 <= rightShifterC;           \
          AdvancePC(); \
        end           \
    endcase       \
  end \
endtask \
 \
 \
task IncrementRegister; \
  begin \
    case (`Operand8_1) \
      0: _r0 <= _r0 + 1; \
      1: _r1 <= _r1 + 1; \
      2: _r2 <= _r2 + 1; \
      3: _r3 <= _r3 + 1; \
      4: _r4 <= _r4 + 1; \
      5: _r5 <= _r5 + 1; \
      6: _r6 <= _r6 + 1; \
      7: _r7 <= _r7 + 1; \
      default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
    endcase  \
    AdvancePC(); \
  end \
endtask \
 \
 \
task DecrementRegister; \
  begin \
    case (`Operand8_1) \
      0: _r0 <= _r0 - 1; \
      1: _r1 <= _r1 - 1; \
      2: _r2 <= _r2 - 1; \
      3: _r3 <= _r3 - 1; \
      4: _r4 <= _r4 - 1; \
      5: _r5 <= _r5 - 1; \
      6: _r6 <= _r6 - 1; \
      7: _r7 <= _r7 - 1; \
      default: begin _error <= 1; _errorCode <= `Error_ArithmeticOperand0; end \
    endcase  \
    AdvancePC(); \
  end \
endtask \


