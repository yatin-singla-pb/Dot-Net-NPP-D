import { Directive, ElementRef, HostListener, Input, OnInit, AfterViewInit, OnDestroy } from '@angular/core';
import { NgControl } from '@angular/forms';
import { Subscription } from 'rxjs';

@Directive({
  selector: '[appCurrencyInput]',
  standalone: true
})
export class CurrencyInputDirective implements OnInit, AfterViewInit, OnDestroy {
  @Input() decimalPlaces: number = 4; // Default to 4 decimal places
  @Input() currencySymbol: string = '$'; // Default to USD

  private el: HTMLInputElement;
  private valueChangeSubscription?: Subscription;
  private isInternalUpdate = false;

  constructor(
    private elementRef: ElementRef,
    private control: NgControl
  ) {
    this.el = this.elementRef.nativeElement;
  }

  ngOnInit() {
    // Subscribe to value changes from the form control
    if (this.control.control) {
      this.valueChangeSubscription = this.control.control.valueChanges.subscribe(value => {
        // Only format if this is an external update (not from our directive)
        if (!this.isInternalUpdate && (!document.activeElement || document.activeElement !== this.el)) {
          if (value !== null && value !== undefined) {
            this.formatValue(value);
          }
        }
      });
    }
  }

  ngAfterViewInit() {
    // Format initial value after view is initialized
    setTimeout(() => {
      if (this.control.value !== null && this.control.value !== undefined) {
        this.formatValue(this.control.value);
      }
    }, 0);
  }

  ngOnDestroy() {
    if (this.valueChangeSubscription) {
      this.valueChangeSubscription.unsubscribe();
    }
  }

  @HostListener('focus')
  onFocus() {
    // On focus, ensure $ symbol is present
    const value = this.el.value;

    // If there's a formatted value, keep it as-is with $ symbol
    if (value && !value.startsWith(this.currencySymbol)) {
      // Add $ if missing
      this.el.value = this.currencySymbol + value;
    }

    // Move cursor to the end
    setTimeout(() => {
      this.el.setSelectionRange(this.el.value.length, this.el.value.length);
    }, 0);
  }

  @HostListener('click')
  onClick() {
    // Prevent cursor from being placed before the $ symbol
    const cursorPosition = this.el.selectionStart || 0;
    if (cursorPosition < this.currencySymbol.length) {
      setTimeout(() => {
        this.el.setSelectionRange(this.currencySymbol.length, this.currencySymbol.length);
      }, 0);
    }
  }

  @HostListener('keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    const cursorPosition = this.el.selectionStart || 0;

    // Prevent deleting the $ symbol
    if (cursorPosition <= this.currencySymbol.length) {
      if (event.key === 'Backspace' || event.key === 'Delete') {
        event.preventDefault();
        // Move cursor after $
        this.el.setSelectionRange(this.currencySymbol.length, this.currencySymbol.length);
      } else if (event.key === 'ArrowLeft' && cursorPosition === this.currencySymbol.length) {
        event.preventDefault();
      }
    }
  }

  @HostListener('blur')
  onBlur() {
    // On blur, format the value as currency
    const value = this.el.value;
    const numericValue = this.parseValue(value);

    this.isInternalUpdate = true;
    if (numericValue !== null) {
      this.formatValue(numericValue);
      // Update the form control with the numeric value (emit event to trigger validation)
      this.control.control?.setValue(numericValue, { emitEvent: true });
    } else {
      // If empty or just the $ symbol, clear everything
      this.el.value = '';
      this.control.control?.setValue(null, { emitEvent: true });
    }
    this.isInternalUpdate = false;
  }

  @HostListener('input', ['$event'])
  onInput(event: Event) {
    // Save cursor position before any changes
    const cursorPosition = this.el.selectionStart || 0;

    // During typing, maintain the $ symbol
    let value = this.el.value;

    // If user deleted the $, add it back
    if (!value.startsWith(this.currencySymbol)) {
      value = this.currencySymbol + value;
    }

    // Extract the numeric part (everything after $)
    const numericPart = value.substring(this.currencySymbol.length);

    // Only allow valid numeric input characters (digits and decimal point)
    const cleanValue = numericPart.replace(/[^0-9.]/g, '');

    // Prevent multiple decimal points
    const parts = cleanValue.split('.');
    let finalNumericValue = cleanValue;
    if (parts.length > 2) {
      // Keep first decimal point, join the rest
      finalNumericValue = parts[0] + '.' + parts.slice(1).join('');
    }

    // Calculate how many characters were removed during cleaning
    const removedChars = numericPart.length - finalNumericValue.length;

    // Update the input value with $ symbol
    this.el.value = this.currencySymbol + finalNumericValue;

    // Restore cursor position, accounting for removed characters
    let newCursorPosition = cursorPosition - removedChars;

    // Ensure cursor is not before the $ symbol
    if (newCursorPosition < this.currencySymbol.length) {
      newCursorPosition = this.currencySymbol.length;
    }

    // Ensure cursor is not beyond the end
    if (newCursorPosition > this.el.value.length) {
      newCursorPosition = this.el.value.length;
    }

    this.el.setSelectionRange(newCursorPosition, newCursorPosition);

    // Update form control with numeric value
    this.isInternalUpdate = true;
    const numericValue = this.parseValue(finalNumericValue);
    if (numericValue !== null) {
      this.control.control?.setValue(numericValue, { emitEvent: true });
    } else if (finalNumericValue === '') {
      this.control.control?.setValue(null, { emitEvent: true });
    }
    this.isInternalUpdate = false;
  }

  private parseValue(value: string): number | null {
    if (!value || value.trim() === '') {
      return null;
    }

    // Remove currency symbol, commas, and spaces
    const cleanValue = value.replace(/[$,\s]/g, '');
    
    // Parse as float
    const parsed = parseFloat(cleanValue);
    
    return isNaN(parsed) ? null : parsed;
  }

  private formatValue(value: number | null) {
    if (value === null || value === undefined) {
      this.el.value = '';
      return;
    }

    // Format with specified decimal places
    const formatted = value.toFixed(this.decimalPlaces);
    
    // Remove trailing zeros after decimal point, but keep at least 2 decimal places
    const parts = formatted.split('.');
    if (parts.length === 2) {
      let decimals = parts[1];
      
      // Remove trailing zeros, but keep minimum 2 decimals
      while (decimals.length > 2 && decimals.endsWith('0')) {
        decimals = decimals.slice(0, -1);
      }
      
      this.el.value = `${this.currencySymbol}${parts[0]}.${decimals}`;
    } else {
      this.el.value = `${this.currencySymbol}${formatted}`;
    }
  }
}

