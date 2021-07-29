function evaluate() {
    var error = null;
    try {
        if (error)
            return "a";
        try {
            console.log("a");
        } catch (e) {
            console.warn(e);
        }
    } finally {
        error = "a";
    }
    assert.strictEqual(error, "a");
}

evaluate();