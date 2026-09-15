import { fileURLToPath, URL } from 'node:url'

import tailwindcss from '@tailwindcss/vite'
import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vite'

export default defineConfig({
  plugins: [vue(), tailwindcss()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    port: 5173,
    // The API (CareConnect.Controller) has no CORS policy configured, and this is a
    // frontend-only project — this proxy lets the dev server call it same-origin instead of
    // requiring a backend change. Production points VITE_API_BASE_URL at the real API host
    // directly, where CORS (or a reverse-proxy) is a deployment-time concern.
    proxy: {
      '/api': {
        target: 'http://localhost:5035',
        changeOrigin: true,
      },
    },
  },
})
