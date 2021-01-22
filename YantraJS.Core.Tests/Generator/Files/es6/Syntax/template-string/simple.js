var b = { name: "Test", email: "test@test.com" };
var a = `My email is ${b.name}<${b.email}>`;
assert.strictEqual("My email is Test<test@test.com>", a);

var c = `\\a`;
assert.strictEqual("\\a", c);