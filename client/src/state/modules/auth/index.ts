import {
  ActionContext,
  ActionTree,
  GetterTree,
  Module,
  MutationTree,
} from 'vuex';
import { init, signIn, signOut, signUp } from './msal';

export interface User {
  id: string;
  name: string;
  accessToken: string;
}

export interface State {
  currentUser: User | undefined;
}

export const state: State = {
  currentUser: undefined,
};

// #region Mutations

export enum MutationTypes {
  SET_CURRENT_USER = 'SET_CURRENT_USER',
}

export type Mutations<S = State> = {
  [MutationTypes.SET_CURRENT_USER](state: S, newValue: User): void;
};

export const mutations: MutationTree<State> & Mutations = {
  [MutationTypes.SET_CURRENT_USER](state, newValue: User) {
    state.currentUser = newValue;
  },
};

// #endregion

// #region Actions

type AugmentedActionContext = {
  commit<K extends keyof Mutations>(
    key: K,
    payload: Parameters<Mutations[K]>[1],
  ): ReturnType<Mutations[K]>;
} & Omit<ActionContext<State, State>, 'commit'>;

export enum ActionTypes {
  INIT = 'INIT',
  SIGN_IN = 'SIGN_IN',
  SIGN_OUT = 'LOG_OUT',
  SIGN_UP = 'SIGN_UP',
}

export interface Actions {
  [ActionTypes.INIT](
    { commit }: AugmentedActionContext,
    payload: number,
  ): Promise<void>;

  [ActionTypes.SIGN_IN]({}): Promise<void>;

  [ActionTypes.SIGN_OUT]({}): Promise<void>;

  [ActionTypes.SIGN_UP]({}): Promise<void>;
}

export const actions: ActionTree<State, State> & Actions = {
  async [ActionTypes.INIT]({ commit }) {
    const authenticatedUser = await init();
    if (!authenticatedUser) {
      return;
    }

    authenticatedUser.caseOf({
      Left: (error) => {
        console.error('Error authenticating user: ', +error);
      },

      Right: (user) => {
        commit(MutationTypes.SET_CURRENT_USER, user);
      },
    });
  },

  async [ActionTypes.SIGN_IN]({}) {
    await signIn();
  },

  async [ActionTypes.SIGN_OUT]({}) {
    await signOut();
  },

  async [ActionTypes.SIGN_UP]({}) {
    signUp();
  },
};

// #endregion

// #region Getters

export type Getters = {
  isLoggedIn(state: State): boolean;
};

export const getters: GetterTree<State, State> & Getters = {
  isLoggedIn: (state) => {
    return !!state.currentUser;
  },
};

// #endregion

export const module: Module<State, State> = {
  state,
  mutations,
  getters,
  actions,
};
