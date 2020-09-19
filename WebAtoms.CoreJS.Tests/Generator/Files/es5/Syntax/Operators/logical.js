assert(false === false);

assert((false && {}) === false);

assert((0 && {}) === 0);

assert(typeof (0 || {}) === "object");

assert(typeof (1 && {}) === "object");

assert((1 && null) === null);

assert((1 || null) === 1);