const elements = [1, 2, 3, 4];

var map = elements.map((e, i) => i ? e * i : undefined);

let a = [1, , undefined, 3];
assert.strictEqual("1;,,undefined;,3;", a.map(x => x + ";").join(","));