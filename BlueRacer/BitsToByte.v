module BitsToByte(
  input wire bit0,
  input wire bit1,
  input wire bit2,
  input wire bit3,
  input wire bit4,
  input wire bit5,
  input wire bit6,
  input wire bit7,
  output wire [7:0] byte);
  assign byte = { bit7, bit6, bit5, bit4, bit3, bit2, bit1, bit0 };
endmodule
