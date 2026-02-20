import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface DashboardWidget {
  id: string;
  title: string;
  visible: boolean;
  order: number;
  adminOnly?: boolean;
  manufacturerOnly?: boolean;
}

export interface DashboardPreferences {
  widgets: DashboardWidget[];
}

@Injectable({
  providedIn: 'root'
})
export class DashboardPreferencesService {
  private readonly STORAGE_KEY = 'npp_dashboard_preferences';
  private preferencesSubject: BehaviorSubject<DashboardPreferences>;
  public preferences$: Observable<DashboardPreferences>;

  private defaultAdminWidgets: DashboardWidget[] = [
    { id: 'recent-proposals', title: 'Recent Proposals', visible: true, order: 0 },
    { id: 'recent-contracts', title: 'Recent Contracts', visible: true, order: 1 },
    { id: 'submitted-proposals', title: 'Submitted Proposals (NPP)', visible: true, order: 2, adminOnly: true },
    { id: 'expiring-contracts', title: 'Contracts Due to Expire', visible: true, order: 3, adminOnly: true },
    { id: 'velocity-exceptions', title: 'Velocity Data Exceptions', visible: true, order: 4, adminOnly: true }
  ];

  private defaultManufacturerWidgets: DashboardWidget[] = [
    { id: 'requested-proposals', title: 'Requested Proposals', visible: true, order: 0, manufacturerOnly: true },
    { id: 'pending-proposals', title: 'Pending Proposals', visible: true, order: 1, manufacturerOnly: true },
    { id: 'completed-proposals', title: 'Completed Proposals', visible: true, order: 2, manufacturerOnly: true }
  ];

  constructor() {
    const savedPreferences = this.loadPreferences();
    this.preferencesSubject = new BehaviorSubject<DashboardPreferences>(savedPreferences);
    this.preferences$ = this.preferencesSubject.asObservable();
  }

  private loadPreferences(isManufacturer: boolean = false): DashboardPreferences {
    try {
      const saved = localStorage.getItem(this.STORAGE_KEY);
      if (saved) {
        const parsed = JSON.parse(saved);
        // Merge with defaults to handle new widgets
        return this.mergeWithDefaults(parsed, isManufacturer);
      }
    } catch (error) {
      console.error('Error loading dashboard preferences:', error);
    }
    return { widgets: [...(isManufacturer ? this.defaultManufacturerWidgets : this.defaultAdminWidgets)] };
  }

  private mergeWithDefaults(saved: DashboardPreferences, isManufacturer: boolean = false): DashboardPreferences {
    const savedWidgetIds = new Set(saved.widgets.map(w => w.id));
    const merged = [...saved.widgets];
    const defaults = isManufacturer ? this.defaultManufacturerWidgets : this.defaultAdminWidgets;

    // Add any new default widgets that don't exist in saved preferences
    defaults.forEach(defaultWidget => {
      if (!savedWidgetIds.has(defaultWidget.id)) {
        merged.push({ ...defaultWidget });
      }
    });

    return { widgets: merged };
  }

  savePreferences(preferences: DashboardPreferences): void {
    try {
      localStorage.setItem(this.STORAGE_KEY, JSON.stringify(preferences));
      this.preferencesSubject.next(preferences);
    } catch (error) {
      console.error('Error saving dashboard preferences:', error);
    }
  }

  getPreferences(): DashboardPreferences {
    return this.preferencesSubject.value;
  }

  updateWidgetOrder(widgets: DashboardWidget[]): void {
    const preferences = this.getPreferences();
    preferences.widgets = widgets.map((w, index) => ({ ...w, order: index }));
    this.savePreferences(preferences);
  }

  toggleWidgetVisibility(widgetId: string): void {
    const preferences = this.getPreferences();
    const widget = preferences.widgets.find(w => w.id === widgetId);
    if (widget) {
      widget.visible = !widget.visible;
      this.savePreferences(preferences);
    }
  }

  resetToDefaults(isManufacturer: boolean = false): void {
    const defaults = isManufacturer ? this.defaultManufacturerWidgets : this.defaultAdminWidgets;
    const preferences = { widgets: [...defaults] };
    this.savePreferences(preferences);
  }

  getVisibleWidgets(isAdmin: boolean = false, isManufacturer: boolean = false): DashboardWidget[] {
    const preferences = this.getPreferences();
    return preferences.widgets
      .filter(w => {
        if (!w.visible) return false;
        if (w.adminOnly && !isAdmin) return false;
        if (w.manufacturerOnly && !isManufacturer) return false;
        return true;
      })
      .sort((a, b) => a.order - b.order);
  }

  initializeForRole(isManufacturer: boolean): void {
    // Check if we need to reinitialize based on role
    const currentPrefs = this.getPreferences();
    const hasManufacturerWidgets = currentPrefs.widgets.some(w => w.manufacturerOnly);
    const hasAdminWidgets = currentPrefs.widgets.some(w => w.adminOnly);

    // If role doesn't match current widgets, reset to defaults
    if ((isManufacturer && hasAdminWidgets && !hasManufacturerWidgets) ||
        (!isManufacturer && hasManufacturerWidgets && !hasAdminWidgets)) {
      this.resetToDefaults(isManufacturer);
    }
  }
}

