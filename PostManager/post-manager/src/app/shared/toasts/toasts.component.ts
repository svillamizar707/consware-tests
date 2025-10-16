import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToastService, Toast } from '../toast.service';

@Component({
  selector: 'app-toasts',
  standalone: true,
  imports: [CommonModule],
  template: `<div class="toasts">
    <div *ngFor="let t of toasts" class="toast" [attr.data-type]="t.type">{{ t.message }}</div>
  </div>` ,
  styles: [`.toasts{position:fixed;right:1rem;top:1rem;z-index:10000}.toast{background:#222;color:#fff;padding:.6rem 1rem;border-radius:4px;margin-bottom:.5rem}[data-type="success"]{background:green}[data-type="error"]{background:#b00020}`]
})
export class ToastsComponent implements OnInit {
  toasts: Toast[] = [];
  constructor(private ts: ToastService) {}
  ngOnInit(): void { this.ts.toasts$.subscribe(t => { this.toasts = [t, ...this.toasts].slice(0,5); setTimeout(() => { this.toasts.pop() }, 3500); }); }
}
