/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'dgii': {
          50: '#eef9f0',
          100: '#d6f1dc',
          200: '#b1e4be',
          300: '#7fd099',
          400: '#4db36f',
          500: '#2a9752',
          600: '#1e7b41',
          700: '#196236',
          800: '#174e2e',
          900: '#144127',
          950: '#0a2414',
        }
      }
    },
  },
  plugins: [],
}

