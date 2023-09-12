function sleep(n) {
    return new Promise((resolve) => {
        setTimeout(resolve, n);
    });
}

class File {

    lines = [];

    constructor() {
        this.open = true;
    }

    add(line) {
        this.lines.push(line);
    }

    async disposeAsync() {
        await sleep(1);
        this.open = false;
    }

    [Symbol.asyncDispose]() {
        return this.disposeAsync();
    }
}

async function runTest(f) {
    await using file = f;
    f.add("1");
    f.add("2");
}

var f1 = new File();
assert.strictEqual(true, f1.open);
await runTest(f1);
assert.strictEqual(false, f1.open);
assert.strictEqual("1,2", f1.lines.toString());


class CorruptFile extends File {
    [Symbol.asyncDispose]() {
        throw new Error("File corrupt");
    }
}

var f2 = new CorruptFile();
assert.strictEqual(true, f2.open);
try {
    await runTest(f2);
} catch (error) {
    console.log(error);
    console.log(Object.getPrototypeOf(error).constructor.name);
    assert.strictEqual(true, error instanceof SuppressedError);
}