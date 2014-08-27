`include "GlobalDefines.v"

`ifdef DebugCpu
`include "Adder.v"
`include "Subtracter.v"
`include "Multiplier.v"
`include "Divider.v"
`include "Ander.v"
`include "Orer.v"
`include "Xorer.v"
`include "LeftShifter.v"
`include "RightShifter.v"
`endif

`include "Error.v"
`include "Opcodes.v"
`include "OpcodeCases.v"
`include "ArithmeticModules.v"

`ifdef ExternalROM
`else
`include "Program.v"
`endif

`define PageShift 8

module Cpu #(
  parameter Architecture = 32,
  parameter DataSize = 8,
  parameter InstructionSize = 8 + Architecture
)(
  input wire clk,
  input wire reset,
  
  output reg [Architecture-1:0] writeInstructionAddress = 0,
  output reg [InstructionSize-1:0] writeInstructionBuffer = 0,
  output reg writeInstructionEnable = 0,  
  input wire writeInstructionComplete, 
  
  output reg [Architecture-1:0] instructionAddress = 0,
  input wire [InstructionSize-1:0] instructionBuffer,
  output reg readInstructionEnable = 0,
  input wire readInstructionStarting,
  input wire readInstructionCompleting,
  input wire readInstructionComplete,

  output reg [Architecture-1:0] read4Address = 0,
  input wire [DataSize*4-1:0] read4Buffer,
  output reg read4Enable = 0,
  input wire read4Starting,
  input wire read4Completing,
  input wire read4Complete,  
  
  output reg [Architecture-1:0] write4Address = 0,
  output reg [DataSize*4-1:0] write4Buffer,
  output reg write4Enable = 0,
  input wire write4Starting,
  input wire write4Completing,
  input wire write4Complete,  
  
  input wire [7:0] in0,
  input wire [7:0] in1,
  input wire [7:0] in2,
  input wire [7:0] in3,
  output reg [7:0] out0 = 0,
  output reg [7:0] out1 = 0,
  output reg [7:0] out2 = 0,
  output reg [7:0] out3 = 0,
  
  `ifdef DebugCpu
  output reg _error = 0,
  output reg _isBooted = 0,
  output reg [Architecture-1:0] _errorCode,
  `endif
  
  input wire sck,
  input wire ss,
  input wire mosi,
  output wire miso
);

`RegisterDeclarations

`ifndef DebugCpu
reg _error = 0, _isBooted = 0;
reg [Architecture-1:0] _errorCode;
`endif

