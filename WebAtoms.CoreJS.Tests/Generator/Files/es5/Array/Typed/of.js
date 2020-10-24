// let uint16 = Int16Array.of('10', '20', '30', '40', '50');
let uint16 = Int8Array.of('10', '20', '30', '40', '50');

assert.strictEqual([10, 20, 30, 40, 50].toString(), uint16.toString());
// expected output: Int16Array [10, 20, 30, 40, 50]