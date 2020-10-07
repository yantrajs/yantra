//const birthday = new Date('August 19, 1975 23:15:30');
let birthday = new Date(1602060788968);
let date1 = birthday.getDay();
// Date.prototype.getDay.call(birthday);
assert.strictEqual(date1, 3);

assert.strictEqual(0, birthday.getDay.length);
