import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from './loading.service';
import { ToastService } from './toast.service';
import { catchError, finalize, throwError } from 'rxjs';

// Show spinner only if request takes longer than this (ms)
const SPINNER_DELAY = 150;

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const loading = inject(LoadingService);
  const toast = inject(ToastService);

  let shown = false;
  const timer = setTimeout(() => {
    loading.show();
    shown = true;
  }, SPINNER_DELAY);

  return next(req).pipe(
    finalize(() => {
      clearTimeout(timer);
      if (shown) loading.hide();
    }),
    catchError((err: any) => {
      // Mejorar mensaje según status
      const status = err?.status ?? null;
      if (status === 0) {
        toast.error('No hay conexión. Revisa tu red.');
      } else if (status === 404) {
        toast.error('Recurso no encontrado (404).');
      } else if (status >= 500) {
        toast.error('Error del servidor. Intenta nuevamente más tarde.');
      } else if (status) {
        toast.error(`Error HTTP ${status}.`);
      } else {
        toast.error('Error en la petición HTTP.');
      }

      return throwError(() => err);
    })
  );
};
