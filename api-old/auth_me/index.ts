import { AzureFunction, Context, HttpRequest } from '@azure/functions';
import * as azureJwt from 'azure-jwt-verify';

const verifyJwt = async (token: string) => {
  return await azureJwt.verify(token, {
    JWK_URI:
      'https://rssfeedeater.b2clogin.com/rssfeedeater.onmicrosoft.com/b2c_1_signupsignin1/discovery/v2.0/keys',
    ISS:
      'https://rssfeedeater.b2clogin.com/a4c3a8f3-8003-445c-b77b-84b73801a070/v2.0/',
    AUD: process.env.AZURE_AD_CLIENT_ID,
  });
};

const httpTrigger: AzureFunction = async function (
  context: Context,
  req: HttpRequest,
): Promise<void> {
  const authHeader = req.headers.authorization;
  const accessToken = authHeader && authHeader.replace(/^Bearer /gi, '');
  if (!accessToken) {
    context.res = {
      status: 401,
      body: JSON.stringify({
        message: 'Missing authorization header',
      }),
    };

    return;
  }

  try {
    const parsedToken = await verifyJwt(accessToken);

    context.res = {
      body: parsedToken,
    };
  } catch (error) {
    context.log.error('Invalid access token', error);
    context.res = {
      status: 401,
      body: JSON.stringify({
        message: 'Missing authentication token',
      }),
    };

    return;
  }
};

export default httpTrigger;
