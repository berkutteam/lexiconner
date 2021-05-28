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

console.log("backgroundWorker.js");

chrome.runtime.onInstalled.addListener(() => {
  console.log(`chrome.runtime.onInstalled.`);

  // add context menu
  // https://developer.chrome.com/docs/extensions/reference/contextMenus/
  chrome.contextMenus.create(
    {
      id: "selectionContextMenu",
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
  chrome.contextMenus.onClicked.addListener((info, tab) => {
    console.log(`Context menu click:`, info, tab);
    const { menuItemId, pageUrl, selectionText } = info;
  });
});

// enabled in incognto mode?
chrome.extension.isAllowedIncognitoAccess((isAllowedAccess) => {
  console.log(`isAllowedIncognitoAccess=${isAllowedAccess}`);
});
