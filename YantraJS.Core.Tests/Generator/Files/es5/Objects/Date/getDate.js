//const birthday = new Date('August 19, 1975 23:15:30');
let birthday = new Date(1602060788968);
let date1 = birthday.getDate();
// Date.prototype.getDate.call(birthday);
assert.strictEqual(date1, 7);

birthday = new Date('August 19, 1975 23:15:30');
date1 = birthday.getDate();
assert.strictEqual(date1, 19);

assert.strictEqual(0, birthday.getDate.length);

birthday = new Date("adsewses");
date1 = birthday.getDate();
// console.log(date1);
assert(isNaN(date1));
