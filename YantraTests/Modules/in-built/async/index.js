var test = require("./test").default;

async function testFunction() {
    var r = await test.base32(5454);
    assert.strictEqual("5AE", r);
}

testFunction().then((r) => {
    // do nothing...
});