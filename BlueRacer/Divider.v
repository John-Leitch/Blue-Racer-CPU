module Divider #(parameter Size = 32)(
  input wire clk,
  input wire reset,
  input wire [Size-1:0] a,
  input wire [Size-1:0] b,
  output wire [Size-1:0] c,
  output wire [Size-1:0] remainder
);
  //assign c = a / b;
lpm_divide #(
  .lpm_widthn(32), 
  .lpm_widthd(32), 
  .lpm_nrepresentation("UNSIGNED"), 
  .lpm_drepresentation("UNSIGNED"),
  .lpm_pipeline(`DivideLatency)
) divider (
  .clock(clk),
  .clken(1),
  //.aclr(~reset),
  .numer(a),
  .denom(b),
  .quotient(c),
  .remain(remainder)
);
  
endmodule