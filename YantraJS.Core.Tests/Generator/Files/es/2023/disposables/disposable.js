class File {

    lines = [];

    constructor() {
        this.open = true;
    }

    add(line) {
        if (this.open) {
            this.lines.push(line);
            return;
        }
        throw new Error("File is disposed");
    }

    [Symbol.dispose]() {
        this.open = false;
    }
}

function runTest(f) {
    using file = f;
    f.add("1");
    f.add("2");
}

var f1 = new File();
assert.strictEqual(true, f1.open);
runTest(f1);
assert.strictEqual(false, f1.open);
assert.strictEqual("12", f1.lines.toString());