reg _isEq = 0;
reg [15:0] _programmerState = 0; 
reg firstInstruction = 1;
reg [1:0] _gotoStage = 0;
reg [2:0] _read4Stage = 0;
reg [2:0] _write4Stage = 0;
reg [3:0] _pushStage = 0;
reg [3:0] _popStage = 0;
reg [3:0] _callStage = 0;
reg [3:0] _returnStage = 0;
reg [2:0] pageInfo[0:`Pages-1];
reg _break = 0, _isBroken = 0;

wire [31:0] mosiBufferUnsafe, mosiBuffer2;
reg [31:0] misoBuffer;
wire mosiShiftComplete, mosiShiftComplete2;
reg mosiBufferHandled = 0;
reg [7:0] _commandState = 0;

reg [31:0] pageNumber = 0;
reg [InstructionSize-1:0] _lastInstruction0 = 0, _lastInstruction1 = 0, _lastInstruction2 = 0;
reg instructionSaved = 0;
reg [Architecture-1:0] _executableBoundary = 0;

`define Opcode instructionBuffer[39:32]
`define SubOpcode instructionBuffer[31:24]

`define Operand32 instructionBuffer[31:0]
`define Operand8_3 instructionBuffer[7:0]
`define Operand8_2 instructionBuffer[15:8]
`define Operand8_1 instructionBuffer[23:16]
`define Operand8_0 instructionBuffer[31:24]

initial
  begin
    for(pageNumber = 0; pageNumber < `Pages; pageNumber = pageNumber + 1) 
      begin : ResetPages
        pageInfo[pageNumber] <= 3'b011;
      end    
  end

task AdvancePC;
  begin
    instructionAddress <= instructionAddress + (InstructionSize / 8);    
    _programCounter <= instructionAddress;
  end
endtask

`ArithmeticModules

always @(posedge clk or posedge reset)
  begin
    if (reset)
      begin
        Reset();
      end  
    else 
      begin
        if (readInstructionComplete)
          begin
            _lastInstruction0 <= instructionBuffer;
            _lastInstruction1 <= _lastInstruction0;
            _lastInstruction2 <= _lastInstruction1;
          end
        
        if (_isBooted)
          begin
            if (~_isBroken)
              begin
                //if (readInstructionEnable && instructionAddress >= _executableBoundary)
                // if (_programCounter >= _executableBoundary)
                  // begin
                    // `SetError(`Error_Execute)
                  // end
              
                if (readInstructionStarting)
                  begin
                  
                    if(_break && 
                      _gotoStage == 0 && 
                      adderStage == 0 && 
                      subtracterStage == 0  && 
                      multiplierStage == 0  && 
                      dividerStage == 0 && 
                      anderStage == 0 && 
                      orerStage == 0 && 
                      xorerStage == 0 && 
                      leftShifterStage == 0 &&
                      rightShifterStage == 0 &&
                      _read4Stage == 0 && 
                      _write4Stage == 0 &&
                      _pushStage == 0 &&
                      _popStage == 0 &&
                      _callStage == 0 &&
                      _returnStage == 0)
                      begin
                        _isBroken <= 1;
                        readInstructionEnable <= 0;
                      end
                    else
                      begin
                        case (`Opcode)
                          `MultiStageOpcodes: readInstructionEnable <= 0;
                          default: readInstructionEnable <= 1;
                        endcase
                      end
                      
                  end
                else if (readInstructionCompleting)
                  begin
                    if (~pageInfo[_programCounter >> `PageShift][2]) `SetError(`Error_Execute)
                  end
                else if (_error)
                  begin
                  end
                else if (readInstructionComplete || 
                  _gotoStage != 0 || 
                  adderStage != 0 || 
                  subtracterStage != 0  || 
                  multiplierStage != 0  || 
                  dividerStage != 0 || 
                  anderStage != 0 || 
                  orerStage != 0 || 
                  xorerStage != 0 || 
                  leftShifterStage != 0 || 
                  rightShifterStage != 0 || 
                  _read4Stage != 0 || 
                  _write4Stage != 0 ||
                  _pushStage != 0 ||
                  _popStage != 0 ||
                  _callStage != 0 ||
                  _returnStage != 0)
                  begin
                    ExecuteInstruction();                    
                  end
                else if (firstInstruction)
                  begin
                    firstInstruction <= 0;
                    AdvancePC();
                  end
              end
            else
              begin
                _isBroken <= _break;
              end
          end
        else
          begin
            Boot();
          end
          
        HandleSpi();
      end
  end

task ExecuteInstruction;
  case (`Opcode)
    `OpcodeAssignCase(`Assign_R0_Const, _r0)
    `OpcodeAssignCase(`Assign_R1_Const, _r1)
    `OpcodeAssignCase(`Assign_R2_Const, _r2)
    `OpcodeAssignCase(`Assign_R3_Const, _r3)
    `OpcodeAssignCase(`Assign_R4_Const, _r4)
    `OpcodeAssignCase(`Assign_R5_Const, _r5)
    `OpcodeAssignCase(`Assign_R6_Const, _r6)
    `OpcodeAssignCase(`Assign_R7_Const, _r7)
    
    `OpcodeEqCase(`Eq_R0_C, _r0)
    `OpcodeEqCase(`Eq_R1_C, _r1)
    `OpcodeEqCase(`Eq_R2_C, _r2)
    `OpcodeEqCase(`Eq_R3_C, _r3)
    `OpcodeEqCase(`Eq_R4_C, _r4)
    `OpcodeEqCase(`Eq_R5_C, _r5)
    `OpcodeEqCase(`Eq_R6_C, _r6)
    `OpcodeEqCase(`Eq_R7_C, _r7)
    
    `OpcodeNotEqCase(`NotEq_R0_C, _r0)
    `OpcodeNotEqCase(`NotEq_R1_C, _r1)
    `OpcodeNotEqCase(`NotEq_R2_C, _r2)
    `OpcodeNotEqCase(`NotEq_R3_C, _r3)
    `OpcodeNotEqCase(`NotEq_R4_C, _r4)
    `OpcodeNotEqCase(`NotEq_R5_C, _r5)
    `OpcodeNotEqCase(`NotEq_R6_C, _r6)
    `OpcodeNotEqCase(`NotEq_R7_C, _r7)
    
    `OpcodeLtCase(`LessThan_R0_C, _r0)
    `OpcodeLtCase(`LessThan_R1_C, _r1)
    `OpcodeLtCase(`LessThan_R2_C, _r2)
    `OpcodeLtCase(`LessThan_R3_C, _r3)
    `OpcodeLtCase(`LessThan_R4_C, _r4)
    `OpcodeLtCase(`LessThan_R5_C, _r5)
    `OpcodeLtCase(`LessThan_R6_C, _r6)
    `OpcodeLtCase(`LessThan_R7_C, _r7)
    
    `OpcodeLtEqCase(`LessThanEq_R0_C, _r0)
    `OpcodeLtEqCase(`LessThanEq_R1_C, _r1)
    `OpcodeLtEqCase(`LessThanEq_R2_C, _r2)
    `OpcodeLtEqCase(`LessThanEq_R3_C, _r3)
    `OpcodeLtEqCase(`LessThanEq_R4_C, _r4)
    `OpcodeLtEqCase(`LessThanEq_R5_C, _r5)
    `OpcodeLtEqCase(`LessThanEq_R6_C, _r6)
    `OpcodeLtEqCase(`LessThanEq_R7_C, _r7)
    
    `OpcodeGtCase(`GreaterThan_R0_C, _r0)
    `OpcodeGtCase(`GreaterThan_R1_C, _r1)
    `OpcodeGtCase(`GreaterThan_R2_C, _r2)
    `OpcodeGtCase(`GreaterThan_R3_C, _r3)
    `OpcodeGtCase(`GreaterThan_R4_C, _r4)
    `OpcodeGtCase(`GreaterThan_R5_C, _r5)
    `OpcodeGtCase(`GreaterThan_R6_C, _r6)
    `OpcodeGtCase(`GreaterThan_R7_C, _r7)
    
    `OpcodeGtEqCase(`GreaterThanEq_R0_C, _r0)
    `OpcodeGtEqCase(`GreaterThanEq_R1_C, _r1)
    `OpcodeGtEqCase(`GreaterThanEq_R2_C, _r2)
    `OpcodeGtEqCase(`GreaterThanEq_R3_C, _r3)
    `OpcodeGtEqCase(`GreaterThanEq_R4_C, _r4)
    `OpcodeGtEqCase(`GreaterThanEq_R5_C, _r5)
    `OpcodeGtEqCase(`GreaterThanEq_R6_C, _r6)
    `OpcodeGtEqCase(`GreaterThanEq_R7_C, _r7)
     
    `Goto_Const:
      begin
        case (_gotoStage)
          0:
            begin
              instructionAddress <= `Operand32;
              readInstructionEnable <= 1;
              _gotoStage <= 1;
            end
          1:
            begin        
              _gotoStage <= 0;
              AdvancePC();
            end
        endcase
      end
     
    `Goto_Eq:
      begin
        case (_gotoStage)
          0:
            begin
              readInstructionEnable <= 1;
            
              if (_isEq)
                begin
                  instructionAddress <= `Operand32;
                end
              /*else
                begin
                  AdvancePC();
                end*/
              _gotoStage <= 1;
            end
          1:
            begin
              _gotoStage <= 0;
              AdvancePC();              
            end
        endcase
      end
     
    `Goto_NotEq:
      begin
        case (_gotoStage)
          0:
            begin
              readInstructionEnable <= 1;
            
              if (~_isEq)
                begin
                  instructionAddress <= `Operand32;
                end
              /*else
                begin
                  AdvancePC();
                end*/
              _gotoStage <= 1;
            end
          1:
            begin
              _gotoStage <= 0;
              AdvancePC();              
            end
        endcase
      end
      
    `Call_Const: CallConstant();
    `Push_Const: PushConstant();
   
    `AddCases
    `SubtractCases
    `MultiplyCases
    `DivideCases
    `AndCases
    `OrCases
    `XorCases
    `LeftShiftCases
    `RightShiftCases
   
    `define SetOut_ConstCase(__opcode, __out) __opcode: begin __out <= `Operand8_3; AdvancePC(); end
    `SetOut_ConstCase(`SetOut0_Const, out0)
    `SetOut_ConstCase(`SetOut1_Const, out1)
    `SetOut_ConstCase(`SetOut2_Const, out2)
    `SetOut_ConstCase(`SetOut3_Const, out3)
      
    `Ext:
      case(`SubOpcode)
        `Inc_R: IncrementRegister();
        `Dec_R: DecrementRegister();
        `In: InRegister();
        `Out: OutRegister();
        `CompareRRCases
        `Assign:
          begin
            case (`Operand8_1)
              `AssignRRCases
            endcase
            AdvancePC();
          end
        default: `SetError(`Error_SubOpcode)
      endcase
       
    `ExtMultiStage:
      case(`SubOpcode)
        `Add_R_R: AddRegisters();
        `Sub_R_R: SubtractRegisters();
        `Mul_R_R: MultiplyRegisters();
        `Div_R_R: DivideRegisters();
        `And_R_R: AndRegisters();
        `Or_R_R: OrRegisters();
        `Xor_R_R: XorRegisters();
        `LeftShift_R_R: LeftShiftRegisters();
        `RightShift_R_R: RightShiftRegisters();
        `Read: ReadRegisterRegister();
        `Write: WriteRegisterRegister();
        `Push_R: PushRegister();
        `Pop_R: PopRegister();
        `Call_R: CallRegister();
        `Return: Return();
        default: `SetError(`Error_SubOpcode2)
      endcase
      
    `Error: `SetError(`Error_General)
     
    default: `SetError(`Error_Opcode)
      
  endcase
endtask 

task OutRegister;
  begin
    case (`Operand8_1)
      0:
        case (`Operand8_2)
          0: out0 <= _r0;
          1: out0 <= _r1;
          2: out0 <= _r2;
          3: out0 <= _r3;
          4: out0 <= _r4;
          5: out0 <= _r5;
          6: out0 <= _r6;
          7: out0 <= _r7;
          default: `SetError(`Error_Operand)
        endcase
      1:
        case (`Operand8_2)
          0: out1 <= _r0;
          1: out1 <= _r1;
          2: out1 <= _r2;
          3: out1 <= _r3;
          4: out1 <= _r4;
          5: out1 <= _r5;
          6: out1 <= _r6;
          7: out1 <= _r7;
          default: `SetError(`Error_Operand)
        endcase
      2:
        case (`Operand8_2)
          0: out2 <= _r0;
          1: out2 <= _r1;
          2: out2 <= _r2;
          3: out2 <= _r3;
          4: out2 <= _r4;
          5: out2 <= _r5;
          6: out2 <= _r6;
          7: out2 <= _r7;
          default: `SetError(`Error_Operand)
        endcase
      3:
        case (`Operand8_2)
          0: out3 <= _r0;
          1: out3 <= _r1;
          2: out3 <= _r2;
          3: out3 <= _r3;
          4: out3 <= _r4;
          5: out3 <= _r5;
          6: out3 <= _r6;
          7: out3 <= _r7;
          default: `SetError(`Error_Operand)
        endcase
      default: `SetError(`Error_Operand)
    endcase
    AdvancePC();
  end
endtask

task InRegister;
  begin
    case (`Operand8_1)
      0:
        begin
          case (`Operand8_2)
            0: _r0 <= in0;
            1: _r0 <= in1;
            2: _r0 <= in2;
            3: _r0 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      1:
        begin
          case (`Operand8_2)
            0: _r1 <= in0;
            1: _r1 <= in1;
            2: _r1 <= in2;
            3: _r1 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      2:
        begin
          case (`Operand8_2)
            0: _r2 <= in0;
            1: _r2 <= in1;
            2: _r2 <= in2;
            3: _r2 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      3:
        begin
          case (`Operand8_2)
            0: _r3 <= in0;
            1: _r3 <= in1;
            2: _r3 <= in2;
            3: _r3 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      4:
        begin
          case (`Operand8_2)
            0: _r4 <= in0;
            1: _r4 <= in1;
            2: _r4 <= in2;
            3: _r4 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      5:
        begin
          case (`Operand8_2)
            0: _r5 <= in0;
            1: _r5 <= in1;
            2: _r5 <= in2;
            3: _r5 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      6:
        begin
          case (`Operand8_2)
            0: _r6 <= in0;
            1: _r6 <= in1;
            2: _r6 <= in2;
            3: _r6 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      7:
        begin
          case (`Operand8_2)
            0: _r7 <= in0;
            1: _r7 <= in1;
            2: _r7 <= in2;
            3: _r7 <= in3;          
            default: `SetError(`Error_Operand)
          endcase
        end
      default: `SetError(`Error_Operand)
    endcase
    AdvancePC();
  end
endtask

task ReadRegisterRegister;
    case (_read4Stage)
      0:
        begin
          _read4Stage <= 1;
          read4Enable <= 1;
          // Todo: work at bit level for registers to get larger offsets
          case (`Operand8_2)
            `ReadRegisterCases            
            default: `SetError(`Error_Operand)
          endcase   
        end
      1:
        begin
          read4Enable <= 0;
        
          if (pageInfo[read4Address >> `PageShift][0])
            begin
              if (read4Completing)
                begin
                  readInstructionEnable <= 1;
                end
              else if (read4Complete)
                begin
                  AdvancePC();
                  _read4Stage <= 0;
                  case (`Operand8_1)
                    0: _r0 <= read4Buffer;
                    1: _r1 <= read4Buffer;
                    2: _r2 <= read4Buffer;
                    3: _r3 <= read4Buffer;
                    4: _r4 <= read4Buffer;
                    5: _r5 <= read4Buffer;
                    6: _r6 <= read4Buffer;
                    7: _r7 <= read4Buffer;
                    default: `SetError(`Error_Operand)
                  endcase
                end
            end
          else
            begin
              `SetError(`Error_Read)
              _read4Stage <= 0;
            end
        end
    endcase  
endtask

task WriteRegisterRegister;
  case (_write4Stage) 
    0:
      begin
        _write4Stage <= 1;
        write4Enable <= 1;
        
        case (`Operand8_1)
          0: write4Address <= _r0;
          1: write4Address <= _r1;
          2: write4Address <= _r2;
          3: write4Address <= _r3;
          4: write4Address <= _r4;
          5: write4Address <= _r5;
          6: write4Address <= _r6;
          7: write4Address <= _r7;
          default: `SetError(`Error_Operand)
        endcase
        
        case (`Operand8_2)
          0: write4Buffer <= _r0;
          1: write4Buffer <= _r1;
          2: write4Buffer <= _r2;
          3: write4Buffer <= _r3;
          4: write4Buffer <= _r4;
          5: write4Buffer <= _r5;
          6: write4Buffer <= _r6;
          7: write4Buffer <= _r7;
          default: `SetError(`Error_Operand)
        endcase
      end
    1:
      begin
        write4Enable <= 0;
        
        if (pageInfo[write4Address >> `PageShift][1])
          begin
            _write4Stage <= 2;
          end
        else
          begin
            _write4Stage <= 0;
            `SetError(`Error_Write)
          end
      end
    2:
      begin
        if (write4Completing)
          begin
            readInstructionEnable <= 1;
          end
        else if (write4Complete)
          begin
            AdvancePC();
            _write4Stage <= 0;                  
          end
      end
  endcase
endtask
  
task GrowStack;
  begin
    write4Enable <= 1;
    write4Address <= _r0 - (Architecture/8);
    _r0 <= _r0 - (Architecture/8);
  end
endtask
  
task CallRegister;
  case (_callStage)
    0:
      begin
        case (`Operand8_1)
          0: instructionAddress <= _r0;
          1: instructionAddress <= _r1;
          2: instructionAddress <= _r2;
          3: instructionAddress <= _r3;
          4: instructionAddress <= _r4;
          5: instructionAddress <= _r5;
          6: instructionAddress <= _r6;
          7: instructionAddress <= _r7;
          default: `SetError(`Error_Operand)
        endcase
        write4Buffer <= instructionAddress;
        GrowStack();
        _callStage <= 1;
      end
    1:
      begin
        if (write4Completing)
          begin
            write4Enable <= 0;
            readInstructionEnable <= 1;
            _callStage <= 2;
          end
      end
    2:
      begin
        AdvancePC();
        _callStage <= 0;
      end
    default: `SetError(`Error_InvalidState)
  endcase
endtask 

task CallConstant;
  case (_callStage)
    0:
      begin
        instructionAddress <= `Operand32;
        write4Buffer <= instructionAddress;
        GrowStack();
        _callStage <= 1;
      end
    1:
      begin
        if (write4Completing)
          begin
            write4Enable <= 0;
            readInstructionEnable <= 1;
            _callStage <= 2;
          end
      end
    2:
      begin
        AdvancePC();
        _callStage <= 0;
      end
    default: `SetError(`Error_InvalidState)
  endcase
endtask 

task Return;
  case (_returnStage)
    0:
      begin        
        read4Address <= _r0;
        read4Enable <= 1;
        _r0 <= _r0 + Architecture/8;
        _returnStage <= 1;
      end
    1:
      begin
        read4Enable <= 0;
        _returnStage <= 2;
      end
    2:
      begin
        if (read4Complete)
          begin
            instructionAddress <= read4Buffer;
            readInstructionEnable <= 1;
            _returnStage <= 3;
          end
      end
    3:
      begin
        AdvancePC();
        _returnStage <= 0;
      end
    default: `SetError(`Error_InvalidState)
  endcase
endtask
  
task PushRegister;
  case (_pushStage)
    0:
      begin
        case (`Operand8_1)
          0: write4Buffer <= _r0;
          1: write4Buffer <= _r1;
          2: write4Buffer <= _r2;
          3: write4Buffer <= _r3;
          4: write4Buffer <= _r4;
          5: write4Buffer <= _r5;
          6: write4Buffer <= _r6;
          7: write4Buffer <= _r7;
          default: `SetError(`Error_Operand)
        endcase
        GrowStack();
        _pushStage <= 1;
      end
    1:
      begin
        if (write4Completing)
          begin
            write4Enable <= 0;
            readInstructionEnable <= 1;
            _pushStage <= 2;
          end
      end
    2:
      begin
        AdvancePC();
        _pushStage <= 0;
      end
    default: `SetError(`Error_InvalidState)
  endcase
endtask 

task PushConstant;
  case (_pushStage)
    0:
      begin
        write4Buffer <= `Operand32;
        GrowStack();
        _pushStage <= 1;
      end
    1:
      begin
        if (write4Completing)
          begin
            write4Enable <= 0;
            readInstructionEnable <= 1;
            _pushStage <= 2;
          end
      end
    2:
      begin
        AdvancePC();
        _pushStage <= 0;
      end
    default: `SetError(`Error_InvalidState)
  endcase
endtask 

task PopRegister;
  case (_popStage)
    0:
      begin        
        read4Address <= _r0;
        read4Enable <= 1;
        _r0 <= _r0 + Architecture/8;
        _popStage <= 1;
      end
    1:
      begin
        //if (read4Starting)
          //begin
            read4Enable <= 0;
            _popStage <= 2;
          //end
      end
    2:
      begin
        if (read4Completing)
          begin            
            readInstructionEnable <= 1;
            _popStage <= 3;
          end
      end
    3:
      begin
        AdvancePC();
        _popStage <= 0;
        case (`Operand8_1)
          0: _r0 <= read4Buffer;
          1: _r1 <= read4Buffer;
          2: _r2 <= read4Buffer;
          3: _r3 <= read4Buffer;
          4: _r4 <= read4Buffer;
          5: _r5 <= read4Buffer;
          6: _r6 <= read4Buffer;
          7: _r7 <= read4Buffer;
          default: `SetError(`Error_Operand)
        endcase
      end
    default: `SetError(`Error_InvalidState)
  endcase
endtask
 
task Boot;
  begin
    if (writeInstructionAddress < `ProgramSize)
      begin
        _isBooted <= 0;
        _executableBoundary <= `ProgramSize;
        if (_programmerState == 0 || writeInstructionComplete)
          begin
            if (writeInstructionAddress != `ProgramSize - 5)
              begin
                _programmerState <= _programmerState + 5;
                writeInstructionAddress <= _programmerState;
                writeInstructionEnable <= 1;
                pageInfo[_programmerState >> `PageShift][0] <= 1;
                pageInfo[_programmerState >> `PageShift][1] <= 0;
                pageInfo[_programmerState >> `PageShift][2] <= 1;
                //pageInfo[_programmerState >> `PageShift][2] <= 0;                

                case (_programmerState) // synthesis full_case
                  `Program
                  default: writeInstructionBuffer <= 0;
                endcase      
              end
            else
              begin
                writeInstructionEnable <= 0;
                writeInstructionAddress <= 0;
                _isBooted <= 1;
                readInstructionEnable <= 1;
              end
          end
        else
          begin
            writeInstructionEnable <= 0;
          end        
      end
    else
      begin
        writeInstructionEnable <= 0;
        writeInstructionAddress <= 0;
        _isBooted <= 1;
        readInstructionEnable <= 1;
      end
  end
endtask



SpiSlave #(.BufferSize(32)) spi(
  .sck(sck), 
  .ss(ss), 
  .mosi(mosi), 
  .miso(miso), 
  .mosiBuffer(mosiBufferUnsafe), 
  .misoBuffer(misoBuffer),
  .shiftComplete(mosiShiftComplete)
);

CrossDomainBuffer buffer(
  .clk(clk),
  .in(mosiBufferUnsafe),
  .save(mosiShiftComplete),
  .out(mosiBuffer2),
  .saved(mosiShiftComplete2)
);

`define RegCmdCase(__command, __register) \
__command:                                \
  begin                                   \
    misoBuffer <= __register;             \
  end                                     \


`define StateCmdCase(__command, __state)  \
__command:                                \
  begin                                   \
    _commandState <= __state;             \
    misoBuffer <= __command;              \
  end                                     \
  
reg _spiRead4 = 0, _spiWrite4 = 0;
reg [31:0] _spiPageNumber = 0;

task HandleSpi;
  begin  
    if (_spiWrite4)
      begin
        _spiWrite4 <= 0;
        write4Enable <= 0;
      end
      
    if (_spiRead4)
      begin
        // read4Enable <= 0;
        
        if (read4Completing)
          begin
            read4Enable <= 0;
          end
        else if (read4Complete)
          begin
            _spiRead4 <= 0;
            misoBuffer <= read4Buffer;
          end
        else
          begin
            misoBuffer <= 32'hdeadbeef;
          end
      end
      
    if (mosiShiftComplete2)
      begin
        if (~mosiBufferHandled)
          begin
            mosiBufferHandled <= 1;
            //misoBuffer <= mosiBuffer2;
            
            case (_commandState)
              
              `CommandState_Waiting:
                begin
                  case (mosiBuffer2)
                  
                    `Command_Nop:
                      begin
                        misoBuffer <= `Command_Nop;
                      end
                  
                    `RegCmdCase(`Command_GetProgramCounter, _programCounter)                    
                    `RegCmdCase(`Command_GetInstructionAddress, instructionAddress)
                    `RegCmdCase(`Command_GetOpcode, `Opcode)                    
                    `RegCmdCase(`Command_GetOperands, `Operand32)
                    `RegCmdCase(`Command_GetError, _error)
                    `RegCmdCase(`Command_GetErrorCode, _errorCode)
                    
                    `Command_Break: 
                      begin
                        //readInstructionEnable <= 0;
                        _break <= 1;
                        misoBuffer <= `Command_Break;
                      end
                    
                    `Command_Continue:
                      begin
                        readInstructionEnable <= 1;
                        
                        // case (`Opcode)
                          // `MultiStageOpcodes: readInstructionEnable <= 0;
                          // default: readInstructionEnable <= 1;
                        // endcase
                        
                        _break <= 0;
                        misoBuffer <= `Command_Continue;
                      end
                      
                    `Command_Restart:
                      begin
                        // readInstructionEnable <= 1;
                        // firstInstruction <= 1;
                        // _programCounter <= 0;
                        // instructionAddress <= 0;                        
                        // _break <= 0;
                        Restart();
                        //readInstructionEnable <= 1;
                        misoBuffer <= `Command_Restart;
                      end
                    
                    `Command_Reset:
                      begin
                        Reset();
                        misoBuffer <= `Command_Reset;
                      end
                    
                    
                    `StateCmdCase(`Command_ReadAddress, `CommandState_ReadAddress)                    
                    
                    `Command_Read4:
                      begin
                        read4Enable <= 1;
                        _spiRead4 <= 1;    
                      end
                    
                    `StateCmdCase(`Command_WriteAddress, `CommandState_WriteAddress)
                    `StateCmdCase(`Command_Write4, `CommandState_Write4)                    
                    
                    `StateCmdCase(`Command_GetPageFlags, `CommandState_WaitingGetPageNumber)                    
                    `StateCmdCase(`Command_SetPageFlags, `CommandState_WaitingSetPageNumber)
                    
                    default: `SetError(`Error_InvalidCommand)
                  endcase
                end
                
              `CommandState_ReadAddress:
                begin
                  read4Address <= mosiBuffer2;
                  _commandState <= `CommandState_Waiting;
                end
                
              `CommandState_Write4:
                begin
                  write4Buffer <= mosiBuffer2;
                  
                  write4Enable <= 1;
                  _spiWrite4 <= 1;
                  _commandState <= `CommandState_Waiting;
                end
              
              `CommandState_WriteAddress:
                begin
                  write4Address <= mosiBuffer2;
                  _commandState <= `CommandState_Waiting;
                end
                
              `CommandState_WaitingGetPageNumber:
                begin
                  misoBuffer <= pageInfo[mosiBuffer2];
                  _commandState <= `CommandState_Waiting;
                end
                
              `CommandState_WaitingSetPageNumber:
                begin
                  //_spiPageNumber <= mosiBuffer2;
                  //
                  // pageInfo[mosiBuffer2[28:0]][0] <= mosiBuffer2[29];
                  // pageInfo[mosiBuffer2[28:0]][1] <= mosiBuffer2[30];
                  // pageInfo[mosiBuffer2[28:0]][2] <= mosiBuffer2[31];
                  //_commandState <= `CommandState_WaitingSetPageFlags;
                  
                  pageInfo[mosiBuffer2[28:0]] <= mosiBuffer2[31:29];
                  _commandState <= `CommandState_Waiting;
                end
              
              `CommandState_WaitingSetPageFlags:
                begin
                  // pageInfo[_spiPageNumber][0] <= mosiBuffer2[0] ? 1'b1 : 1'b0;
                  // pageInfo[_spiPageNumber][1] <= mosiBuffer2[1] ? 1'b1 : 1'b0;
                  // pageInfo[_spiPageNumber][2] <= mosiBuffer2[2] ? 1'b1 : 1'b0;
                  pageInfo[_spiPageNumber] <= mosiBuffer2[2:0];
                  _commandState <= `CommandState_Waiting;
                end
              
                
              default: `SetError(`Error_InvalidCommandState)
            endcase
          end
      end
    else
      begin
        mosiBufferHandled <= 0;
      end
  end
endtask



task Reset;
  begin
    writeInstructionAddress <= 0;
    writeInstructionBuffer <= 0;
    writeInstructionEnable <= 0;
    
    instructionAddress <= 0;
    readInstructionEnable <= 0;
    
    read4Address <= 0;
    read4Enable <= 0;
    _read4Stage <= 0;
    
    write4Address <= 0;
    write4Buffer <= 0;
    write4Enable <= 0;    
    _write4Stage <= 0;
    
    out0 <= 0;
    out1 <= 0;
    out2 <= 0;
    out3 <= 0;
    
    _isBooted <= 0;
    _isEq <= 0;
    _error <= 0;
    _errorCode <= 0;
    _programmerState <= 0;
    firstInstruction <= 1;
        
    `ResetRegisters
    
    _programCounter <= 0;
    _gotoStage <= 0;
    _pushStage <= 0;
    _popStage <= 0;
    _callStage <= 0;
    _returnStage <= 0;
    _break <= 0;
    _isBroken <= 0;
    
    adderA <= 0;
    adderB <= 0;
    adderStage <= 0;    
    
    misoBuffer <= 0;
    mosiBufferHandled <= 0;
    _spiPageNumber <= 0;
    
    _commandState <= 0;
    _spiRead4 <= 0;
    _spiWrite4 <= 0;
    _lastInstruction0 <= 0;
    _lastInstruction1 <= 0;
    _lastInstruction2 <= 0;
    
    _executableBoundary <= 0;
    
    for(pageNumber = 0; pageNumber < `Pages; pageNumber = pageNumber + 1) 
      begin : ResetPages
        pageInfo[pageNumber] <= 3'b011;
      end    
  end
endtask

task Restart;
  begin
    writeInstructionAddress <= 0;
    writeInstructionBuffer <= 0;
    writeInstructionEnable <= 0;
    
    instructionAddress <= 0;
    readInstructionEnable <= 1;
    
    read4Address <= 0;
    read4Enable <= 0;
    _read4Stage <= 0;
    
    write4Address <= 0;
    write4Buffer <= 0;
    write4Enable <= 0;    
    _write4Stage <= 0;
    
    out0 <= 0;
    out1 <= 0;
    out2 <= 0;
    out3 <= 0;
    
    _isEq <= 0;
    _error <= 0;
    _errorCode <= 0;
    
    firstInstruction <= 0;
        
    `ResetRegisters
    
    _programCounter <= 0;
    _gotoStage <= 0;
    _pushStage <= 0;
    _popStage <= 0;
    _callStage <= 0;
    _returnStage <= 0;
    _break <= 0;
    _isBroken <= 0;
    
    adderA <= 0;
    adderB <= 0;
    adderStage <= 0;    
    
    misoBuffer <= 0;
    mosiBufferHandled <= 0;
    _spiPageNumber <= 0;
    
    _commandState <= 0;
    _lastInstruction0 <= 0;
    _lastInstruction1 <= 0;
    _lastInstruction2 <= 0;
    
    _executableBoundary <= 0;
    
    // _spiRead4 <= 0;
    // _spiWrite4 <= 0;
  end
endtask

endmodule