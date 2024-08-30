var p = new Proxy({}, {
    get(t, p, receiver) {
        return p + "." + p;
    }
});

assert.strictEqual(p.a, "a.a");
