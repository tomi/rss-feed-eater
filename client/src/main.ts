import { createApp } from 'vue';

import App from './App.vue';
import { store } from './state/store';
import './index.css';
import { getAppInsights } from './appInsights';

createApp(App).use(store).mount('#app');

getAppInsights().trackPageView();
