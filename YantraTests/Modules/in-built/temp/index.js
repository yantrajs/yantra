const temp = require("temp");

const t = temp.print({ name: "Akash", age: 40 });
assert.strictEqual("Name is Akash and age is 40", t);


var clr = require("clr").default;

var ClrString = clr.getClass("System.String");

var m = temp.getMethod("add", ClrString, ClrString);

assert.strictEqual("a b", m.call(temp, "a", "b"));

m = temp.getMethod("add", ClrString, ClrString, ClrString);
assert.strictEqual("a b c", m.call(temp, "a", "b", "c"));