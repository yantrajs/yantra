var clr = require("clr").default;

assert(clr.getClass);

var clrType = clr.getClass("System.Int32");
assert(clrType.toString);