
var { mul, default: a, log } = require("./a");
assert.strictEqual("A B", a("A", "B"));
assert.strictEqual(12, mul(3, 4));

log("a");
