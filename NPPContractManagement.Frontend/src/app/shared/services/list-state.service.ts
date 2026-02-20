import { Injectable } from '@angular/core';

export interface ListState {
  filters: { [key: string]: any };
  pageIndex: number;
  pageSize: number;
  sortField: string;
  sortDirection: 'asc' | 'desc';
  savedAt: number;
}

@Injectable({
  providedIn: 'root'
})
export class ListStateService {
  private readonly PREFIX = 'npp_list_';
  private readonly USER_KEY = 'npp_user';
  private readonly MAX_AGE_MS = 24 * 60 * 60 * 1000; // 24 hours

  private getUserId(): number | null {
    try {
      const raw = localStorage.getItem(this.USER_KEY);
      if (!raw) return null;
      const user = JSON.parse(raw);
      return user?.id ?? null;
    } catch {
      return null;
    }
  }

  private getKey(pageKey: string): string | null {
    const userId = this.getUserId();
    if (userId == null) return null;
    return `${this.PREFIX}${userId}_${pageKey}`;
  }

  saveState(pageKey: string, state: ListState): void {
    const key = this.getKey(pageKey);
    if (!key) return;
    try {
      state.savedAt = Date.now();
      localStorage.setItem(key, JSON.stringify(state));
    } catch (e) {
      console.warn('Failed to save list state:', e);
    }
  }

  getState(pageKey: string): ListState | null {
    const key = this.getKey(pageKey);
    if (!key) return null;
    try {
      const raw = localStorage.getItem(key);
      if (!raw) return null;
      const state: ListState = JSON.parse(raw);
      // Expire stale state
      if (state.savedAt && Date.now() - state.savedAt > this.MAX_AGE_MS) {
        localStorage.removeItem(key);
        return null;
      }
      return state;
    } catch (e) {
      console.warn('Failed to restore list state:', e);
      return null;
    }
  }

  clearState(pageKey: string): void {
    const key = this.getKey(pageKey);
    if (!key) return;
    localStorage.removeItem(key);
  }

  clearAllStates(): void {
    const userId = this.getUserId();
    const prefix = userId != null ? `${this.PREFIX}${userId}_` : this.PREFIX;
    const keysToRemove: string[] = [];
    for (let i = 0; i < localStorage.length; i++) {
      const k = localStorage.key(i);
      if (k && k.startsWith(prefix)) {
        keysToRemove.push(k);
      }
    }
    keysToRemove.forEach(k => localStorage.removeItem(k));
  }
}
