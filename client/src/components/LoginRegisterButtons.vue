<template>
  <primary-button @click="login">Login</primary-button>

  <primary-button @click="register">Register</primary-button>
</template>

<script>
import { defineComponent } from 'vue';
import PrimaryButton from './PrimaryButton.vue';
import { actionsTypes } from '../state/store';

export default defineComponent({
  name: 'LoginRegisterButtons',
  components: { PrimaryButton },

  methods: {
    login() {
      this.$store.dispatch(actionsTypes.auth.LOG_IN);
    },

    register() {
      // TODO
    },
  },
});
</script>

<style></style>
