# vue-client

## Project setup

### Install global packages
```bash
npm install azure-cli -g 
az --version

npm install -g @vue/cli
vue --version

# list global packages
npm list -g --depth 0
```

### Install local packages
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Run your tests
```
npm run test
```

### Lints and fixes files
```
npm run lint
```

### Run your end-to-end tests
```
npm run test:e2e
```

### Run your unit tests
```
npm run test:unit
```

### Run vue-ui (part of vue-cli)
```bash
vue ui
```



# Enforce coding styles

## ESLINT

`eslint` is used to validate and fix coding styles.

```bash
npm run lint
npm run lint:fix
```

## EditorConfig

EditorConfig extension is used to maintain consistent coding styles for multiple developers working on the same project across various editors and IDEs.

VS Code extension:
https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig

Check `.editorconfig` file for more info.


## Prettier
Prettier is code formatter.

VS Code extension:
https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode

Configured in `/.vscode/settings.json`. Allows auto run for Prettier on save or paste. For instance, instead of manually running `npm run format`.


## Fix Git and CRLF line endings on Windows

By default Git uses `core.autocrlf = true` which leads to auto replacement to CRLF on commit and branch checkout.

Check current config by running `git config --list` or accessing `/.git/config` file.

Disable by running `git config core.autocrlf false`

Note: this setting can be change globally for Git, but do so if you need it.




## Tips:

Use nvm for Node.Js version management - https://github.com/coreybutler/nvm-windows

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).

Temporary ```.env.[mode].local``` file is created during serve and build with correct ```VUE_APP_ASPNETCORE_ENVIRONMENT``` and other env variables.

```VUE_APP_ASPNETCORE_ENVIRONMENT``` - ```DevelopmentLocalhost```, ```DevelopmentDocker```, ...

```vue.config.js``` - holds vue-cli config and env variables config. Can read env variables loaded from ```.env.[mode].local``` using ```process.env.<name>```.


# Help

### Run local npm package bin
If run in bash:

```
./node_modules/.bin/<package-name>
```

In cmd use backslash \ instead of /

```
.\node_modules\.bin\vue-cli-service
```

```
node ./cli/serve.js --mode=development --environment=DevelopmentLocalhost
node ./cli/serve.js --mode=development --environment=DevelopmentDocker

node ./cli/build.js --mode=production --environment=DevelopmentLocalhost --publish
node ./cli/build.js --mode=production --environment=DevelopmentDocker --publish
node ./cli/build.js --mode=production --environment=DevelopmentAzure --publish
node ./cli/build.js --mode=production --environment=ProductionAzure --publish
```

```
npm run serve_DevelopmentLocalhost
npm run serve_DevelopmentDocker

npm run build_DevelopmentLocalhost
npm run build_DevelopmentDocker
npm run build_DevelopmentAzure
npm run build_ProductionAzure
```


# Used Packages


### OIDCS and OAuth2 client
https://www.npmjs.com/package/oidc-client


### Notifications
https://www.npmjs.com/package/vue-notification


### Phone input
Good, but its styles conflicts with bootstrap4: [vue-phone-number-input](https://www.npmjs.com/package/vue-phone-number-input)


Currently used: https://www.npmjs.com/package/vue-tel-input


### Select and multiselect input
https://www.npmjs.com/package/vue-multiselect
https://vue-multiselect.js.org/#sub-getting-started

### Other form inputs
https://www.npmjs.com/package/vue-js-toggle-button

### Modals
https://www.npmjs.com/package/vue-js-modal


### Charts
https://vue-chartjs.org/


### Date & Time
https://momentjs.com/
https://momentjs.com/timezone/
https://github.com/mariomka/vue-datetime?ref=madewithvuejs.com


### Pagination
https://www.npmjs.com/package/vuejs-paginate

### Scrolling a page
https://www.npmjs.com/package/vue-scrollto


### Print
https://printjs.crabbly.com/

### Progress bar
https://github.com/dzwillia/vue-simple-progress

### Tooltip - v-tooltip
https://v-tooltip.netlify.app/

### Flags
https://www.npmjs.com/package/country-flag-icons



# Used components


### Sidebar
https://bootstrapious.com/p/bootstrap-sidebar

