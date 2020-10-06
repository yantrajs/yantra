const elements = ['Fire', 'Air', 'Water'];

assert.strictEqual(elements.join(), "Fire,Air,Water");
assert.strictEqual(elements.join(""), "FireAirWater");
assert.strictEqual(elements.join(";"), "Fire;Air;Water");