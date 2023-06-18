var a = { name: "name", email: "name@email.com" };

var { name, email } = a;

assert.strictEqual("name", name);
assert.strictEqual("name@email.com", email);

var b = { a };

var { a: { name: n1, email: e1 } } = b;

assert.strictEqual("name", n1);
assert.strictEqual("name@email.com", e1);

var { "b": b2 } = { b: 1 };
assert.strictEqual(b2, 1);

