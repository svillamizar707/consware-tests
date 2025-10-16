import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../loading.service';

@Component({
  selector: 'app-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `<div class="spinner-overlay" *ngIf="loading">
    <div class="spinner"></div>
  </div>`,
  styles: [`.spinner-overlay{position:fixed;inset:0;background:rgba(255,255,255,0.6);display:flex;align-items:center;justify-content:center;z-index:9999}
.spinner{width:48px;height:48px;border:6px solid #eee;border-top-color:#3f51b5;border-radius:50%;animation:spin .8s linear infinite}@keyframes spin{to{transform:rotate(360deg)}}`]
})
export class SpinnerComponent implements OnInit {
  loading = false;
  constructor(private ls: LoadingService) {}
  ngOnInit(): void { this.ls.loading$.subscribe(v => this.loading = v); }
}
