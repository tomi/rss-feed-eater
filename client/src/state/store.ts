import {
  CommitOptions,
  createStore,
  DispatchOptions,
  Store as VuexStore,
} from 'vuex';
import * as auth from './modules/auth';

const modules = {
  auth: auth.module,
};

export type State = {
  auth: auth.State;
};
export type Mutations = auth.Mutations;
export type Actions = auth.Actions;
export type Getters = auth.Getters;

export const actionsTypes = {
  auth: auth.ActionTypes,
};

export const store = createStore({
  modules,
  // Enable strict mode in development to get a warning
  // when mutating state outside of a mutation.
  // https://vuex.vuejs.org/guide/strict.html
  strict: import.meta.env.DEV,
});

export type Store = Omit<
  VuexStore<State>,
  'getters' | 'commit' | 'dispatch'
> & {
  commit<K extends keyof Mutations, P extends Parameters<Mutations[K]>[1]>(
    key: K,
    payload: P,
    options?: CommitOptions,
  ): ReturnType<Mutations[K]>;
} & {
  dispatch<K extends keyof Actions>(
    key: K,
    payload: Parameters<Actions[K]>[1],
    options?: DispatchOptions,
  ): ReturnType<Actions[K]>;
} & {
  getters: {
    [K in keyof Getters]: ReturnType<Getters[K]>;
  };
};

export function useStore() {
  return (store as unknown) as Store;
}

store.dispatch(auth.ActionTypes.INIT);
// Automatically run the `init` action for every module,
// if one exists.
// dispatchActionForAllModules('init')
