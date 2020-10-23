export default class FileUtil {
    /**
     * Writes given content at file path specified
     * @param filePath path to file
     * @param contents text content
     * @param encoding encoding - defaults to "utf-8"
     */
    public static writeAllText(filePath: string, contents: string, encoding?: string);
    /**
     * Reads content from file path specified
     * @param filePath path to file
     * @param encoding encoding - defaults to "utf-8"
     */
    public static readAllText(filePath: string, encoding?: string);
}