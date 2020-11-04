//Thu Oct 08 2020 17:29:34 GMT+0530 (India Standard Time)
let day = new Date(1602158374106);
let m = day.getSeconds();


assert.strictEqual(m, 34); 
//"new Date('24 Apr 2010 23:59:57')
day = new Date(1272133797000);
m = day.getSeconds();
assert.strictEqual(m, 57); 

assert.strictEqual(0, day.getSeconds.length);
