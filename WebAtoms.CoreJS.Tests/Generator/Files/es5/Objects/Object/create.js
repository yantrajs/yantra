var person = {
    isHuman: false,
    printIntroduction: function () {
        console.log("" + this.name + " is " + (this.isHuman ? 'human' : 'not human'));
    }
};

var me = Object.create(person);
assert(me.printIntroduction);
assert.strictEqual(typeof me.printIntroduction, "function");