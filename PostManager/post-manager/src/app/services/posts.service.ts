
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';
import { Post } from '../models/post.model';

@Injectable({ providedIn: 'root' })
export class PostsService {
  private readonly API = 'https://jsonplaceholder.typicode.com/posts';

  constructor(private http: HttpClient) {}

  list(limit = 10): Observable<Post[]> {
    const params = new HttpParams().set('_limit', String(limit));
    return this.http.get<Post[]>(this.API, { params }).pipe(catchError(() => of([])));
  }

  get(id: number): Observable<Post> {
    return this.http.get<Post>(`${this.API}/${id}`);
  }

  create(post: Omit<Post, 'id'>): Observable<Post> {
    return this.http.post<Post>(this.API, post).pipe(catchError(() => of({ ...post, id: -1 } as Post)));
  }

  update(id: number, post: Partial<Omit<Post, 'id'>>): Observable<Post> {
    return this.http.put<Post>(`${this.API}/${id}`, post).pipe(catchError(() => of({ id, ...(post as object) } as Post)));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.API}/${id}`).pipe(catchError(() => of(void 0)));
  }
}
