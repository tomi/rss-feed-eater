import * as msal from '@azure/msal-browser';
import { Codec, string } from 'purify-ts/Codec';
import { config } from '../../../config';

const policies = {
  names: {
    signUp: 'B2C_1_signup',
    signIn: 'B2C_1_signin',
  },
  authorities: {
    signUp:
      'https://rssfeedeater.b2clogin.com/rssfeedeater.onmicrosoft.com/B2C_1_signup',
    signIn:
      'https://rssfeedeater.b2clogin.com/rssfeedeater.onmicrosoft.com/B2C_1_signin',
  },
  authorityDomain: 'rssfeedeater.b2clogin.com',
};

const msalConfig: msal.Configuration = {
  auth: {
    clientId: config.authConfig.clientId,
    redirectUri: config.authConfig.redirectUri,
    knownAuthorities: [policies.authorityDomain],
  },
  cache: {
    cacheLocation: 'sessionStorage',
  },
};

const idTokenClaimsCodec = Codec.interface({
  sub: string,
  name: string,
});

const responseCodec = Codec.interface({
  accessToken: string,
  idTokenClaims: idTokenClaimsCodec,
});

const scopes = {
  signIn: [
    'openid',
    'https://rssfeedeater.onmicrosoft.com/e0b926a2-97f9-4da7-a557-7e22514b9a5a/demo.read',
  ],
  signUp: [
    'openid',
    'https://rssfeedeater.onmicrosoft.com/e0b926a2-97f9-4da7-a557-7e22514b9a5a/demo.read',
  ],
  api: [
    'https://rssfeedeater.onmicrosoft.com/e0b926a2-97f9-4da7-a557-7e22514b9a5a/demo.read',
  ],
};

const msalInstance = new msal.PublicClientApplication(msalConfig);

export async function init() {
  const response = await msalInstance.handleRedirectPromise();

  if (response) {
    return response.accessToken
      ? parseResponse(response)
      : acquireAccessToken();
  }

  const accountInfo = getAccount();
  if (accountInfo) {
    return acquireAccessToken();
  }
}

export async function acquireAccessToken() {
  try {
    const response = await msalInstance.acquireTokenSilent({
      account: getAccount(),
      scopes: scopes.api,
    });

    return parseResponse(response);
  } catch (error) {
    if (error instanceof msal.InteractionRequiredAuthError) {
      await msalInstance.acquireTokenRedirect({
        account: getAccount(),
        scopes: scopes.api,
      });

      return undefined;
    }

    console.error('Acquiring token failed: ' + error);
    return undefined;
  }
}

export async function signIn() {
  await msalInstance.loginRedirect({
    scopes: scopes.signIn,
    authority: policies.authorities.signIn,
  });
}

export async function signUp() {
  await msalInstance.loginRedirect({
    scopes: scopes.signUp,
    authority: policies.authorities.signUp,
  });
}

export async function signOut() {
  const currentAccount = getAccount();
  if (!currentAccount) {
    return;
  }

  await msalInstance.logout({
    account: currentAccount,
  });
}

function getAccount() {
  const currentAccounts = msalInstance.getAllAccounts();
  if (currentAccounts.length === 0) {
    return undefined;
  }

  return currentAccounts[0];
}

function parseResponse(response: msal.AuthenticationResult) {
  return responseCodec.decode(response).map((decoded) => ({
    accessToken: decoded.accessToken,
    id: decoded.idTokenClaims.sub,
    name: decoded.idTokenClaims.name,
  }));
}
