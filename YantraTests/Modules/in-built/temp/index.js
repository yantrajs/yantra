const temp = require("temp");

const t = temp.print({ name: "Akash", age: 40 });
assert.strictEqual("Name is Akash and age is 40", t);


var clr = require("clr").default;

var String = clr.getClass("System.String");

var m = temp.getMethod("add", String, String);

assert.strictEqual("a b", m.call(temp, "a", "b"));

m = temp.getMethod("add", String, String, String);
assert.strictEqual("a b c", m.call(temp, "a", "b", "c"));