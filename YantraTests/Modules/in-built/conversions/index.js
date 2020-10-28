var clr = require("clr").default;

var IntArray = clr.getClass("System.Int32[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

var SystemArray = clr.getClass("System.Array");

var a = new IntArray(10);

a[0] = 1;
a[1] = 2;

assert.strictEqual(1, a[0]);
assert.strictEqual(2, a[1]);

console.log(Object.keys(IntArray.prototype));
console.log(Object.keys(SystemArray.prototype));
assert(a instanceof SystemArray);
// assert(IntArray.prototype.length);

// length should be 10...
assert.strictEqual(10, a.length);

var List = clr.getClass("System.Collections.Generic.List`1, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
var String = clr.getClass("System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
var ListOfString = List.makeGenericType(String);

var a2 = new ListOfString(2);
a2.add("akash");
a2.add("kava");

assert.strictEqual("akash", a2[0]);
assert.strictEqual("kava", a2[1]);

ListOfString = clr.getClass("System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

a2 = new ListOfString(2);
a2.add("akash");
a2.add("kava");

assert.strictEqual("akash", a2[0]);
assert.strictEqual("kava", a2[1]);

const Int32 = clr.getClass("System.Int32");

ListOfStringCapacity = ListOfString.getConstructor(Int32);

a2 = new ListOfStringCapacity(2);
assert.strictEqual(2, a2.capacity);

assert(a2 instanceof ListOfString);
