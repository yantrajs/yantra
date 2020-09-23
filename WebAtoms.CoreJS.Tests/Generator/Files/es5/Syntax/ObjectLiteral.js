var a = {
    b: 3
};

assert.strictEqual(a.b, 3);

var c = {
    get c() {
        return "d";
    }
};
assert.strictEqual(c.c, "d");