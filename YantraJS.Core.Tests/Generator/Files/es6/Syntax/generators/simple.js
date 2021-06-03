//function* g1() {
//    yield 0;
//    yield 1;
//    yield 2;
//}

//let g = g1();
//let c = g.next();
//assert.strictEqual(0, c.value);
//assert.strictEqual(false, c.done);
//c = g.next();
//assert.strictEqual(1, c.value);
//assert.strictEqual(false, c.done);
//c = g.next();
//assert.strictEqual(2, c.value);
//assert.strictEqual(false, c.done);

//c = g.next();
//assert.strictEqual(undefined, c.value);
//assert.strictEqual(true, c.done);

function* g1(n) {
    for (var i = 0; i < n; i++) {
        yield i;
    }
}

var a = Array.from(g1(5));
assert.strictEqual('0,1,2,3,4', a.toString());