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

assert(aInFunction === "b");
assert(aOutside === "a");
assert(aInScope === "c");