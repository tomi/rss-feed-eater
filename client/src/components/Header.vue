<template>
  <header class="relative bg-white dark:bg-darker">
    <div
      class="flex items-center justify-between p-2 border-b dark:border-primary-darker"
    >
      <a
        href="/"
        class="inline-block text-2xl font-bold tracking-wider uppercase text-primary-dark dark:text-light"
      >
        RSS Feed Eater
      </a>

      <nav
        aria-label="Secondary"
        class="hidden space-x-4 md:flex md:items-center"
      >
        <user-menu v-if="isLoggedIn" />
        <login-register-buttons v-else />
      </nav>
    </div>
  </header>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import LoginRegisterButtons from './LoginRegisterButtons.vue';
import UserMenu from './UserMenu.vue';

export default defineComponent({
  name: 'Header',
  components: { LoginRegisterButtons, UserMenu },
  computed: {
    isLoggedIn() {
      return this.$store.getters.isLoggedIn;
    },
  },
});
</script>

<style scoped></style>
