var AmdLoader = (function () {
    function AmdLoader() {
        this.name = "loader";
    };
    AmdLoader.instance = new AmdLoader();
    return AmdLoader;
}());

assert.strictEqual(AmdLoader.instance.name,"loader");
