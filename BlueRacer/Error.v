`define SetError(__code)        \
begin                           \
  _error <= 1;                  \
  _errorCode <= __code;         \
end                             \
