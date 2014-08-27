module MemoryController #(parameter DATA_WIDTH=8, parameter ADDR_WIDTH=32)(
  input wire clk,
  input wire reset,  
  
  input wire read1,
  input wire [ADDR_WIDTH-1:0] read1Address,
  output reg [DATA_WIDTH-1:0] read1Buffer = 0,
  output reg read1Active = 0,
  output reg read1Complete = 0,
  
  input wire read2,
  input wire [ADDR_WIDTH-1:0] read2Address,
  output reg [DATA_WIDTH*2-1:0] read2Buffer = 0,
  output reg read2Active = 0,
  output reg read2Complete = 0,
  
  input wire read4,
  input wire [ADDR_WIDTH-1:0] read4Address,
  output reg [DATA_WIDTH*4-1:0] read4Buffer = 0,
  output reg read4Active = 0,
  output reg read4Starting = 0,
  output reg read4Completing = 0,
  output reg read4Complete = 0,
  
  input wire read5,
  input wire [ADDR_WIDTH-1:0] read5Address,
  output reg [DATA_WIDTH*5-1:0] read5Buffer = 0,
  output reg read5Active = 0,
  output reg read5Starting = 0,
  output reg read5Completing = 0,
  output reg read5Complete = 0,
  
  input wire write1,
  input wire [ADDR_WIDTH-1:0] write1Address,
  input wire [DATA_WIDTH-1:0] write1Buffer,
  output reg write1Active = 0,
  output reg write1Complete = 0,
  output reg write1Completing = 0,
  
  input wire write4,
  input wire [ADDR_WIDTH-1:0] write4Address,  
  input wire [DATA_WIDTH*4-1:0] write4Buffer,
  output reg write4Active = 0,
  output reg write4Complete = 0,
  output reg write4Completing = 0,
  
  input wire write5,
  input wire [ADDR_WIDTH-1:0] write5Address,  
  input wire [DATA_WIDTH*5-1:0] write5Buffer,
  output reg write5Active = 0,
  output reg write5Complete = 0,
  output reg write5Completing = 0
);

reg _readEnable = 0, _writeEnable = 0;
reg [ADDR_WIDTH-1:0] _readAddressInternal = 0, _writeAddressInternal = 0;
reg [DATA_WIDTH-1:0] _dataIn = 0;
wire [DATA_WIDTH-1:0] _dataOut;

simple_dual_port_ram_single_clock #(.DATA_WIDTH(DATA_WIDTH), .ADDR_WIDTH(ADDR_WIDTH)) memory (
  .clk(clk),
  .read_addr(_readAddressInternal),  
  .re(_readEnable),
  .q(_dataOut),
  .write_addr(_writeAddressInternal),
  .data(_dataIn),
  .we(_writeEnable)
);

reg [1:0] _read1State = 0;
reg [1:0] _read2State = 0;
reg [2:0] _read4State = 0;
reg [3:0] _read5State = 0;
reg [1:0] _write1State = 0;
reg [3:0] _write4State = 0;
reg [3:0] _write5State = 0;

task Read;
	begin
	 if (read1 & _read1State == 0 & ~read2Active & ~read4Active & ~read5Active)
       begin
         _readAddressInternal <= read1Address;
         _readEnable <= 1;
         _read1State <= 1;
         read1Active <= 1;
       end
	 else if (read4 & _read4State == 0 & ~read1Active & ~read2Active & ~read5Active)
       begin
         _readAddressInternal <= read4Address;
         _readEnable <= 1;
         _read4State <= 1;
         read4Active <= 1;
         read4Starting <= 0;
         read4Complete <= 0;
         read4Completing <= 0;
       end
	 else if (read5 & _read5State == 0 & ~read1Active & ~read2Active & ~read4Active)
       begin
         _readAddressInternal <= read5Address;
         _readEnable <= 1;
         _read5State <= 1;
         read5Active <= 1;
         read5Starting <= 0;
         read5Complete <= 0;
         read5Completing <= 0;
       end
	end
endtask

task Read1;
  begin

/*
       
     if (_read1State == 1) //  Wait for data
       begin           
         _read1State <= 2;
		 _readAddressInternal <= read1Address;
       end
       
     if (_read1State == 2)
       begin
         read1Buffer[7:0] <= _dataOut;
         read1Complete <= 1;
         
         //if (read1 & ~read2Active & ~read4Active)
		 if (read1 & ~read2 & ~read4)
            begin
               _read1State <= 2;
               _readAddressInternal <= read1Address;
            end
         else
            begin
              read1Active <= 0;
               _read1State <= 0;
            end
       end
    else
      begin
        read1Complete <= 0;
      end
     */ 
     
  end
