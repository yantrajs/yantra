var clr = require("clr").default;

assert(clr.getClass);

var clrType = clr.getClass("System.Int32");
assert(clrType.prototype.getTypeCode);
assert(clrType.tryParse);

var Random = clr.getClass("System.Random");

var r = new Random();
var n = r.next();

assert(typeof n === "number");

var TimeZoneInfo = clr.getClass("System.TimeZoneInfo");

var local = TimeZoneInfo.local;

assert(local instanceof TimeZoneInfo);
assert(!(local instanceof Random));

assert.strictEqual(1, clr.temp1);

clr.temp1 = 3;
assert.strictEqual(3, clr.temp1);
