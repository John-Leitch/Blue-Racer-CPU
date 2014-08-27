module ByteToBits(
  input wire [7:0] byte, 
  output wire bit0,
  output wire bit1,
  output wire bit2,
  output wire bit3,
  output wire bit4,
  output wire bit5,
  output wire bit6,
  output wire bit7);
  assign bit0 = byte[0];
  assign bit1 = byte[1];
  assign bit2 = byte[2];
  assign bit3 = byte[3];
  assign bit4 = byte[4];
  assign bit5 = byte[5];
  assign bit6 = byte[6];
  assign bit7 = byte[7];
endmodule
