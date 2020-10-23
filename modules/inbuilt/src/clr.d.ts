declare module "clr" {
    export default class Clr {
        /**
         * Returns JavaScript equivalent of .Net type
         * @param {string} name fully qualified type name
         */
        public static getClass(name: string);
    }
}