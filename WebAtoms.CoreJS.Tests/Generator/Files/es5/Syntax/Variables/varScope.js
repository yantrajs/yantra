var a = "a";

var aInFunction = null;

(function () {
    var a = "b";
    aInFunction = a;
})();

var aOutside = a;

assert(aInFunction === "b");
assert(aOutside === "a");