endtask

task Read2;
  begin

     if (read2 & _read2State == 0)
       begin
         _readAddressInternal <= read2Address;
         _readEnable <= 1;
         _read2State <= 1;
       end
       
     if (_read2State == 1) //  Wait for data
       begin           
         _read2State <= 2;
         _readAddressInternal <= _readAddressInternal + 1;
       end
       
     if (_read2State == 2)
       begin
         read2Buffer[15:8] <= _dataOut;
         _read2State <= 3;
         _readAddressInternal <= read2Address;
       end
       
   if (_read2State == 3)
       begin
         read2Buffer[7:0] <= _dataOut;
         
         if (read2 & ~read1Active & ~read4Active)
            begin
               _read2State <= 2;
               _readAddressInternal <= _readAddressInternal + 1;
            end
         else
            begin
               _read2State <= 0;
            end
       end  
     
  end
endtask

task Read4;
  begin
  
     if (_read4State == 1) //  Wait for data
       begin        
        read4Starting <= 0;
         _read4State <= 2;
         _readAddressInternal <= _readAddressInternal + 1;
       end       
     else if (_read4State == 2)
       begin
        read4Starting <= 1;
         read4Buffer[31:24] <= _dataOut;
         _read4State <= 3;
         _readAddressInternal <= _readAddressInternal + 1;
         read4Complete <= 0;
       end
     else if (_read4State == 3)
       begin
        read4Starting <= 0;
         read4Buffer[23:16] <= _dataOut;
         _read4State <= 4;
         _readAddressInternal <= _readAddressInternal + 1;
       end
     else if (_read4State == 4)
       begin
         read4Buffer[15:8] <= _dataOut;
         _read4State <= 5;
         _readAddressInternal <= read4Address;
         read4Completing <= 1;
       end
     else if (_read4State == 5)
       begin
         read4Buffer[7:0] <= _dataOut;
         read4Complete <= 1;
         read4Completing <= 0;
         
         //if (read4 & ~read1Active & ~read2Active)
         if (read4 & ~read1 & ~read2 & ~read5)
            begin
               _read4State <= 2;
               _readAddressInternal <= _readAddressInternal + 1;
            end
         else
            begin
              read4Active <= 0;
               _read4State <= 0;
            end
      end
    else
      begin
        read4Complete <= 0;
      end
      
     
  end
endtask

task Read5;
  begin
  
     if (_read5State == 1) //  Wait for data
       begin
         read5Starting <= 0;
         _read5State <= 2;
         _readAddressInternal <= _readAddressInternal + 1;
       end
     else if (_read5State == 2)
       begin
         read5Starting <= 1;
         read5Complete <= 0;
         read5Buffer[39:32] <= _dataOut;
         _read5State <= 3;
         _readAddressInternal <= _readAddressInternal + 1;
       end
     else if (_read5State == 3)
       begin
         read5Starting <= 0;
         read5Buffer[31:24] <= _dataOut;
         _read5State <= 4;
         _readAddressInternal <= _readAddressInternal + 1;
       end
     else if (_read5State == 4)
       begin
         read5Buffer[23:16] <= _dataOut;
         _read5State <= 5;
         _readAddressInternal <= _readAddressInternal + 1;
       end
     else if (_read5State == 5)
       begin
         read5Buffer[15:8] <= _dataOut;
         _read5State <= 6;
         _readAddressInternal <= read5Address;
         read5Completing <= 1;
       end
     else if (_read5State == 6)
       begin
         read5Buffer[7:0] <= _dataOut;
         read5Complete <= 1;
         read5Completing <= 0;
         
         if (read5 & ~read1 & ~read2 & ~read4)
          begin
            _read5State <= 2;
            _readAddressInternal <= _readAddressInternal + 1;
          end
         else
          begin
            read5Active <= 0;
            _read5State <= 0;
            _readEnable <= 0;
          end
       end
    else
      begin
        read5Complete <= 0;
      end
      
     
  end
endtask

