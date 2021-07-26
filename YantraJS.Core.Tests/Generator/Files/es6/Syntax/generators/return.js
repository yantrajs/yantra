function* g() {
    yield 1;
    yield 2;
    return yield 3;
}

var a = Array.from(g());
assert.strictEqual(a.toString(), "1,2,3");