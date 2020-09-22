var AmdLoader = (function () {
    function AmdLoader() {
        this.name = "loader";
    };
    AmdLoader.instance = new AmdLoader();
    return AmdLoader;
}());

assert(AmdLoader.instance.name === "loader");
