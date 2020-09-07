var fx = new Function("a", "b", "return a + b;");
assert(fx(1, 2) === 3, fx(1, 2));
