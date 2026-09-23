import { Injectable, signal } from '@angular/core';

export interface ConfirmRequest {
  title: string;
  message: string;
  confirmText: string;
  cancelText: string;
  danger: boolean;
}

export interface ConfirmOptions {
  title?: string;
  confirmText?: string;
  cancelText?: string;
  danger?: boolean;
}

@Injectable({ providedIn: 'root' })
export class ConfirmDialogService {
  request = signal<ConfirmRequest | null>(null);
  private resolver: ((result: boolean) => void) | null = null;

  confirm(message: string, options?: ConfirmOptions): Promise<boolean> {
    this.request.set({
      title: options?.title ?? 'Are you sure?',
      message,
      confirmText: options?.confirmText ?? 'Confirm',
      cancelText: options?.cancelText ?? 'Cancel',
      danger: options?.danger ?? true
    });

    return new Promise<boolean>(resolve => {
      this.resolver = resolve;
    });
  }

  resolve(result: boolean): void {
    this.resolver?.(result);
    this.resolver = null;
    this.request.set(null);
  }
}
