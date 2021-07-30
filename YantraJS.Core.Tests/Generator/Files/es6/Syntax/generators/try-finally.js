
let a = 1;

function* g1(n) {
    try {
        yield 1;
        return n + 1;
    } finally {
        a++;
    }
}

var b = Array.from(g1());

assert.strictEqual(a, 2);