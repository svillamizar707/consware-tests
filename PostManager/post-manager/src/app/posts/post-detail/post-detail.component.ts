import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostsService } from '../../services/posts.service';
import { Post } from '../../models/post.model';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingService } from '../../shared/loading.service';
import { ToastService } from '../../shared/toast.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-post-detail',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './post-detail.component.html',
  styleUrls: ['./post-detail.component.scss']
})
export class PostDetailComponent implements OnInit {
  private postsService = inject(PostsService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private loading = inject(LoadingService);
  private toast = inject(ToastService);

  post$!: Observable<Post>;

  ngOnInit(): void {
  const id = Number(this.route.snapshot.paramMap.get('id'));
  this.loading.show();
  this.post$ = this.postsService.get(id);
  this.post$.subscribe({ next: () => this.loading.hide(), error: () => { this.loading.hide(); this.toast.error('Error cargando la publicación'); } });
  }

  back(): void {
    this.router.navigate(['/posts']);
  }
}
