var b = { b: 2 };
b.b += 2;
b.b &&= 1;
assert.strictEqual('{"b":1}', JSON.stringify(b));

b.b = 0;
b.b &&= 1;
assert.strictEqual('{"b":0}', JSON.stringify(b));

let called = false;
function getValue(i) {
    called = true;
    return i;
}

b.b = 0;
b.b &&= getValue(1);
assert.strictEqual(false, called);
assert.strictEqual('{"b":0}', JSON.stringify(b));

b.b = 1;
b.b &&= getValue(1);
assert.strictEqual(true, called);
assert.strictEqual('{"b":1}', JSON.stringify(b));

called = false;
b.b = 0;
b.b ||= getValue(1);
assert.strictEqual(true, called);
assert.strictEqual('{"b":1}', JSON.stringify(b));

called = false;
b.b = 1;
b.b ||= getValue(1);
assert.strictEqual(false, called);
assert.strictEqual('{"b":1}', JSON.stringify(b));