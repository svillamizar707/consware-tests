import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ConfirmService {
  private _open = new Subject<{ message: string; resolve: (v: boolean) => void }>();
  open$ = this._open.asObservable();

  confirm(message = '¿Confirmar?') {
    return new Promise<boolean>(resolve => this._open.next({ message, resolve }));
  }
}
