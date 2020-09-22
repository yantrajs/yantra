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