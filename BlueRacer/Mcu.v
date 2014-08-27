`ifdef DebugCpu
`include "Cpu.v"
`endif

module Mcu #(
  parameter Architecture = 32,
  parameter DataSize = 8 + Architecture,
  parameter InstructionSize = 8 + Architecture
)(
  input wire clk,
  input wire reset,
  `ifdef DebugCpu
  output wire readInstructionStarting, 
  output wire readInstructionCompleting, 
  output wire readInstructionComplete,
  output wire [31:0] instructionAddress,
  output wire [DataSize - 1:0] instructionBuffer,
  output wire error,
  output wire isBooted,
  output wire [Architecture-1:0] errorCode,
  `endif  
  input wire [7:0] in0,
  input wire [7:0] in1,
  input wire [7:0] in2,
  input wire [7:0] in3,
  output wire [7:0] out0,
  output wire [7:0] out1,
  output wire [7:0] out2,
  output wire [7:0] out3,
  input wire sck,
  input wire ss,
  input wire mosi,
  output wire miso
);

wire [31:0] writeInstructionAddress;
wire [DataSize - 1:0] writeInstructionBuffer;
wire writeInstructionEnable, writeInstructionComplete;

`ifndef DebugCpu
wire [31:0] instructionAddress;
wire [DataSize - 1:0] instructionBuffer;
wire readInstructionStarting, readInstructionCompleting, readInstructionComplete;
`endif

wire [31:0] read4Address, read4Buffer;
wire read4Enable, read4Starting, read4Completing, read4Complete;

wire write4;
wire [31:0] write4Address, write4Buffer;
wire write4Completing, write4Complete;

MemoryController memory(
  .clk(clk),
  .reset(reset),
  .read5Address(instructionAddress),
  .read5(readInstructionEnable),
  .read5Buffer(instructionBuffer),
  .read5Starting(readInstructionStarting),
  .read5Complete(readInstructionComplete),
  .read5Completing(readInstructionCompleting),
  .read1(0),
  .read1Address(0),
  .read2(0),
  .read2Address(0),
  .read4(read4Enable),
  .read4Address(read4Address),
  .read4Buffer(read4Buffer),
  .read4Complete(read4Complete),
  .read4Completing(read4Completing),
  
  .write1(0),
  .write1Address(0),
  .write1Buffer(0),
  
  .write4(write4),
  .write4Address(write4Address),
  .write4Buffer(write4Buffer),
  .write4Completing(write4Completing),
  .write4Complete(write4Complete),
  
  .write5(writeInstructionEnable),
  .write5Address(writeInstructionAddress),  
  .write5Buffer(writeInstructionBuffer),
  .write5Complete(writeInstructionComplete)
);

Cpu cpu(
  .clk(clk),
  .reset(reset),
  
  .writeInstructionAddress(writeInstructionAddress),
  .writeInstructionBuffer(writeInstructionBuffer),
  .writeInstructionEnable(writeInstructionEnable),
  .writeInstructionComplete(writeInstructionComplete),
  
  .instructionAddress(instructionAddress),
  .instructionBuffer(instructionBuffer),
  .readInstructionEnable(readInstructionEnable),
  .readInstructionStarting(readInstructionStarting),
  .readInstructionCompleting(readInstructionCompleting),
  .readInstructionComplete(readInstructionComplete),
  
  .read4Enable(read4Enable),
  .read4Address(read4Address),
  .read4Buffer(read4Buffer),
  .read4Completing(read4Completing),
  .read4Complete(read4Complete),
  
  .write4Enable(write4),
  .write4Address(write4Address),
  .write4Buffer(write4Buffer),
  .write4Completing(write4Completing),
  .write4Complete(write4Complete),
  
  .in0(in0),
  .in1(in1),
  .in2(in2),
  .in3(in3),
  .out0(out0),
  .out1(out1),
  .out2(out2),
  .out3(out3),
  
  `ifdef DebugCpu
  ._error(error),
  ._isBooted(isBooted),
  ._errorCode(errorCode),
  `endif
  
  .sck(sck),
  .ss(ss),
  .mosi(mosi),
  .miso(miso)
);
endmodule