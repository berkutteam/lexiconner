import Vue from "vue";
import Multiselect from "vue-multiselect";
import Notifications from "vue-notification";
import _ from "lodash";

import store from "@/store";
import apiUtil from "@/utils/api";
import wordUtil from "@/utils/word";

import App from "./App.vue";

const logPrefix = "wordAddPopupContentScript.js.";
console.log(logPrefix);

// register globally
Vue.component("multiselect", Multiselect);
Vue.use(Notifications);

// init
apiUtil.init({
  identityUrl: process.env.VUE_APP_IDENTITY_URL,
  apiUrl: process.env.VUE_APP_API_URL,
});

let vueApp = null;
console.log(`Rendering Vue app...`);
renderApp();

let lastSelection = null;
let lastSelectionCoords = null;

const throttledLog = _.throttle((...args) => {
  console.log(...args);
}, 500);

// listen to message when to show poup near the selected word
chrome.runtime.onMessage.addListener(function onMessageListener(
  request,
  sender,
  sendResponse
) {
  console.log(logPrefix, "onMessage:", request);
  const { action, word } = request;

  if (action !== "contentScriptShowWordPopup") {
    return;
  }

  // respond synchronously (don't return true);
  // respond asynchronously (return true);
  sendResponse({
    action: "contentScriptShowWordPopupConfirmed",
  });

  console.log(`Word:`, word);

  if (!vueApp) {
    throw new Error("Vue app is not rendered.");
  }
  if (!lastSelection) {
    throw new Error("No selected text found.");
  }

  // get selected sentence
  const selectionContentText = wordUtil.getContextTextFromSelection(
    lastSelection,
    word.word
  );

  vueApp.showPopup(lastSelectionCoords, {
    ...word,
    examples: selectionContentText ? [selectionContentText] : [],
  });
});

// track mouse position to be able to open the opup near the selected word when context menu item is clicked
// document.addEventListener("mousemove", (e) => {
//   const { offsetX, offsetY, clientX, clientY, target } = e;
//   throttledLog(`mousemove:`, { offsetX, offsetY, clientX, clientY, target });
// });

// always track mouse selection on the page
document.addEventListener("selectionchange", () => {
  const selection = document.getSelection();
  throttledLog(`selectionchange:`, selection);

  if (!selection.anchorNode) {
    return;
  }

  // get coords of anchorNode
  let anchorNode = selection.anchorNode;
  let rect;
  if (anchorNode.nodeName === "#text") {
    // for text node you can't use getBoundingClientRect
    var range = document.createRange();
    range.selectNodeContents(anchorNode);
    var rects = range.getClientRects();
    if (rects.length > 0) {
      throttledLog("Text node rect: ", rects[0]);
      rect = rects[0];
    }
  } else {
    rect = anchorNode.getBoundingClientRect();
  }

  throttledLog("anchorNode:", anchorNode, "rect:", rect);

  lastSelection = selection;
  lastSelectionCoords = rect;
});

function renderApp() {
  const appElId = "lexiconnerAppRoot";
  let appEl = document.getElementById(appElId);
  if (!appEl) {
    appEl = document.createElement("div");
    appEl.id = appElId;
    document.body.appendChild(appEl);

    /* eslint-disable no-new */
    vueApp = new Vue({
      el: `#${appElId}`,
      store,
      ...App,
    });
  }
}
