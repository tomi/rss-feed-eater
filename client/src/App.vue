<template>
  <div
    class="flex flex-col h-screen antialiased text-gray-900 bg-gray-100 dark:bg-dark dark:text-light"
  >
    <Header>Moi</Header>

    <div class="flex h-full">
      <sidebar>Kekek</sidebar>
      <img alt="Vue logo" src="./assets/logo.png" />
      <HelloWorld msg="Hello Vue 3 + TypeScript + Vite" />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import HelloWorld from './components/HelloWorld.vue';
import Header from './components/Header.vue';
import Sidebar from './components/Sidebar.vue';

export default defineComponent({
  name: 'App',
  components: {
    HelloWorld,
    Header,
    Sidebar,
  },
});
</script>

<style></style>
