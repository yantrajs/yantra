assert.strictEqual("44", [1, , 3, 4].reduce(((pv, c) => pv + c + "")));
assert.strictEqual(8, [1, , 3, 4].reduce(((pv, c) => pv + c)));
assert.strictEqual("NaN34", [1, undefined , 3, 4].reduce(((pv, c) => pv + c + "")));