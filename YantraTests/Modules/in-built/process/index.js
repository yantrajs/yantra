var clr = require("clr").default;

var Environment = clr.getClass("System.Environment");
var process = require("process");

assert(process);
assert(process.arch);

assert.strictEqual(Environment.osversion.platform, process.platform);

