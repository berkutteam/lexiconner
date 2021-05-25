
// browser.runtime.onMessage.addListener(function (request, sender, sendResponse) {
//   console.log("Hello from the background");

//   browser.tabs.executeScript({
//     file: "content-script.js",
//   });
// });

throw new Error("lol");

chrome.runtime.onInstalled.addListener(() => {
    console.log(`chrome.runtime.onInstalled.`);
});

// enabled in incognto mode?
chrome.extension.isAllowedIncognitoAccess((isAllowedAccess) => {
    console.log(`isAllowedIncognitoAccess=${isAllowedAccess}`);
});
  
