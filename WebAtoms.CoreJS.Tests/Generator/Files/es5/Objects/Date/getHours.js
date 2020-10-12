//const birthday = new Date('March 13, 08 04:20');
let birthday = new Date(1205362200000);
let date1 = birthday.getHours();
// Date.prototype.getDay.call(birthday);
assert.strictEqual(date1, 4);

assert.strictEqual(0, birthday.getHours.length);

//new Date('24 Apr 2010 23:59:57')
birthday = new Date(1272133797000)
date1 = birthday.getHours();
assert.strictEqual(date1, 23);


//for console in JS
//a = new Date('March 13, 08 04:20')
//Thu Mar 13 2008 04: 20: 00 GMT + 0530(India Standard Time)
//a.getTime()
//1205362200000