let event = new Date('August 19, 1975 23:15:30');

event.setFullYear(1969);

assert.strictEqual(1969, event.getFullYear());
// expected output: 1969

event.setFullYear(1);
//event.setFullYear(0); //edge case, cannot be tested in .net

assert.strictEqual(1, event.getFullYear());
// expected output: 1

assert(isNaN(event.setFullYear(NaN)));
assert.strictEqual(3, event.setFullYear.length);