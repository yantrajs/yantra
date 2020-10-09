// If ES parser fails, .NET Parser succeeds, it is acceptable
// If ES Parse succeeds, .NET Parser fails, not acceptable
// If ES Parser succeeds, .NET Parser succeeds, values should be same

assert(isNaN(Date.parse("2010-0-2"))); // month out of range

assert(isNaN(Date.parse('2010-13-2')));                         // month out of range
assert(isNaN(Date.parse('2010-2-0')));                          // day out of range
assert(isNaN(Date.parse('2010-2-32')));                         // day out of range
assert(isNaN(Date.parse('1970-01-01T24:01')));                  // 24:00 is the last valid time.
assert(isNaN(Date.parse('1970-01-01T24:00:01')));               // 24:00 is the last valid time.
assert(isNaN(Date.parse('1970-01-01T24:00:00.001')));           // 24:00 is the last valid time.
assert(isNaN(Date.parse('1970-01-01T12:60')));                  // 00-59 minutes.
assert(isNaN(Date.parse('1970-01-01T12:34:60')));               // 00-59 seconds.
assert(isNaN(Date.parse('1970-01-01T12')));                     // no minutes.
assert(isNaN(Date.parse('1970-01-01T5:34')));                   // hours must be 2 digits, edge case. This works fine in .net but not in v8. v8 gives NaN. Format commented in DateParser.cs
assert(isNaN(Date.parse('1970-01-01T05:3')));                   // minutes must be 2 digits. This works fine in .net but not in v8. v8 gives NaN
//assert(isNaN(Date.parse('1970-01-01T05:34:2')));              // seconds must be 2 digits. This works fine in .net but not in v8. v8 gives NaN, edge case. This works fine in .net but not in v8. v8 gives NaN
//assert(isNaN(Date.parse('1970-01-01T05:34:22.')));            // milliseconds must have at least one digit., edge case. This works fine in .net but not in v8. v8 gives NaN
assert(isNaN(Date.parse('2011-02-29T12:00:00.000Z')));          // 29 Feb did not exist in 2011.

// Time-only forms should not be supported (see addendum).
assert(isNaN(Date.parse('T12:34Z')));

// Invalid unstructured dates.
// assert(isNaN(Date.parse('5 Jan')));                         // no year, this would work fine in ES6, is no more a negative TC
// assert(isNaN(Date.parse('Jan 2010')));                      // no day, , this would work fine in ES6, is no more a negative TC
// assert(isNaN(Date.parse('5 2010')));                        // no day, edge case. This works fine in .net but not in v8. v8 gives NaN
// assert(isNaN(Date.parse('24 Apr 2010 15 : 30 : 01')));      // spaces between time components, edge case. This works fine in .net but not in v8. v8 gives NaN.
assert(isNaN(Date.parse('24 Apr 2010 15')));                // extraneous number
// assert(isNaN(Date.parse('24 Apr 2010 15:30:01.123')));      // milliseconds not supported, this would work fine in ES6, is no more a negative TC
assert(isNaN(Date.parse('24 Apr 2010 hello')));             // extraneous text
assert(isNaN(Date.parse('24 Apr 2010 13:30:01 AM')));       // 12 hour clock goes from 0-12.
// assert(isNaN(Date.parse('24 Apr 2010 13:30:01 PM')));       // 12 hour clock goes from 0-12, edge case. This works fine in .net but not in v8. v8 gives NaN

//positive
assert.strictEqual(Date.parse('1970-01-01T12:34:56'), 25496000);
assert.strictEqual(Date.parse('2010-02-05T12:34:56.1234567890123456789Z'), 1265373296123);
assert.strictEqual(Date.parse('1970-01-01T12:34:56.123'), 25496123);
assert.strictEqual(Date.parse('2010T12:34'), 1262329440000);

assert.strictEqual(Date.parse('August 19, 1975 23:15:30 GMT+07:00'), 177696930000);
