assert.strictEqual("1970-01-01T00:00:00.012Z", new Date(12).toISOString());
assert.strictEqual("2010-04-24T23:59:57.000Z", new Date('24 Apr 2010 23:59:57 GMT').toISOString());
assert.strictEqual(0, new Date().toISOString.length);
let event = new Date('05 October 2011 14:48 UTC');
assert.strictEqual("2011-10-05T14:48:00.000Z", event.toISOString());