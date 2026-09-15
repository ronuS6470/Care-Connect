<script setup lang="ts">
import { ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'

import AppButton from '@/components/common/AppButton.vue'
import AppInput from '@/components/common/AppInput.vue'
import AppSelect from '@/components/common/AppSelect.vue'
import { useToast } from '@/composables/useToast'
import { useAuthStore } from '@/stores/auth'
import { ROLE_HOME_PATH, UserRole } from '@/types/enums'
import { email as emailRule, required, runRules } from '@/validation/rules'

const auth = useAuthStore()
const router = useRouter()
const route = useRoute()
const toast = useToast()

const email = ref('')
const role = ref<UserRole>(UserRole.Admin)
const emailError = ref<string | null>(null)
const submitting = ref(false)

const roleOptions = [
  { label: 'Admin', value: UserRole.Admin },
  { label: 'Caregiver', value: UserRole.Caregiver },
  { label: 'Client', value: UserRole.Client },
]

async function handleSubmit() {
  emailError.value = runRules(email.value, [required('Enter your email.'), emailRule()])
  if (emailError.value) return

  submitting.value = true
  try {
    await auth.login({ email: email.value, role: role.value })
    const redirect = typeof route.query.redirect === 'string' ? route.query.redirect : ROLE_HOME_PATH[role.value]
    router.push(redirect)
  } catch {
    toast.error('Unable to sign in. Please try again.')
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

      <form class="card-base space-y-4 p-6" @submit.prevent="handleSubmit">
        <AppInput
          v-model="email"
          type="email"
          label="Email"
          placeholder="you@careconnect.com"
          autocomplete="email"
          required
          :error="emailError"
          @blur="emailError = runRules(email, [required('Enter your email.'), emailRule()])"
        />

        <AppSelect v-model="role" label="Sign in as" :options="roleOptions" required />

        <AppButton type="submit" block :loading="submitting">Continue</AppButton>

        <p class="text-center text-xs text-ink-muted">
          Development sign-in — connects using the API's local test identities until Auth0 login is wired in.
        </p>
      </form>
    </div>
  </div>
</template>
