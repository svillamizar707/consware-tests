import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

export type Toast = { message: string; type?: 'success' | 'error' | 'info' };

@Injectable({ providedIn: 'root' })
export class ToastService {
  private _toasts = new Subject<Toast>();
  toasts$ = this._toasts.asObservable();

  show(message: string, type: Toast['type'] = 'info') {
    this._toasts.next({ message, type });
  }

  success(message: string) { this.show(message, 'success'); }
  error(message: string) { this.show(message, 'error'); }
  info(message: string) { this.show(message, 'info'); }
}
