import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import { ToastsComponent } from './shared/toasts/toasts.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, SpinnerComponent, ToastsComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('post-manager');
}
