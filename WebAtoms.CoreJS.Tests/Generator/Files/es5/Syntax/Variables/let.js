var a = "a";

var aInFunction = null;
var aInScope = null;

(function () {
    var a = "b";
    {
        let a = "c";
        aInScope = a;
    }
    aInFunction = a;
})();

var aOutside = a;

assert.strictEqual("b", aInFunction);
assert.strictEqual("a", aOutside);
assert.strictEqual("c", aInScope);