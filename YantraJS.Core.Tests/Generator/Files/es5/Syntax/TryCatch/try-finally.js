var error = null;
try {
    console.log("a");
} finally {
    error = "a";
}
assert.strictEqual(error, "a");