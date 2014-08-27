`define OpcodeAssignCase(__opcode, __register)  \
__opcode:                                       \
  begin                                         \
    __register <= `Operand32;                   \
    AdvancePC();                                \
  end                                           \

`define OpcodeEqCase(__opcode, __register)  \
__opcode:                                   \
  begin                                     \
    _isEq <= __register == `Operand32;      \
    AdvancePC();                            \
  end                                       \

`define OpcodeNotEqCase(__opcode, __register) \
__opcode:                                     \
  begin                                       \
    _isEq <= __register != `Operand32;        \
    AdvancePC();                              \
  end                                         \
  
`define OpcodeLtCase(__opcode, __register)  \
__opcode:                                   \
  begin                                     \
    _isEq <= __register < `Operand32;       \
    AdvancePC();                            \
  end                                       \
  
`define OpcodeLtEqCase(__opcode, __register)  \
__opcode:                                     \
  begin                                       \
    _isEq <= __register <= `Operand32;        \
    AdvancePC();                              \
  end                                         \
  
`define OpcodeGtCase(__opcode, __register)  \
__opcode:                                   \
  begin                                     \
    _isEq <= __register > `Operand32;       \
    AdvancePC();                            \
  end                                       \
  
`define OpcodeGtEqCase(__opcode, __register)  \
__opcode:                                     \
  begin                                       \
    _isEq <= __register >= `Operand32;        \
    AdvancePC();                              \
  end                                         \
  