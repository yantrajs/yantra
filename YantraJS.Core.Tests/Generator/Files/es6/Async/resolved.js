function resolved() {
    return Promise.resolve(1);
}

async function awaiter() {
    await resolved();
    return 4;
}

awaiter().then((n) => {
    assert.strictEqual(4, n);
});