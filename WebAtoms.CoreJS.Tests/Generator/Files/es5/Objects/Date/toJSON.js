let event = new Date('August 19, 1975 23:15:30 UTC');

let jsonDate = event.toJSON();

assert.strictEqual("1975-08-19T23:15:30.000Z",jsonDate);
// expected output: 1975-08-19T23:15:30.000Z

assert.strictEqual("2010-04-24T23:59:57.000Z", new Date('24 Apr 2010 23:59:57 GMT').toJSON());
assert.strictEqual(null, new Date(NaN).toJSON());

//yet not implemented
//assert.strictEqual("Tue, 19 Aug 1975 23:15:30 GMT", new Date(jsonDate).toUTCString());
// expected output: Tue, 19 Aug 1975 23:15:30 GMT