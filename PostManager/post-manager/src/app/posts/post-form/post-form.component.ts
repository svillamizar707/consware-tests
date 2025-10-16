import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PostsService } from '../../services/posts.service';
import { Post } from '../../models/post.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-post-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './post-form.component.html',
  styleUrls: ['./post-form.component.scss']
})
export class PostFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private postsService = inject(PostsService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  form = this.fb.group({
    userId: [0 as number, [Validators.required]],
    title: ['', [Validators.required, Validators.minLength(3)]],
    body: ['', [Validators.required]]
  });

  editingId: number | null = null;

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.editingId = Number(idParam);
      this.postsService.get(this.editingId).subscribe(post => {
        this.form.patchValue({ userId: Number(post.userId), title: post.title, body: post.body });
      });
    }
  }

  submit(): void {
    if (this.form.invalid) return;
    const raw = this.form.value;
    const payload: Omit<Post, 'id'> = {
      userId: Number(raw.userId),
      title: String(raw.title),
      body: String(raw.body)
    };
    if (this.editingId) {
      this.postsService.update(this.editingId, payload).subscribe(() => {
        // mostrar mensaje y navegar
        this.router.navigate(['/posts']);
      });
    } else {
      this.postsService.create(payload).subscribe(() => {
        this.router.navigate(['/posts']);
      });
    }
  }
}
