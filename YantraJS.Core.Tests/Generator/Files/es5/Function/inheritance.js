// A class is a special type of JavaScript object which
// is always created via a constructor. These classes
// act a lot like objects, and have an inheritance structure
// similar to languages such as Java/C#/Swift.
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
// Here's an example class:
var Vendor = /** @class */ (function () {
    function Vendor(name) {
        this.name = name;
    }
    Vendor.prototype.greet = function () {
        return "Hello, welcome to " + this.name;
    };
    return Vendor;
}());
// An instance can be created via the new keyword, and
// you can call methods and access properties from the
// object.
var shop = new Vendor("Ye Olde Shop");
console.log(shop.greet());
// You can subclass an object. Here's a food cart which
// has a variety as well as a name:
var FoodTruck = /** @class */ (function (_super) {
    __extends(FoodTruck, _super);
    function FoodTruck(name, cuisine) {
        var _this = _super.call(this, name) || this;
        _this.cuisine = cuisine;
        return _this;
    }
    FoodTruck.prototype.greet = function () {
        return "Hi, welcome to food truck " + this.name + ". We serve " + this.cuisine + " food.";
    };
    return FoodTruck;
}(Vendor));
// Because we indicated that there needs to be two arguments
// to create a new FoodTruck, TypeScript will provide errors
// when you only use one:
var nameOnlyTruck = new FoodTruck("Salome's Adobo", "");
// Correctly passing in two arguments will let you create a
// new instance of the FoodTruck:
var truck = new FoodTruck("Dave's Doritos", "junk");
console.log(truck.greet());