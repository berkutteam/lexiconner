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

  getContextTextFromSelection(selection, selectedText) {
    const ELEMENT_NODE = 1; // e.g. span
    const TEXT_NODE = 3;

    if (![ELEMENT_NODE, TEXT_NODE].includes(selection.anchorNode.nodeType)) {
      throw new Error(
        `nodeType ${selection.anchorNode.nodeType} is not supported.`
      );
    }

    let text = "";
    switch (selection.anchorNode.nodeType) {
      case ELEMENT_NODE:
        text = selection.anchorNode.innerText;
        break;
      case TEXT_NODE:
        text = selection.anchorNode.textContent;
        break;
    }
    let parentText = selection.anchorNode.parentNode.innerText;
    let parentParentText = selection.anchorNode.parentNode.parentNode.innerText;

    console.log(1, text);
    console.log(2, parentText);
    // console.log(3, parentParentText);

    // text must contain the whole sentece where word placed
    const searchTerm =
      text
        .split(".")
        .find((sentence) =>
          sentence.toLowerCase().includes(selectedText.toLowerCase())
        ) || "";
    console.log(1, searchTerm);
    let resultText =
      parentParentText
        .split(".")
        .map((sentence) => sentence + ".")
        .find((sentence) =>
          sentence.toLowerCase().includes(searchTerm.toLowerCase())
        ) || null;

    // cleanup the result text
    if (resultText) {
      resultText = resultText.replace(/\n|\n\r|\[\d{1,}\]/g, "");
    }

    return resultText;
  }
}

export default new WordUtil();
