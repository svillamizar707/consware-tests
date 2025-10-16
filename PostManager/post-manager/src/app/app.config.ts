// This file contains the application configuration for Angular
// It sets up the necessary providers and routes


import { ApplicationConfig, provideBrowserGlobalErrorListeners, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';


import { routes } from './app.routes';
import { apiInterceptor } from './shared/http-interceptor';


export const appConfig: ApplicationConfig = {
  providers: [
  provideBrowserGlobalErrorListeners(),
  provideZoneChangeDetection({ eventCoalescing: true }),
  provideRouter(routes),
    provideHttpClient(withInterceptors([apiInterceptor]))
  ]
};

