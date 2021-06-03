var AssignmentKind = null;
var ts = {};
(function (a) {
    a.a = 1;
})(AssignmentKind = ts.AssignmentKind || (ts.AssignmentKind = {}));
assert.strictEqual(AssignmentKind.a, 1);