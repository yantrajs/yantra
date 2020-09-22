var p = JSON.parse("\"a\"");

assert(p === "a");

p = JSON.parse("null");

assert(p === null);

p = JSON.parse("{ \"a\": 1 }");

assert(p.a === 1);

p = JSON.parse("{ \"a\": 1, \"b\": [1, 2] }");

assert(p.a === 1);

assert(p.b[0] === 1);
assert(p.b[1] === 2);

p = JSON.parse('{ "a": 1, "b": { "c": [ {  }, { "c1": "c1" }, 5 ] } }');

assert(p.a === 1);
var b = p.b;
var ary = b.c;
assert(ary.length === 3);

var keys = Object.keys(ary[0]);
assert(keys.length === 0);

var ary1 = ary[1];
assert(ary1.c1 === "c1");