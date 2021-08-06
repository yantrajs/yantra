function t() {
    throw new Error("1");
}

function* g1(n) {
    try {
        yield 1;
        t();
    } catch(e) {
        yield 2;
    }
    yield 3;
}

var a = Array.from(g1(1));
assert.strictEqual('1,2,3', a.toString());