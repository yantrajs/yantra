const array1 = ['a', 'b', 'c'];
const array2 = ['d', 'e', 'f'];
const array3 = array1.concat(array2);

assert.strictEqual(JSON.stringify(array3), '["a","b","c","d","e","f"]');

const a4 = array1.concat("def");

assert.strictEqual(JSON.stringify(a4), '["a","b","c","def"]');