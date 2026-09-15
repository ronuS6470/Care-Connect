import { createPinia } from 'pinia'
import { createApp } from 'vue'

import App from './App.vue'
import router from './router'
import { useAuthStore } from './stores/auth'

import './assets/main.css'

const app = createApp(App)

app.use(createPinia())

// Restores any locally-stored session before the router's first navigation runs, so its very
// first guard check (see router/index.ts) already knows the real auth/role state.
useAuthStore().initializeAuth()

app.use(router)

app.mount('#app')
