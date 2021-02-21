import { array, Codec, string } from 'purify-ts/Codec';

const configCodec = Codec.interface({
  authConfig: Codec.interface({
    clientId: string,
    redirectUri: string,
    authority: string,
    knownAuthorities: array(string),
  }),
});

export const config = configCodec
  .decode({
    authConfig: {
      clientId: import.meta.env.VITE_AUTH_CLIENT_ID,
      redirectUri: import.meta.env.VITE_AUTH_REDIRECT_URI,
      authority: import.meta.env.VITE_AUTH_AUTHORITY,
      knownAuthorities: [import.meta.env.VITE_AUTH_KNOWN_AUTHORITY],
    },
  })
  .caseOf({
    Left: (error) => {
      throw new Error('Unabled to parse config: ' + error);
    },

    Right: (x) => x,
  });
