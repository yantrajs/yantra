var clr = require("clr").default;

var Environment = clr.getClass("System.Environment");
var process = require("process");

assert(process);
assert(process.arch);

assert.strictEqual(Environment.osversion.platform, process.platform);

var url = require("url");

var u = new url("https://yantrajs.com");
assert.strictEqual("yantrajs.com", u.host);