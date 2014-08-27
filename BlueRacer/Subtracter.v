module Subtracter #(parameter Size = 32)(
  input wire clk,
  input wire reset,
  input wire [Size-1:0] a,
  input wire [Size-1:0] b,
  output wire [Size-1:0] c
);
  assign c = a - b;
endmodule