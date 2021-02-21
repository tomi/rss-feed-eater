<template>
  <button
    class="inline-block px-6 py-2 text-xs font-medium leading-6 text-center text-blue-700 uppercase transition bg-transparent border-2 border-blue-700 rounded-full ripple hover:bg-blue-100 focus:outline-none"
  >
    <slot />
  </button>
</template>

<script>
import { defineComponent } from 'vue';

export default defineComponent({
  name: 'PrimaryButton',
});
</script>

<style></style>
