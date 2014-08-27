module LcdShiftRegister (
  input wire clk,
  input wire reset,
  input wire [7:0] in,
  output reg [127:0] out = 0
);

reg [7:0] index = 0;

`define OutByte(__ofs) out[((__ofs + 1)*8)-1:__ofs*8]
always @(posedge clk or posedge reset)
  if (reset)
    begin
      out <= 0;
      index <= 0;
      //00000000deadbeef
      //out <= { out[119:0], in };

    end
  else
    begin
      case (index)
        0: `OutByte(0) <= in;
        1: `OutByte(1) <= in;
        2: `OutByte(2) <= in;
        3: `OutByte(3) <= in;
        4: `OutByte(4) <= in;
        5: `OutByte(5) <= in;
        6: `OutByte(6) <= in;
        7: `OutByte(7) <= in;
        8: `OutByte(8) <= in;
        9: `OutByte(9) <= in;
        10: `OutByte(10) <= in;
        11: `OutByte(11) <= in;
        12: `OutByte(12) <= in;
        13: `OutByte(13) <= in;
        14: `OutByte(14) <= in;
        15: `OutByte(15) <= in;        
      endcase
      if (index <= 15) index <= index + 1;
      else index <= 0;
    end

endmodule
