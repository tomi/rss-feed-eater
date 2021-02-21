<template>
  <primary-button @click="signIn">Login</primary-button>

  <primary-button @click="signUp">Register</primary-button>
</template>

<script>
import { defineComponent } from 'vue';
import PrimaryButton from './PrimaryButton.vue';
import { actionsTypes } from '../state/store';

export default defineComponent({
  name: 'LoginRegisterButtons',
  components: { PrimaryButton },

  methods: {
    signIn() {
      this.$store.dispatch(actionsTypes.auth.SIGN_IN);
    },

    signUp() {
      this.$store.dispatch(actionsTypes.auth.SIGN_UP);
    },
  },
});
</script>

<style></style>
