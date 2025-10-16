import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostsService } from '../../services/posts.service';
import { Post } from '../../models/post.model';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';
import { LoadingService } from '../../shared/loading.service';
import { ToastService } from '../../shared/toast.service';
import { ConfirmService } from '../../shared/confirm.service';

@Component({
  selector: 'app-posts-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './posts-list.component.html',
  styleUrls: ['./posts-list.component.scss']
})
export class PostsListComponent implements OnInit {
  private postsService = inject(PostsService);
  private router = inject(Router);
  private loading = inject(LoadingService);
  private toast = inject(ToastService);
  private confirm = inject(ConfirmService);
  posts$!: Observable<Post[]>;

  ngOnInit(): void {
    this.load();
  }

  private load(): void {
    this.loading.show();
    this.posts$ = this.postsService.list(10);
    // hide when observable emits - simple subscription
    this.posts$.subscribe({ next: () => this.loading.hide(), error: () => this.loading.hide() });
  }

  view(id: number): void {
    this.router.navigate([`/posts/${id}`]);
  }

  edit(id: number): void {
    this.router.navigate([`/posts/${id}/edit`]);
  }

  create(): void {
    this.router.navigate(['/posts/new']);
  }

  async delete(id: number): Promise<void> {
    const ok = await this.confirm.confirm('¿Eliminar publicación? Esta acción es irreversible (simulada).');
    if (!ok) return;
    this.loading.show();
    this.postsService.delete(id).subscribe({
      next: () => {
        this.loading.hide();
        this.toast.success('Publicación eliminada (simulado)');
        this.load();
      },
      error: () => {
        this.loading.hide();
        this.toast.error('Error al eliminar la publicación');
      }
    });
  }
}
