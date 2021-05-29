"use strict";

// Extension event listeners are a little different from the patterns you may have seen in DOM or
// Node.js APIs. The below event listener registration can be broken in to 4 distinct parts:
//
// * chrome      - the global namespace for Chrome's extension APIs
// * runtime     – the namespace of the specific API we want to use
// * onInstalled - the event we want to subscribe to
// * addListener - what we want to do with this event
//
// See https://developer.chrome.com/docs/extensions/reference/events/ for additional details.

// import apiUtil from "@/utils/api";
import wordUtil from "@/utils/word";
// import authService from "@/services/authService";

console.log("backgroundWorker.js");

const contentMenuId = "translateSelectionContextMenu";
const maxAllowedWordsInTextSelection = 1;

// enabled in incognto mode?
chrome.extension.isAllowedIncognitoAccess((isAllowedAccess) => {
  console.log(`isAllowedIncognitoAccess=${isAllowedAccess}`);
});

// onInstalled
chrome.runtime.onInstalled.addListener(() => {
  console.log(`chrome.runtime.onInstalled.`);

  // add context menu
  // https://developer.chrome.com/docs/extensions/reference/contextMenus/
  chrome.contextMenus.create(
    {
      id: contentMenuId,
      title: "Translate",
      contexts: ["selection"],
      enabled: true,
      type: "normal",
      visible: true,
    },
    () => {
      // Called when the item has been created in the browser. If an error occurs during creation, details will be available in runtime.lastError.
      console.error(`Context menu creation error:`, chrome.runtime.lastError);
    }
  );
});

// listen to context menu click
chrome.contextMenus.onClicked.addListener((info, tab) => {
  console.log(`Context menu click:`, { info, tab });
  const { menuItemId, pageUrl, selectionText } = info;
  const { favIconUrl, title, url } = tab;

  if (menuItemId !== contentMenuId) {
    return;
  }

  // check user authenticated
  // if (!(await authService.checkIsAuthenticatedAsync())) {
  //   console.error(`User is not authenticated!`);
  //   // TODO: show popup login
  //   return;
  // }

  // validate selected text
  const wordCountInSelection = wordUtil.countWordsInText(selectionText);
  if (wordCountInSelection > maxAllowedWordsInTextSelection) {
    console.error(
      `User selected ${wordCountInSelection} words, but the limit is ${maxAllowedWordsInTextSelection}.`
    );

    // show alert on the page to user
    chrome.scripting.executeScript({
      target: { tabId: tab.id },
      function: showAlertContentScript, // pass a COPY of the function
    });

    chrome.tabs.sendMessage(
      tab.id,
      {
        action: "contentScriptShowAlert",
        message: `Sorry, you selected ${wordCountInSelection} word(s), but currently we allow only ${maxAllowedWordsInTextSelection} word(s).`,
      },
      (response) => {
        console.log("Extension response:", response);
      }
    );

    return;
  }

  // inject content script programatically int othe page to get page meta
  // NB: content script hasn't access to variables declared here, but they have accessto shared DOM
  console.log("Executing content script to get page meta...");
  chrome.scripting.executeScript({
    target: { tabId: tab.id },
    function: getPageMetaContentScript, // pass a COPY of the function
  });

  // listen to response from content script
  console.log("Waiting for content script response...");
  chrome.runtime.onMessage.addListener(async function onMessageListener(
    request,
    sender,
    sendResponse
  ) {
    const isFromContentScript = sender.tab;
    if (!isFromContentScript) {
      return;
    }

    console.log("Content script response:", { request, sender });

    if (request.action === "contentScriptMetaResponse") {
      // respond synchronously (don't return true);
      // respond asynchronously (return true);
      sendResponse({
        action: "contentScriptMetaResponseConfirmed",
      });

      // unsubscribe
      chrome.runtime.onMessage.removeListener(onMessageListener);

      // build word request
      const pageMeta = request.meta;
      const word = {
        word: wordUtil.cleanupText(selectionText),
        wordLanguageCode: pageMeta.lang,

        pageMeta: {
          pageUrl,
          selectionText,
        },
      };
    }
  });
});

// #region Content script functions

function showAlertContentScript() {
  console.log("Running showAlertContentScript.");

  chrome.runtime.onMessage.addListener(function onMessageListener(
    request,
    sender,
    sendResponse
  ) {
    if (request.action === "contentScriptShowAlert") {
      // respond synchronously (don't return true);
      // respond asynchronously (return true);
      sendResponse({
        action: "contentScriptShowAlertConfirmed",
      });

      // unsubscribe
      chrome.runtime.onMessage.removeListener(onMessageListener);

      // show alert
      if (request.message) {
        window.alert(request.message);
      }
    }
  });
}

function getPageMetaContentScript() {
  console.log("Running getPageMetaContentScript.");

  // detect page language
  const lang = document.documentElement.lang;
  const meta = {
    lang,
  };

  // send meta to extension
  console.log("Send meta back to extension:", meta);
  chrome.runtime.sendMessage(
    {
      action: "contentScriptMetaResponse",
      meta,
    },
    (response) => {
      console.log("Extension response:", response);
    }
  );
}

// #endregion
