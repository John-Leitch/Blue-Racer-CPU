//`timescale 1ns / 1ns
`timescale 1 ps / 1 ps

`define DebugCpu 1
`define ExternalROM 1
`include "{ProgramVerilogFile}"
`include "Mcu.v"
`include "CrossDomainBuffer.v"
`include "MemoryController.v"
`include "simple_dual_port_ram_single_clock.v"
`include "SpiSlave.v"

module McuTest2;
  
reg clk, reset;

wire [31:0] instructionAddress;
wire [39:0] instructionBuffer;

wire readInstructionStarting, 
  readInstructionCompleting, 
  readInstructionComplete;
  
wire isBooted;
wire error;
wire [31:0] errorCode;

reg [7:0] in0 = 32'h00000000,
  in1 = 32'h00000000,
  in2 = 32'h00000000,
  in3 = 32'h00000000;

wire [7:0] out0, out1, out2, out3;

Mcu mcu(
  .clk(clk), 
  .reset(reset), 
  .instructionAddress(instructionAddress),
  .instructionBuffer(instructionBuffer),
  .readInstructionStarting(readInstructionStarting),
  .readInstructionCompleting(readInstructionCompleting),
  .readInstructionComplete(readInstructionComplete),
  .in0(in0),   
  .in1(in1), 
  .in2(in2), 
  .in3(in3), 
  .out0(out0), 
  .out1(out1),
  .out2(out2), 
  .out3(out3),
  .isBooted(isBooted),
  .error(error),
  .errorCode(errorCode)
);

initial
  begin
    clk = 0;
    forever #10 clk = !clk;
  end
  
initial
  begin
    reset = 0;
    #20 reset = 1;
    #20 reset = 0;
  end

reg [31:0] count = 0;

reg {Tests};

reg testComplete;
  
always @(posedge readInstructionComplete)
  begin
    count <= count + 1;
    case (count)
      {InstructionCompleteCasesText}
      default:
        begin
        end
    endcase
  end

always @(posedge clk)
begin
  
  

end

    
endmodule
