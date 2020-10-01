assert(Object.is(true, true));
assert(!Object.is(true, false));
assert(!Object.is(false, true));
assert(Object.is(false, false));

assert(!Object.is(false, undefined));
assert(!Object.is(false, null));

assert(!Object.is(true, undefined));
assert(!Object.is(true, null));

assert(Object.is(undefined, undefined));
assert(Object.is(Object.nonExistentProperty, undefined));

assert(Object.is(0.0, 0.0));
assert(!Object.is(0.0, -0.0));
assert(Object.is(-0.0, -0.0));
assert(Object.is(1, 1));
assert(!Object.is(1, 1.0000000000001));

assert(!Object.is("1", 1));
assert(!Object.is(1, "1"));

assert(Object.is("1", "1"));
assert(!Object.is(" 1", "1"));

var a = {};

var b = a;

assert(Object.is(a, b));

assert(!Object.is(a, {}));


assert(!Object.is(Number.POSITIVE_INFINITY, Number.NEGATIVE_INFINITY));
assert(Object.is(Number.POSITIVE_INFINITY, Infinity));

assert(Object.is(Number.NaN, parseFloat("abcd")));