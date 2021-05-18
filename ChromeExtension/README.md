
# Chrome extension

## manifest.json

`"manifest_version": 3` - is a new version with different syntax and API in code.

`action.default_icon` - sets icons to be displayed in top right corner of the browser (browser action).

`icons` - sets icons to be displayed in extensions tab.

`permissions`:

- `storage` - storage API
- `activeTab` - access to currently active tab
- `scripting` - permission to use the Scripting API's `chrome.scripting.executeScript` method

## Files

- `popup.html` - content of the popup activated by browser action
- `options.html` - content of the options page (Settings -> Extensions -> `<Your entension name>` -> Details -> Extension option)

