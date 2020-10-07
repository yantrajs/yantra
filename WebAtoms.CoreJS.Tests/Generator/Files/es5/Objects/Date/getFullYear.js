//let a = new Date('24 Apr 2010 23:59:57')
let a = new Date(1272133797000);
//let date = a.getDate();
let year = a.getFullYear();
assert.strictEqual(year, 2010);
