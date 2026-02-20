import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppConfigService {
  private readonly config = {
    //apiUrl: 'http://localhost:5143/api',
    //apiUrl: 'http://34.66.36.52:8081/api',
    apiUrl: 'http://34.9.77.60:8081/api', //this one works
    production: false
  };

  get apiUrl(): string {
    return this.config.apiUrl;
  }

  get isProduction(): boolean {
    return this.config.production;
  }
}
