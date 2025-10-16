import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { PostsService } from '../../services/posts.service';
import { Post } from '../../models/post.model';
import { ActivatedRoute, Router } from '@angular/router';
import { LoadingService } from '../../shared/loading.service';
import { ToastService } from '../../shared/toast.service';

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
  private loading = inject(LoadingService);
  private toast = inject(ToastService);

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
      this.loading.show();
      this.postsService.get(this.editingId).subscribe({ next: post => {
        this.form.patchValue({ userId: Number(post.userId), title: post.title, body: post.body });
        this.loading.hide();
      }, error: () => { this.loading.hide(); this.toast.error('Error cargando la publicación'); } });
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
    this.loading.show();
    if (this.editingId) {
      this.postsService.update(this.editingId, payload).subscribe({ next: () => {
        this.loading.hide();
        this.toast.success('Publicación actualizada');
        this.router.navigate(['/posts']);
      }, error: () => { this.loading.hide(); this.toast.error('Error actualizando'); } });
    } else {
      this.postsService.create(payload).subscribe({ next: () => {
        this.loading.hide();
        this.toast.success('Publicación creada');
        this.router.navigate(['/posts']);
      }, error: () => { this.loading.hide(); this.toast.error('Error creando'); } });
    }
  }
}
