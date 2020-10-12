var a = new Date(1602066395459);
a.setDate(-1);
assert.strictEqual(29, a.getDate());
assert.strictEqual(1, a.setDate.length);
var val = a.setDate(undefined);
assert(isNaN(val));
assert(isNaN(a.getDate()));

