// This file is executed in background when extension starts

try {
  require("./backgroundWorker.js");
} catch (e) {
  console.error(e);
}
