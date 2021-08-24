function t(a, ...args) {
    return [a.raw, ...args];
}

console.log(t`a ${1}`);