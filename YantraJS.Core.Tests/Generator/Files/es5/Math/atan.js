﻿assert(Number.isNaN(Math.atan()));
assert(Number.isNaN(Math.atan(undefined)));
assert.strictEqual(Math.atan(null), 0);
assert.strictEqual(Math.atan(0), 0);
assert.strictEqual(Math.atan(""), 0);
assert(isNaN(Math.atan("abcd")));
assert.strictEqual(Math.atan("1.2"), 0.8760580505981934);
assert.strictEqual(Math.atan(" 1.2"), 0.8760580505981934);
assert.strictEqual(Math.atan(8 / 10), 0.6747409422235527);
assert.strictEqual(Math.atan(5 / 3), 1.0303768265243125);