task Write;
  begin
    if (write1 && _write1State == 0 && ~write4Active && ~write5Active)
      begin
        _dataIn <= write1Buffer;
        _writeAddressInternal <= write1Address;
        _writeEnable <= 1;
        _write1State <= 1;
        write1Active <= 1;
        write1Complete <= 0;
        write1Completing <= 1;
      end
    else if (write4 && _write4State == 0 && ~write1Active && ~write5Active)
      begin
        _dataIn <= write4Buffer[31:24];
        _writeAddressInternal <= write4Address;
        _writeEnable <= 1;
        _write4State <= 1;
        write4Active <= 1;
        write4Complete <= 0;
        write4Completing <= 0;
      end
    else if (write5 && _write5State == 0 && ~write1Active && ~write4Active)
      begin
        _dataIn <= write5Buffer[39:32];
        _writeAddressInternal <= write5Address;
        _writeEnable <= 1;
        _write5State <= 1;
        write5Active <= 1;
        write5Complete <= 0;
        write5Completing <= 0;
      end
  end
endtask

task Write1;
  begin
    if (_write1State == 1)
      begin
        _writeAddressInternal <= write1Address;
        _dataIn <= write1Buffer;
        write1Complete <= 1;
        write1Completing <= 0;
        
        if (write1)
          begin
            _write1State <= 1;
          end
        else
          begin
            write1Active <= 0;
            _write1State <= 2;
          end
      end
    else if (_write5State == 2)
      begin
        _write1State <= 0;
        write1Complete <= 0;
      end
  end
endtask

task Write4;
  begin  
    if (_write4State == 1)
      begin
        _dataIn <= write4Buffer[23:16];
        _write4State <= 2;
        _writeAddressInternal <= _writeAddressInternal + 1;
      end
    else if (_write4State == 2)
      begin
        _dataIn <= write4Buffer[15:8];
        _write4State <= 3;
        _writeAddressInternal <= _writeAddressInternal + 1;
        write4Completing <= 1;
      end
    else if (_write4State == 3)
      begin
        _writeAddressInternal <= _writeAddressInternal + 1;
        _dataIn <= write4Buffer[7:0];
        write4Complete <= 1;
        write4Completing <= 0;
        
        if (write4)
          begin
            _write4State <= 0;
          end
        else
          begin
            write4Active <= 0;
            _write4State <= 4;
          end
      end
    else if (_write4State == 4)
      begin
        _write4State <= 0;
        write4Complete <= 0;
      end
  end
endtask

task Write5;
  begin  
    if (_write5State == 1)
      begin
        _dataIn <= write5Buffer[31:24];
        _write5State <= 2;
        _writeAddressInternal <= _writeAddressInternal + 1;
      end
    else if (_write5State == 2)
      begin
        _dataIn <= write5Buffer[23:16];
        _write5State <= 3;
        _writeAddressInternal <= _writeAddressInternal + 1;
      end
    else if (_write5State == 3)
      begin
        _dataIn <= write5Buffer[15:8];
        _write5State <= 4;
        _writeAddressInternal <= _writeAddressInternal + 1;
        write5Completing <= 1;
      end
    else if (_write5State == 4)
      begin
        _writeAddressInternal <= _writeAddressInternal + 1;
        _dataIn <= write5Buffer[7:0];
        write5Complete <= 1;
        write5Completing <= 0;
        if (write5)
          begin
            _write5State <= 0;
          end
        else
          begin
            write5Active <= 0;
            _write5State <= 5;
          end
      end
    else if (_write5State == 5)
      begin
        _write5State <= 0;
        write5Complete <= 0;
      end
  end
endtask

always @(posedge clk or posedge reset)
  begin
    if (reset)
      begin
        _readAddressInternal <= 0;
        _read1State <= 0;
        read1Buffer <= 0;
        read1Active <= 0;
        read1Complete <= 0;

        _readAddressInternal <= 0;
        _read2State <= 0;
        read2Buffer <= 0;
        read2Active <= 0;
        read2Complete <= 0;

        _readAddressInternal <= 0;
        _read4State <= 0;
        read4Buffer <= 0;
        read4Active <= 0;
        read4Complete <= 0;

        _writeEnable <= 0;
        _writeAddressInternal <= 0;
        _write5State <= 0;
        write5Active <= 0;
        write5Complete <= 0;
        write5Completing <= 0;        

        _dataIn <= 0;
      end
    else
      begin
        Read();
        Read1();
        Read2();
        Read4();
        Read5();
        
        Write();
        Write1();
        Write4();
        Write5();
      end
  end

endmodule