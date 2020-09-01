var target = { a: 1, b: 2 };
var source = { b: 4, c: 5 };

var returnedTarget = Object.assign(target, source);

assert(returnedTarget.a === 1);
assert(returnedTarget.b === 4);
assert(returnedTarget.c === 5);
