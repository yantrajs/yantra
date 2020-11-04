import m from "./m";
import { mul, add as Add } from "./m";

assert.strictEqual(" C ", m("C"));
assert.strictEqual(15, mul(5, 3));
assert.strictEqual(10, Add(2, 8));