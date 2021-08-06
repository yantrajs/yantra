function* g1(n) {
    try {
        yield 1;
    } catch(e) {
        yield e;
    }
}

var g = g1(1);
var a = g.next();
assert.strictEqual(1, a.value);
g.throw(new Error('2'));
a = g.next();
assert.strictEqual('Error: 2', a.value.toString());
