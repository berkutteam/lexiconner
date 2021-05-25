
# Chrome extension

## manifest.json

```json
{
    "name": "Lexiconner",
    "description": "Lexiconner Chrome Extension",
    "version": "1.0",
    
    // v1, v2 - old, v3 - new
    "manifest_version": 3,

    // required permissions that will be shown during installation to user
    "permissions": [
        // storage API
        "storage",

        // access to currently active tab
        "activeTab",

        // permission to use the Scripting API's `chrome.scripting.executeScript` method
        "scripting"
    ],

    // optional permissions that won't be shown during installation to user, but will be requested during work with the extension
    "optional_permissions": [

    ],

    // register background js file that can do some init work and listen to Chrome events
    "background": {
        "service_worker": "background.js",

        // the persistent key should be set to false unless the extension uses chrome.webRequest API to block or modify network requests
        "persistent": false
    },

    // sets icons to be displayed in top right corner of the browser (browser action)
    "action": {
        "default_popup": "popup.html",
        "default_icon": {
            "16": "images/get_started16.png",
            "32": "images/get_started32.png",
            "48": "images/get_started48.png",
            "128": "images/get_started128.png"
        }
    },

    // sets icons to be displayed in Chrome Settings -> Extensions.
    "icons": {
        "16": "images/get_started16.png",
        "32": "images/get_started32.png",
        "48": "images/get_started48.png",
        "128": "images/get_started128.png"
    },

    "options_page": "options.html",

    // An extension's content security policy (CSP) was specified in MV2 as a string; in MV3 it is an object with members representing alternative CSP contexts:
    // https://developer.chrome.com/docs/extensions/mv3/intro/mv3-migration/#fcontent-security-policy
    "content_security_policy":  {
        // This policy covers pages in your extension, including html files and service workers.
        "extension_pages": "script-src 'self' ; object-src 'self'",

        // NB: 'unsafe-eval'is forbidden in V3. E.g. this will fail:
        "extension_pages": "script-src 'self' 'unsafe-eval'; object-src 'self';",

        // This policy covers any sandboxed extension pages that your extension uses.
        "sandbox": "..."
    }
}
```

Keyboard shortcut

```json
"commands": {
    "_execute_action": {
        "suggested_key": {
        "default": "Ctrl+Shift+F",
        "mac": "MacCtrl+Shift+F"
        },
        "description": "Opens popup.html"
    }
}
```

## Files

- `popup.html` - content of the popup activated by browser action
- `options.html` - content of the options page (Settings -> Extensions -> `<Your entension name>` -> Details -> Extension option)

## Resources

- Debugging - https://developer.chrome.com/docs/extensions/mv3/tut_debugging/
- Permissions - https://developer.chrome.com/docs/extensions/mv3/permission_warnings/
- API Reference - https://developer.chrome.com/docs/extensions/reference/


## Use with Vue.js

### Vue.js 2

```bash
npm install -g @vue/cli

# create vue app using boilerplate - 
# https://github.com/Kocal/vue-web-extension
# https://github.com/adambullmer/vue-cli-plugin-browser-extension
vue create --preset kocal/vue-web-extension chrome-extension
cd chrome-extension
npm i
npm run build
npm run serve
```

Resources:
- https://www.sitepoint.com/build-vue-chrome-extension/
- https://vue-web-extension.netlify.app/intro/setup.html
- https://github.com/Kocal/vue-web-extension
- https://github.com/adambullmer/vue-cli-plugin-browser-extension


#### Fix error: "Service worker registration failed" when adding extension lcoally to Chrome using kocal/vue-web-extension preset

*using kocal/vue-web-extension* preset configures `background.js` this way:
```json
"background": {
    "service_worker": "js/background.js"
}
```

As stated here https://stackoverflow.com/questions/66114920/service-worker-registration-failed-chrome-extension it's because of the fact, that in the manifest v3: *Service worker file must be in the root path of the extension where manifest.json is.*

Meanwhile, the correct manifest.json should look like this:
```json
"background": {
  "service_worker": "background.js"
},
```


Solution:

Fork https://github.com/adambullmer/vue-cli-plugin-browser-extension and make fxes manually.

Forked version: https://github.com/berkutteam/vue-cli-plugin-browser-extension

Install forked package
```bash
yarn remove vue-cli-plugin-browser-extension --dev

yarn add git+https://github.com/berkutteam/vue-cli-plugin-browser-extension.git --dev
```

#### Fix Webpack source maps and unsafe-eval issue

It might throw this error:

```log
Uncaught EvalError: Refused to evaluate a string as JavaScript because 'unsafe-eval' is not an allowed source of script in the following Content Security Policy directive: "script-src 'self'".
```

Check the answer:

https://stackoverflow.com/a/49100966/5168794


### Vue.js 3
TODO
