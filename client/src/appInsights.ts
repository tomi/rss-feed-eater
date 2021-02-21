import { ApplicationInsights } from '@microsoft/applicationinsights-web';

import { config } from './config';

let appInsights: ApplicationInsights;

const createTelemetryService = () => {
  const initialize = (): ApplicationInsights => {
    appInsights = new ApplicationInsights({
      config: {
        instrumentationKey: config.instrumentationKey,
        maxBatchInterval: 0,
        disableFetchTracking: false,
        enableCorsCorrelation: true,
        enableAutoRouteTracking: true,
        autoTrackPageVisitTime: true,
        extensions: [],
        extensionConfig: {},
      },
    });

    appInsights.loadAppInsights();
    return appInsights;
  };

  return { appInsights, initialize };
};

export const ai = createTelemetryService();
export const getAppInsights = () => {
  if (appInsights) {
    return appInsights;
  }

  const ts = createTelemetryService();
  ts.initialize();
  return appInsights;
};
