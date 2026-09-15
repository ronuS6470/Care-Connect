<script setup lang="ts">
import { reactive, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import AppAlert from '@/components/common/AppAlert.vue'
import AppButton from '@/components/common/AppButton.vue'
import AppInput from '@/components/common/AppInput.vue'
import { useApiError } from '@/composables/useApiError'
import { useAuthStore } from '@/stores/auth'
import { ROLE_HOME_PATH } from '@/types/enums'
import { email as emailRule, required, runRules } from '@/validation/rules'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()
const { getMessage } = useApiError()

const form = reactive({ email: '', password: '' })
const fieldErrors = reactive<{ email: string | null; password: string | null }>({ email: null, password: null })

const showPassword = ref(false)
const submitting = ref(false)
const apiError = ref<string | null>(null)

function validateEmail() {
  fieldErrors.email = runRules(form.email, [required('Enter your email.'), emailRule()])
  return fieldErrors.email === null
}

function validatePassword() {
  fieldErrors.password = runRules(form.password, [required('Enter your password.')])
  return fieldErrors.password === null
}

async function handleSubmit() {
  apiError.value = null

  const emailValid = validateEmail()
  const passwordValid = validatePassword()
  if (!emailValid || !passwordValid) return

  submitting.value = true
  try {
    const response = await auth.login({ email: form.email, password: form.password })
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : ROLE_HOME_PATH[response.role]
    router.push(redirect)
  } catch (err) {
    apiError.value = getMessage(err, 'Unable to sign in. Please try again.')
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="flex min-h-screen items-center justify-center bg-surface px-4 py-12">
    <div class="w-full max-w-sm">
      <div class="mb-8 flex flex-col items-center gap-3 text-center">
        <span class="flex h-12 w-12 items-center justify-center rounded-xl bg-brand-700 text-lg font-bold text-white">CC</span>
        <div>
          <h1 class="text-xl font-semibold text-ink">Sign in to CareConnect</h1>
          <p class="mt-1 text-sm text-ink-muted">Non-medical home-care management</p>
        </div>
      </div>

      <form class="card-base space-y-4 p-6" novalidate @submit.prevent="handleSubmit">
        <AppAlert v-if="apiError" tone="danger" dismissible @dismiss="apiError = null">{{ apiError }}</AppAlert>

        <AppInput
          v-model="form.email"
          type="email"
          label="Email"
          placeholder="you@careconnect.com"
          autocomplete="email"
          required
          :error="fieldErrors.email"
          @blur="validateEmail"
        />

        <AppInput
          v-model="form.password"
          :type="showPassword ? 'text' : 'password'"
          label="Password"
          placeholder="••••••••"
          autocomplete="current-password"
          required
          :error="fieldErrors.password"
          @blur="validatePassword"
        >
          <template #trailing>
            <button
              type="button"
              class="pointer-events-auto text-ink-muted hover:text-ink"
              :aria-label="showPassword ? 'Hide password' : 'Show password'"
              @click="showPassword = !showPassword"
            >
              <svg v-if="showPassword" class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path
                  d="M3.28 2.22a.75.75 0 00-1.06 1.06l14.5 14.5a.75.75 0 101.06-1.06l-1.745-1.745a10.029 10.029 0 003.3-4.38 1.651 1.651 0 000-1.185A10.004 10.004 0 009.999 3a9.956 9.956 0 00-4.744 1.194L3.28 2.22zM7.752 6.69l1.092 1.092a2.5 2.5 0 013.374 3.373l1.091 1.092a4 4 0 00-5.557-5.557z"
                />
                <path
                  d="M10.748 13.93l2.523 2.523a9.987 9.987 0 01-3.27.547c-4.258 0-7.894-2.66-9.337-6.41a1.651 1.651 0 010-1.186A10.007 10.007 0 012.839 6.02L6.07 9.252a4 4 0 004.678 4.678z"
                />
              </svg>
              <svg v-else class="h-4 w-4" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
                <path d="M10 12.5a2.5 2.5 0 100-5 2.5 2.5 0 000 5z" />
                <path
                  fill-rule="evenodd"
                  d="M.664 10.59a1.651 1.651 0 010-1.186A10.004 10.004 0 0110 3c4.257 0 7.893 2.66 9.336 6.41.147.381.147.804 0 1.186A10.004 10.004 0 0110 17c-4.257 0-7.893-2.66-9.336-6.41zM14 10a4 4 0 11-8 0 4 4 0 018 0z"
                  clip-rule="evenodd"
                />
              </svg>
            </button>
          </template>
        </AppInput>

        <AppButton type="submit" block :loading="submitting">Sign in</AppButton>
      </form>
    </div>
  </div>
</template>
