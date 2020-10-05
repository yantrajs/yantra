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
assert(isNaN(Date.parse('1970-01-01T5:34')));                   // hours must be 2 digits.
assert(isNaN(Date.parse('1970-01-01T05:3')));                   // minutes must be 2 digits.
assert(isNaN(Date.parse('1970-01-01T05:34:2')));                // seconds must be 2 digits.
assert(isNaN(Date.parse('1970-01-01T05:34:22.')));              // milliseconds must have at least one digit.
assert(isNaN(Date.parse('2011-02-29T12:00:00.000Z')));          // 29 Feb did not exist in 2011.