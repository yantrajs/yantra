//Thu Oct 08 2020 17:29:34 GMT+0530 (India Standard Time)
let day = new Date(1602158374106);
let mins = day.getMinutes();

// Date.prototype.getDay.call(birthday);
assert.strictEqual(mins, 29);

assert.strictEqual(0, day.getMinutes.length);

//"new Date('24 Apr 2010 23:59:57')
day = new Date(1272133797000);
mins = day.getMinutes();
assert.strictEqual(mins, 59);
