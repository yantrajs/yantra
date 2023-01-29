function toArray() {
    return Array.from(arguments);
}

function toArray2() {
    const a = () => Array.from(arguments);
    return a();
}


assert.strictEqual("1,2", toArray(1, 2).toString());
assert.strictEqual("1,2", toArray2(1, 2).toString());