`include "GlobalDefines.v"

// Quartus II Verilog Template
// Simple Dual Port RAM with separate read/write addresses and
// single read/write clock

module simple_dual_port_ram_single_clock
#(parameter DATA_WIDTH=40, parameter ADDR_WIDTH=32)
(
   input [(DATA_WIDTH-1):0] data,
   input [(ADDR_WIDTH-1):0] read_addr, write_addr,
   input we, re, clk,
   output reg [(DATA_WIDTH-1):0] q
);

   // Declare the RAM variable
   reg [DATA_WIDTH-1:0] ram[`RamSize-1:0];

   always @ (posedge clk)
   begin
      // Write
      if (we)
         ram[write_addr] <= data;

      // Read (if read_addr == write_addr, return OLD data).   To return
      // NEW data, use = (blocking write) rather than <= (non-blocking write)
      // in the write assignment.    NOTE: NEW data may require extra bypass
      // logic around the RAM.
      //if (re)
        q <= ram[read_addr];
   end

endmodule
