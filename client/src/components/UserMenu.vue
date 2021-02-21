<template>
  <div class="relative">
    <button
      type="button"
      aria-haspopup="true"
      :aria-expanded="isOpen ? 'true' : 'false'"
      class="p-2 w-10 h-10 transition-colors duration-200 rounded-full text-primary bg-primary-50 hover:text-primary hover:bg-primary-100 dark:hover:text-light dark:hover:bg-primary-dark dark:bg-dark focus:outline-none focus:bg-primary-100 dark:focus:bg-primary-dark focus:ring-primary-darker"
      @click="onClick"
    >
      <span class="sr-only">User menu</span>
      {{ userInitials }}
    </button>

    <!-- User dropdown menu -->
    <transition
      name="fade"
      enter-from-class="translate-y-1/2 opacity-0"
      enter-active-class="transition-all transform ease-out"
      enter-to-class="translate-y-0 opacity-100"
      leave-from-class="translate-y-0 opacity-100"
      leave-active-class="transition-all transform ease-in"
      leave-to-class="translate-y-1/2 opacity-0"
    >
      <div
        v-show="isOpen"
        class="absolute right-0 w-48 py-1 bg-white rounded-md shadow-lg top-12 ring-1 ring-black ring-opacity-5 dark:bg-dark focus:outline-none"
        tabindex="-1"
        role="menu"
        aria-orientation="vertical"
        aria-label="User menu"
        @click="onClick"
        @keydown.escape="open = false"
      >
        <a
          href="#"
          role="menuitem"
          class="block px-4 py-2 text-sm text-gray-700 transition-colors hover:bg-gray-100 dark:text-light dark:hover:bg-primary"
        >
          Settings
        </a>
        <a
          href="#"
          role="menuitem"
          class="block px-4 py-2 text-sm text-gray-700 transition-colors hover:bg-gray-100 dark:text-light dark:hover:bg-primary"
          @click="signOut"
        >
          Logout
        </a>
      </div>
    </transition>
  </div>
</template>

<script>
import { defineComponent } from 'vue';
import { from } from 'fromfrom';
import { actionsTypes } from '../state/store';

export default defineComponent({
  name: 'UserMenu',
  components: {},
  data() {
    return {
      isOpen: false,
    };
  },

  computed: {
    isLoggedIn() {
      return this.$store.getters.isLoggedIn;
    },

    userInitials() {
      const { currentUser } = this.$store.state.auth;
      if (!currentUser) {
        return '';
      }

      return from(currentUser.name.split(' '))
        .take(2)
        .map((word) => word[0].toUpperCase())
        .toArray()
        .join('');
    },
  },

  methods: {
    onClick() {
      this.isOpen = !this.isOpen;
    },

    signOut() {
      this.$store.dispatch(actionsTypes.auth.SIGN_OUT);
    },
  },
});
</script>

<style></style>
