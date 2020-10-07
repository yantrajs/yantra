const months = ['Jan', 'March', 'April', 'June'];
let r = months.splice(1, 0, 'Feb');

assert.strictEqual(0, r.length);

assert.strictEqual("Jan,Feb,March,April,June", months.toString());

r = months.splice(4, 1, 'May');

assert.strictEqual("Jan,Feb,March,April,May", months.toString());

let myFish = ['angel', 'clown', 'mandarin', 'sturgeon'];
let removed = myFish.splice(2, 0, 'drum');

assert.strictEqual(0, removed.length);
assert.strictEqual("angel,clown,drum,mandarin,sturgeon", myFish.toString());

myFish = ['angel', 'clown', 'mandarin', 'sturgeon']
removed = myFish.splice(2, 0, 'drum', 'guitar')

assert.strictEqual(0, removed.length);
assert.strictEqual("angel,clown,drum,guitar,mandarin,sturgeon", myFish.toString());

myFish = ['angel', 'clown', 'drum', 'mandarin', 'sturgeon'];
removed = myFish.splice(3, 1);
assert.strictEqual(1, removed.length);
assert.strictEqual("mandarin", removed[0]);
assert.strictEqual("angel,clown,drum,sturgeon", myFish.toString());

myFish = ['angel', 'clown', 'drum', 'sturgeon']
removed = myFish.splice(2, 1, 'trumpet')
assert.strictEqual(1, removed.length);
assert.strictEqual("drum", removed[0]);
assert.strictEqual("angel,clown,trumpet,sturgeon", myFish.toString());

myFish = ['angel', 'clown', 'trumpet', 'sturgeon']
removed = myFish.splice(0, 2, 'parrot', 'anemone', 'blue')
assert.strictEqual(2, removed.length);
assert.strictEqual("angel,clown", removed.toString());
assert.strictEqual("parrot,anemone,blue,trumpet,sturgeon", myFish.toString());

myFish = ['parrot', 'anemone', 'blue', 'trumpet', 'sturgeon'];
removed = myFish.splice(2, 2);
assert.strictEqual(2, removed.length);
assert.strictEqual("blue,trumpet", removed.toString());
assert.strictEqual("parrot,anemone,sturgeon", myFish.toString());

myFish = ['angel', 'clown', 'mandarin', 'sturgeon'];
removed = myFish.splice(-2, 1);
assert.strictEqual(1, removed.length);
assert.strictEqual("mandarin", removed.toString());
assert.strictEqual("angel,clown,sturgeon", myFish.toString());

myFish = ['angel', 'clown', 'mandarin', 'sturgeon'];
removed = myFish.splice(2);
assert.strictEqual(2, removed.length);
assert.strictEqual("mandarin,sturgeon", removed.toString());
assert.strictEqual("angel,clown", myFish.toString());

