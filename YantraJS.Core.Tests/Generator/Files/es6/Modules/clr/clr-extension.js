var clr = require("clr").default;

var List = clr.getClass("System.Collections.Generic.List`1, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
var ClrString = clr.getClass("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
var ListOfString = List.makeGenericType(ClrString);

var a2 = new ListOfString(2);
a2.add("akash");
a2.add("kava");

assert.strictEqual("akash", a2[0]);
assert.strictEqual("kava", a2[1]);

assert.strictEqual(2, a2.count);

a2["a"] = "b";
assert.strictEqual("b", a2["a"]);
