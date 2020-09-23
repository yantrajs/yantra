var a = "a";

var aInFunction = null;

(function () {
    var a = "b";
    aInFunction = a;
})();

var aOutside = a;

assert.strictEqual("b", aInFunction);
assert.strictEqual("a" , aOutside);