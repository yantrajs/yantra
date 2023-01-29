var a = {
    add(a, b) {
        return a + b;
    }
};

assert.strictEqual(2, a.add?.(1, 1));


var a = {
    add(a, b) {
        return a + b;
    }
};

var _a;
assert.strictEqual(2, (_a = a.add) === null || _a === void 0 ? void 0 : _a.call(this,1,1));