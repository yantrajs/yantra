assert(("nul" < null) === false);

assert(("nul" < 0) === false);

assert(("nul" > "0"));

assert(true < "2");

assert(!(true < "1"));

assert(!(1 < undefined));

assert(!("undefine" < undefined));

assert(!("0" < null));

assert("-1" < null);

assert("a" < "b");

assert(!("a" < "1"));

assert(!("1" < "01"));

assert("1" < "10");

assert("1" < "10s");

assert(1 < "10");

assert(!(1 < "10a"));