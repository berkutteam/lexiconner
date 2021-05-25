// This file is executed in background when extension starts

let color = '#3aa757';

chrome.runtime.onInstalled.addListener(() => {
  console.log(`chrome.runtime.onInstalled.`);
  chrome.storage.sync.set({ color });
  console.log('Default background color set to %cgreen', `color: ${color}`);
});

// enabled in incognto mode?
chrome.extension.isAllowedIncognitoAccess((isAllowedAccess) => {
    console.log(`isAllowedIncognitoAccess=${isAllowedAccess}`);
});
