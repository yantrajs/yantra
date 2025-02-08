function* g1() {
    yield 1;
    yield* g2();
    yield 5;
}

function* g2() {
    yield 2;
    yield 3;
    yield 4;
}

const a1 = Array.from(g1());
assert.strictEqual("1,2,3,4,5", a1.join(","));