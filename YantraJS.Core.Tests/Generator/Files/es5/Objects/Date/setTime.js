//let date = new Date(1970, 1, 1, 0, 0, 0);
// Sun Feb 01 1970 00:00:00 GMT+0530 (India Standard Time)
//  2658600000 =  date.getTime()
date = new Date(2658600000);
//date.setTime(0);
assert.strictEqual(0, date.setTime(0));

assert(isNaN(date.setTime(NaN)));

date = new Date(2658600000);

assert.strictEqual(1, date.setTime(1.123456));
assert.strictEqual(1, date.setTime(1.8));
assert.strictEqual(-1, date.setTime(-1.123456));
assert.strictEqual(-1, date.setTime(-1.8));
assert(isNaN(date.setTime(9e15)));
date = new Date(2658600000);
assert(isNaN(date.setTime(Infinity)));
date = new Date(2658600000);
assert.strictEqual(Number.POSITIVE_INFINITY, 1 / date.setTime(-0));
assert.strictEqual(1, date.setTime.length);
