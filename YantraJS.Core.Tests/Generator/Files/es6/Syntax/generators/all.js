function* f(a, b, c) {
    //if (a) {
    //    return a;
    //}
    //if (b) {
    //    yield 1;
    //    return b;
    //}
    const r = this.get(c);
    yield r.b;
    return null;
}

const target = {
    get(n) {
        return this;
    },
    b: "b"
};

const r = f.call(target);
assert.strictEqual("b", r.next().value);
