function t() {
    
}

function* g1(n) {
    try {
        yield 1;
        t();
    } catch(e) {
        yield 2;
    }
}

var a = Array.from(g1(0));

assert.strictEqual('1', a.toString());