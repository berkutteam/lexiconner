"use strict";

// This file is executed in background when extension starts
// importScripts is Chrome built-in function

console.log("background.js");

// wrap the code in try/catch to see meaningfull errors in CHroem instead of
// "Service worker registration failed" Due to a bug in Chrome
try {
  // eslint-disable-next-line no-undef
  // importScripts("./backgroundWorker.js"); // when using this webpack will actually ignore backgroundWorker.js as it not referenced in any file
  require("./backgroundWorker.js"); // webpack's require
} catch (err) {
  console.error("Error in background.js:", err);
  throw err;
}
