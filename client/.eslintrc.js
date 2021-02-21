module.exports = {
  root: true,
  env: {
    node: true,
  },
  parserOptions: {
    parser: '@typescript-eslint/parser',
    ecmaVersion: 2020,
  },
  plugins: ['@typescript-eslint'],
  extends: ['plugin:vue/vue3-recommended', 'prettier'],
  rules: {
    'no-console': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'no-debugger': process.env.NODE_ENV === 'production' ? 'warn' : 'off',
    'spaced-comment': 'error',
    '@typescript-eslint/no-unused-vars': 'error',
  },
}
