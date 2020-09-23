assert(Number.isNaN(Math.floor()));
assert(Number.isNaN(Math.floor(undefined)));
assert(Math.floor(null) === 0);
assert(Math.floor(0) === 0);
assert(Math.floor("") === 0);
assert(Number.isNaN(Math.floor("abcd")));
assert(Math.floor("1.2") === 1);
assert(Math.floor(" 1.2") === 1);