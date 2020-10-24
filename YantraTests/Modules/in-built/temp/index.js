const temp = require("temp");

const t = temp.print({ name: "Akash", age: 40 });
assert.strictEqual("Name is Akash and age is 40", t);