import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostsService } from '../../services/posts.service';
import { Post } from '../../models/post.model';
import { ActivatedRoute, Router } from '@angular/router';
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

  post$!: Observable<Post>;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.post$ = this.postsService.get(id);
  }

  back(): void {
    this.router.navigate(['/posts']);
  }
}
