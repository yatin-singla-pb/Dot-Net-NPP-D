import { isUnassigned, normalizeToNull, coerceSelectValue } from './form-utils';

describe('form-utils', () => {
  describe('isUnassigned', () => {
    it('returns true for null/undefined', () => {
      expect(isUnassigned(null)).toBeTrue();
      expect(isUnassigned(undefined as any)).toBeTrue();
    });

    it('returns true for empty string', () => {
      expect(isUnassigned('')).toBeTrue();
      expect(isUnassigned('   ')).toBeTrue();
    });

    it('returns true for NaN and 0', () => {
      expect(isUnassigned(NaN)).toBeTrue();
      expect(isUnassigned(0)).toBeTrue();
    });

    it('returns false for valid numbers and strings', () => {
      expect(isUnassigned(1)).toBeFalse();
      expect(isUnassigned('1')).toBeFalse();
      expect(isUnassigned('abc')).toBeFalse();
    });
  });

  describe('normalizeToNull', () => {
    it('normalizes unassigned to null', () => {
      expect(normalizeToNull(undefined)).toBeNull();
      expect(normalizeToNull(null)).toBeNull();
      expect(normalizeToNull('')).toBeNull();
      expect(normalizeToNull('   ')).toBeNull();
      expect(normalizeToNull(0)).toBeNull();
      expect(normalizeToNull(NaN)).toBeNull();
    });

    it('keeps valid values as-is', () => {
      expect(normalizeToNull(5)).toBe(5);
      expect(normalizeToNull('abc')).toBe('abc');
      const obj = { id: 10 };
      expect(normalizeToNull(obj)).toBe(obj);
    });
  });

  describe('coerceSelectValue', () => {
    it('returns null for unassigned', () => {
      expect(coerceSelectValue(null)).toBeNull();
      expect(coerceSelectValue('')).toBeNull();
      expect(coerceSelectValue(0)).toBeNull();
    });

    it('passes through primitive when asObject=false', () => {
      expect(coerceSelectValue(7)).toBe(7);
    });

    it('coerces to object when asObject=true', () => {
      expect(coerceSelectValue(7, true)).toEqual({ id: 7 });
      expect(coerceSelectValue({ id: 9 }, true)).toEqual({ id: 9 });
      expect(coerceSelectValue(7, true, 'opCoId')).toEqual({ opCoId: 7 });
    });
  });
});

