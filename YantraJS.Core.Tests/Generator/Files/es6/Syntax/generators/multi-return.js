function* g(i) {
    if (i === 4)
        throw new Error('error');
    if(i)
        return yield 2;
    var a = new RegExp();
    return yield 3;
}

var a = Array.from(g());
assert.strictEqual(a.toString(), '3');

a = Array.from(g(1));
assert.strictEqual(a.toString(), '2');