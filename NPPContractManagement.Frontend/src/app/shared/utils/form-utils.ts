export function isUnassigned(value: any): boolean {
  // Treat null, undefined, empty string, NaN, and 0 as unassigned
  if (value === null || value === undefined) return true;
  if (typeof value === 'string' && value.trim() === '') return true;
  if (typeof value === 'number' && (isNaN(value) || value === 0)) return true;
  return false;
}

export function normalizeToNull<T = any>(value: T): T | null {
  return isUnassigned(value) ? null : value;
}

// For selects that bind to primitives or objects
export function coerceSelectValue<T = any>(value: any, asObject: boolean = false, idKey: string = 'id'): any {
  if (isUnassigned(value)) return null;
  if (!asObject) return value;
  // For object-bound selects, accept either id or object; return object-ish form
  if (typeof value === 'object' && value !== null) return value;
  return { [idKey]: value } as any;
}

