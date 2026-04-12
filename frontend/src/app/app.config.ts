import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch } from '@angular/common/http';

import { routes } from './app.routes';
import { FormsModule } from '@angular/forms';

/**
 * Angular Configuration File - app.config.ts
 * 
 * This file is the central configuration for your Angular application.
 * It's where you register all the "providers" (services and utilities) that
 * your app needs to function.
 * 
 * Key Concepts:
 * - ApplicationConfig: TypeScript interface defining the app's configuration
 * - providers: Array of services/services that will be available throughout the app
 * - provideRouter(): Angular function that sets up client-side routing
 * - provideHttpClient(): Angular function that enables HTTP communication with APIs
 * - withFetch(): Optional parameter that uses the modern Fetch API instead of XMLHttpRequest
 * - importProvidersFrom(): Allows us to include Angular module providers (like FormsModule)
 */
export const appConfig: ApplicationConfig = {
  providers: [
    // Enable client-side routing for navigation between pages
    provideRouter(routes),
    
    // Enable HTTP communication with backend API using modern Fetch API
    provideHttpClient(withFetch()),
  ]
};