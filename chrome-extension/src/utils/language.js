import _ from "lodash";

class LanguageUtil {
  constructor() {}

  /** Returns ISO 2 letter language code */
  getLanguageCode(language) {
    return language.trim().split(/-|_/)[0];
  }

  /** Returns ISO 2 letter language code */
  detectBrowserLanguages() {
    // e.g.: "en", "ru-RU", "ru", "en-US", "uk"
    const browserLocales =
      navigator.languages === undefined
        ? [navigator.language]
        : navigator.languages;

    if (!browserLocales) {
      return undefined;
    }

    const langs = _.uniq(browserLocales.map((x) => x.trim().split(/-|_/)[0]));
    return langs;
  }
}

export default new LanguageUtil();
