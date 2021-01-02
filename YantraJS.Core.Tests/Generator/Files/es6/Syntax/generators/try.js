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
}

var a = [];
for (var i of g1(5)) {
    a.push(i);
}

assert.strictEqual("1,2", a.toString());