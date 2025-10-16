import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { LoadingService } from './loading.service';
import { ToastService } from './toast.service';
import { catchError, finalize, throwError } from 'rxjs';

export const apiInterceptor: HttpInterceptorFn = (req, next) => {
  const loading = inject(LoadingService);
  const toast = inject(ToastService);

  loading.show();
  return next(req).pipe(
    finalize(() => loading.hide()),
    catchError(err => {
      toast.error('Error en la petición HTTP');
      return throwError(() => err);
    })
  );
};
