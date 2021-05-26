module.exports = {
  root: true,
  env: {
    node: true
  },
  extends: [
    'plugin:vue/essential',

    // enables the config from eslint-config-prettier, which turns off some ESLint rules that conflict with Prettier.
    'plugin:prettier/recommended',

    'eslint:recommended'
  ],
  plugins: ["prettier"],
  rules: {
    // turns on the rule provided by eslint-plugin-prettier plugin, which runs Prettier from within ESLint.
    "prettier/prettier": "error",

    // turns off two ESLint core rules that unfortunately are problematic with Prettier
    "arrow-body-style": "off",
    "prefer-arrow-callback": "off",

    //'no-console': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    // 'no-debugger': process.env.NODE_ENV === 'production' ? 'error' : 'off',
    'no-console': 'off',
    'no-debugger': 'off',
    'vue/no-unused-vars': 'off',
    'no-unused-vars': 'off',
    'no-empty-pattern': 'off',
    "linebreak-style": ["error", "unix"], // Expect linebreaks to be Unix 'LF' instead of Windows 'CRLF'
  },
  parserOptions: {
    parser: 'babel-eslint'
  },
  overrides: [
    {
      files: [
        '**/__tests__/*.{j,t}s?(x)'
      ],
      env: {
        mocha: true
      }
    }
  ]
}
