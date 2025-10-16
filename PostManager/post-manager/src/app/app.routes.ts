import { Routes } from '@angular/router';

import { PostsListComponent } from './posts/posts-list/posts-list.component';
import { PostFormComponent } from './posts/post-form/post-form.component';

export const routes: Routes = [
	{ path: '', redirectTo: 'posts', pathMatch: 'full' },
	{ path: 'posts', component: PostsListComponent },
	{ path: 'posts/new', component: PostFormComponent },
	{ path: 'posts/:id/edit', component: PostFormComponent }
];
