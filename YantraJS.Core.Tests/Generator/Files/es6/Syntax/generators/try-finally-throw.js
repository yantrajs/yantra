function t() {
    throw new Error("1");
}

let f = 1;

function* g1(n) {
    try {
        yield 1;
        t();
    } finally {
        f++;
    }
}

try {
    var a = Array.from(g1(5));

    assert.strictEqual("1,2", a.toString());
} catch (e) {
}

assert.strictEqual(f, 2);