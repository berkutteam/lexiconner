module.exports = {
  lintOnSave: true,

  pages: {
    popup: {
      template: "public/browser-extension.html",
      entry: "./src/popup/main.js",
      title: "Popup",
    },
    options: {
      template: "public/browser-extension.html",
      entry: "./src/options/main.js",
      title: "Options",
    },
  },

  pluginOptions: {
    // https://github.com/adambullmer/vue-cli-plugin-browser-extension
    browserExtension: {
      componentOptions: {
        background: {
          entry: "src/background.js",
        },
        contentScripts: {
          entries: {
            "content-script": ["src/content-scripts/content-script.js"],
          },
        },
      },

      // Names of manifest.json keys that will be automatically synced with package.json on build
      manifestSync: ["version", "description"],

      // Function to modify the manifest JSON outputted by this plugin.
      manifestTransformer: (manifest) => {
        console.log(`manifestTransformer.`, process.env.BROWSER);
        return manifest;
      },

      // Directory where the zipped browser extension should get created.
      // (artificats created only for NODE_ENV=production)
      artifactsDir: "./artifacts",
      artifactFilename: ({ name, version, mode }) => {
        if (mode === "production") {
          return `${name}-v${version}.zip`;
        }
        return `${name}-v${version}-${mode}.zip`;
      },
    },
  },

  // tweak the webpack config
  // https://cli.vuejs.org/guide/webpack.html
  configureWebpack: (config) => {
    if (process.env.NODE_ENV === "production") {
      // mutate config for production...
      return {};
    } else {
      // mutate for development...
      return {
        // WebPack can generate a lot of eval which is forbidden by Chrome Extensions CSP
        // https://stackoverflow.com/a/49100966/5168794
        // https://webpack.js.org/configuration/devtool/
        devtool: "cheap-module-source-map",
      };
    }
  },
};
