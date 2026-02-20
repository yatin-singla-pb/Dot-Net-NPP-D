import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class FeatureFlagService {
  // Simple localStorage-backed feature flags. Enable via dev console:
  // localStorage.setItem('FEATURE_PROPOSALS_V1', 'true')
  enabled(flag: string): boolean {
    try {
      const v = localStorage.getItem(flag);
      return v === 'true';
    } catch {
      return false;
    }
  }
}

