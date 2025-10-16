import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfirmService } from '../confirm.service';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `<div class="confirm-overlay" *ngIf="visible">
    <div class="confirm-box">
      <p>{{ message }}</p>
      <div class="actions">
        <button (click)="respond(false)">Cancelar</button>
        <button (click)="respond(true)">Confirmar</button>
      </div>
    </div>
  </div>`,
  styles: [`.confirm-overlay{position:fixed;inset:0;background:rgba(0,0,0,0.4);display:flex;align-items:center;justify-content:center;z-index:12000}
.confirm-box{background:#fff;padding:1rem 1.25rem;border-radius:6px;min-width:260px}
.actions{display:flex;gap:.5rem;justify-content:flex-end;margin-top:1rem}`]
})
export class ConfirmDialogComponent implements OnInit {
  visible = false;
  message = '';
  private resolver: ((v: boolean)=>void) | null = null;

  constructor(private cs: ConfirmService) {}

  ngOnInit(): void {
    this.cs.open$.subscribe(({ message, resolve }) => {
      this.message = message;
      this.resolver = resolve;
      this.visible = true;
    });
  }

  respond(value: boolean) {
    this.visible = false;
    if (this.resolver) {
      this.resolver(value);
      this.resolver = null;
    }
  }
}
