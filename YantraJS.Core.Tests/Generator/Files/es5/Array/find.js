const a = ["a", "b", "c", 2, 3];
const first = a.find((x) => typeof x === "number");
assert.strictEqual(2, first);

class A { }

class B { }

let aa = [{ id: A, value: 1 }, { id: B, value: 2 }];
assert.strictEqual(aa.find((x) => x.id === A).value, 1);
assert.strictEqual(aa.find((x) => x.id === B).value, 2);

class ArrayHelper {
    static remove(a, filter) {
        for (let i = 0; i < a.length; i++) {
            const item = a[i];
            if (filter(item)) {
                a.splice(i, 1);
                return true;
            }
        }
        return false;
    }
}

ArrayHelper.remove(aa, (x) => x.id === ArrayHelper || x.value === ArrayHelper);
assert.strictEqual(2, aa.length);

aa = [{ id: A, value: 1 }, { id: B, value: 2 }];
ArrayHelper.remove(aa, (x) => x.id === A || x.value === A);
assert.strictEqual(1, aa.length);
assert.strictEqual(B, aa[0].id);

aa = [{ id: A, value: 1 }, { id: B, value: 2 }];
ArrayHelper.remove(aa, (x) => x.id === B || x.value === B);
assert.strictEqual(1, aa.length);
assert.strictEqual(A, aa[0].id);