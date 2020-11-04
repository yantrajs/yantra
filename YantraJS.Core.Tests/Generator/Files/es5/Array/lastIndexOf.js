const elements = [1,2,1,2,1,1,2,2,1,2];

const i = elements.lastIndexOf(1);
assert.strictEqual(elements.length - 2, i);
assert.strictEqual(elements.lastIndexOf(4), -1);
assert.strictEqual([].lastIndexOf(4), -1);
assert.strictEqual([1].lastIndexOf(4), -1);