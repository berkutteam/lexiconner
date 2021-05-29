class WordUtil {
  constructor() {}

  /** Deletes unwated chars from the text */
  cleanupText(text) {
    const notWantedRegex = /[^a-zA-Z- ]/g;
    const extraSpacesRegex = / {1,}/g;
    return text.replace(notWantedRegex, "").replace(extraSpacesRegex, " ");
  }

  countWordsInText(text) {
    let cleaned = this.cleanupText(text);
    return cleaned.split(" ").length;
  }
}

export default new WordUtil();
