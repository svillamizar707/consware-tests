import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PostsService } from '../../services/posts.service';
import { Post } from '../../models/post.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-posts-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './posts-list.component.html',
  styleUrls: ['./posts-list.component.scss']
})
export class PostsListComponent implements OnInit {
  private postsService = inject(PostsService);
  posts$!: Observable<Post[]>;

  ngOnInit(): void {
    this.posts$ = this.postsService.list(10);
  }
}
