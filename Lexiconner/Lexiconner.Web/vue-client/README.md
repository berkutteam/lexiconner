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


# Used components


### Sidebar
https://bootstrapious.com/p/bootstrap-sidebar

