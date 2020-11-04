let event = new Date('August 19, 1975 23:15:30');
n = event.toString();
console.log(n);
assert.strictEqual("Tue Aug 19 1975 23:15:30 GMT+0530 (India Standard Time)",event.toString());