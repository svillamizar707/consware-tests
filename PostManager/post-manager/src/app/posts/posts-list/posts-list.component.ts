import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostsService } from '../../services/posts.service';
import { Post } from '../../models/post.model';
import { Observable } from 'rxjs';
import { Router } from '@angular/router';

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
  posts$!: Observable<Post[]>;

  ngOnInit(): void {
    this.posts$ = this.postsService.list(10);
  }

  view(id: number): void {
    this.router.navigate([`/posts/${id}`]);
  }

  edit(id: number): void {
    this.router.navigate([`/posts/${id}/edit`]);
  }

  delete(id: number): void {
    const ok = window.confirm('¿Eliminar publicación? Esta acción es irreversible (simulada).');
    if (!ok) return;
    this.postsService.delete(id).subscribe(() => {
      // refrescar la lista
      this.posts$ = this.postsService.list(10);
      alert('Publicación eliminada (simulado).');
    });
  }
}
