
var { mul, default: a } = require("./a");
assert.strictEqual("A B", a("A", "B"));
assert.strictEqual(12, mul(3, 4));