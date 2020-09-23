var p = JSON.parse("\"a\"");

assert.strictEqual(p, "a");

p = JSON.parse("null");

assert.strictEqual(p, null);

p = JSON.parse("{ \"a\": 1 }");

assert.strictEqual(p.a, 1);

p = JSON.parse("{ \"a\": 1, \"b\": [1, 2] }");

assert.strictEqual(p.a, 1);

assert.strictEqual(p.b[0], 1);
assert.strictEqual(p.b[1], 2);

p = JSON.parse('{ "a": 1, "b": { "c": [ {  }, { "c1": "c1" }, 5 ] } }');

assert.strictEqual(p.a, 1);
var b = p.b;
var ary = b.c;
assert.strictEqual(ary.length, 3);

var keys = Object.keys(ary[0]);
assert.strictEqual(keys.length, 0);

var ary1 = ary[1];
assert.strictEqual(ary1.c1, "c1");