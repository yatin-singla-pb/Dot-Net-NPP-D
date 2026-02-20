import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Role } from './auth.service';

// Re-export Role for convenience
export type { Role } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly endpoint = 'roles';

  constructor(private apiService: ApiService) {}

  getAllRoles(): Observable<Role[]> {
    return this.apiService.get<Role[]>(this.endpoint);
  }

  getRoleById(id: number): Observable<Role> {
    return this.apiService.get<Role>(`${this.endpoint}/${id}`);
  }

  createRole(role: { name: string; description?: string }): Observable<Role> {
    return this.apiService.post<Role>(this.endpoint, role);
  }

  updateRole(id: number, role: { name: string; description?: string; isActive: boolean }): Observable<Role> {
    return this.apiService.put<Role>(`${this.endpoint}/${id}`, role);
  }

  deleteRole(id: number): Observable<any> {
    return this.apiService.delete(`${this.endpoint}/${id}`);
  }

  activateRole(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/activate`, {});
  }

  deactivateRole(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/deactivate`, {});
  }

  // Alias methods for compatibility
  getAll(): Observable<Role[]> {
    return this.getAllRoles();
  }

  getById(id: number): Observable<Role> {
    return this.getRoleById(id);
  }

  create(role: { name: string; description?: string }): Observable<Role> {
    return this.createRole(role);
  }

  update(id: number, role: { name: string; description?: string; isActive: boolean }): Observable<Role> {
    return this.updateRole(id, role);
  }

  delete(id: number): Observable<any> {
    return this.deleteRole(id);
  }
}
