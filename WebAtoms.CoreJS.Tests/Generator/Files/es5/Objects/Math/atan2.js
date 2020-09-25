function calcAngleDegrees(x, y) {
    return Math.atan2(y, x) * 180 / Math.PI;
}
assert(Number.isNaN(Math.atan2()));
assert(Number.isNaN(Math.atan2(undefined)));
assert(isNaN(Math.atan2(null)));
assert(isNaN(Math.atan2(0)));
assert(isNaN(Math.atan2("")));
assert(isNaN(Math.atan2("abcd")));
assert(isNaN(Math.atan2("1.2")));
assert(isNaN(Math.atan2(" 1.2")));
assert.strictEqual(calcAngleDegrees(5, 5), 45);
assert.strictEqual(calcAngleDegrees(10, 10), 45);
assert.strictEqual(calcAngleDegrees(0, 10), 90);