
function getRandomInt(max) {
    return Math.floor(Math.random() * Math.floor(max));
}
assert(getRandomInt(3) in [0, 1, 2] );
assert.strictEqual(getRandomInt(1),0);
var val = Math.random();
assert(0 < val);
assert(1 > val)
