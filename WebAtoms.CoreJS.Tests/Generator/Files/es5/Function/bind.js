function unbound() {
    return this;
}

assert.strictEqual(2, (unbound.bind(2))());

function unboundAdd(a, b) {
    return this + a + b;
}

assert.strictEqual(10, (unboundAdd.bind(2))(3, 5));

assert.strictEqual(10, (unboundAdd.bind(2, 3, 5))());