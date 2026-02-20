import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ApiService } from './api.service';
import { User, Role } from './auth.service';

// Re-export User and Role for convenience
export type { User, Role } from './auth.service';
export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface UserOption { id: number; name: string; email: string; }

export interface CreateUserRequest {
  userId?: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  company?: string;
  jobTitle?: string;
  address?: string;
  city?: string;
  state?: string;
  postCode?: string;
  notes?: string;
  status: number;
  failedAuthAttempts?: number;
  industryId?: number;
  accountStatus?: number;
  class?: string;
  groupEmail?: boolean;
  roleIds: number[];
  manufacturerIds?: number[];
}

// Alias for compatibility
export type CreateUserDto = CreateUserRequest;

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  company?: string;
  jobTitle?: string;
  address?: string;
  city?: string;
  state?: string;
  postCode?: string;
  notes?: string;
  status: number;
  failedAuthAttempts?: number;
  industryId?: number;
  accountStatus?: number;
  class?: string;
  groupEmail?: boolean;
  isActive: boolean;
  roleIds: number[];
  manufacturerIds?: number[];
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private readonly endpoint = 'users';

  constructor(private apiService: ApiService) {}
  getPaginated(pageNumber = 1, pageSize = 10, sortBy = 'Id', sortDirection: 'asc' | 'desc' = 'asc', searchTerm = '', status: string = ''): Observable<PaginatedResult<User>> {
    const params: any = { pageNumber, pageSize, sortBy, sortDirection };
    if (searchTerm) params.searchTerm = searchTerm;
    if (status) params.status = status;

    return this.apiService.get<any>(this.endpoint, params).pipe(
      map(response => {
        const items = (response.Items || response.items || []) as User[];
        return {
          items,
          totalCount: response.TotalCount || response.totalCount || items.length,
          pageNumber: response.PageNumber || response.pageNumber || pageNumber,
          pageSize: response.PageSize || response.pageSize || pageSize,
          totalPages: response.TotalPages || response.totalPages || Math.ceil((response.TotalCount || items.length) / (response.PageSize || pageSize))
        } as PaginatedResult<User>;
      })
    );
  }


  getAllUsers(): Observable<User[]> {
    return this.apiService.get<User[]>(this.endpoint);
  }

  getUserById(id: number): Observable<User> {
    return this.apiService.get<User>(`${this.endpoint}/${id}`);
  }

  getUserByUserId(userId: string): Observable<User> {
    return this.apiService.get<User>(`${this.endpoint}/by-userid/${userId}`);
  }

  createUser(user: CreateUserRequest): Observable<User> {
    return this.apiService.post<User>(this.endpoint, user);
  }

  updateUser(id: number, user: UpdateUserRequest): Observable<User> {
    return this.apiService.put<User>(`${this.endpoint}/${id}`, user);
  }

  deleteUser(id: number): Observable<any> {
    return this.apiService.delete(`${this.endpoint}/${id}`);
  }

  activateUser(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/activate`, {});
  }

  deactivateUser(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/deactivate`, {});
  }

  suspendUser(id: number): Observable<any> {
    return this.apiService.patch(`${this.endpoint}/${id}/suspend`, {});
  }

  unsuspendUser(id: number): Observable<any> {
    return this.apiService.patch(`${this.endpoint}/${id}/unsuspend`, {});
  }

  sendUserInvitation(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/send-invitation`, {});
  }

  resendRegistrationInvitation(id: number): Observable<any> {
    return this.apiService.post(`${this.endpoint}/${id}/resend-invitation`, {});
  }

  // Alias methods for compatibility
  getAll(): Observable<User[]> {
    return this.getAllUsers();
  }

  getById(id: number): Observable<User> {
    return this.getUserById(id);
  }

  create(user: CreateUserRequest): Observable<User> {
    return this.createUser(user);
  }

  update(id: number, user: UpdateUserRequest): Observable<User> {
    return this.updateUser(id, user);
  }

  delete(id: number): Observable<any> {
    return this.deleteUser(id);
  }

  // User -> Manufacturers assignments
  getUserManufacturers(userId: number): Observable<{ id: number; name: string }[]> {
    return this.apiService.get<any>(`${this.endpoint}/${userId}/manufacturers`).pipe(
      map((arr: any[]) => (arr || []).map(x => ({ id: x.id, name: x.name })))
    );
  }

  // Eligible brokers for a manufacturer (users with Manufacturer role, optionally filtered by manufacturer assignment)
  getEligibleBrokers(manufacturerId?: number, searchTerm?: string, limit = 1000): Observable<UserOption[]> {
    const params: any = { limit };
    if (manufacturerId != null) params.manufacturerId = manufacturerId;
    if (searchTerm) params.searchTerm = searchTerm;
    return this.apiService.get<any[]>(`${this.endpoint}/eligible-brokers`, params).pipe(
      map(arr => (arr || []).map(x => ({ id: x.id, name: x.name, email: x.email }) as UserOption))
    );
  }

  getUsersByManufacturer(manufacturerId: number): Observable<UserOption[]> {
    return this.apiService.get<any[]>(`${this.endpoint}/by-manufacturer/${manufacturerId}`).pipe(
      map(arr => (arr || []).map(x => ({
        id: x.id,
        name: `${x.firstName} ${x.lastName}`.trim(),
        email: x.email
      }) as UserOption))
    );
  }

  syncUserManufacturers(userId: number, manufacturerIds: number[]): Observable<void> {
    return this.apiService.post<void>(`${this.endpoint}/${userId}/manufacturers`, manufacturerIds || []);
  }

  // Profile self-service
  getProfile(): Observable<User> {
    return this.apiService.get<User>('auth/profile');
  }

  updateProfile(data: {
    firstName: string;
    lastName: string;
    email: string;
    phoneNumber?: string;
    company?: string;
    jobTitle?: string;
    address?: string;
    city?: string;
    state?: string;
    postCode?: string;
  }): Observable<User> {
    return this.apiService.put<User>('auth/profile', data);
  }
}
