import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';

import { App } from './app';
import { AboutComponent } from './about/about';
import { AuthInterceptor } from './interceptors/auth-interceptor'; // استدعاء الـ interceptor
import { CategoriesComponent } from './dashboard/categories/categories';
const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  {
    path: '',
    loadChildren: () => import('./auth/auth-module').then(m => m.AuthModule)
  },
  {
    path: 'dashboard',
    loadChildren: () =>
      import('./dashboard/dashboard-module').then(m => m.DashboardModule)
  },
  { path: 'about', component: AboutComponent },
  { path: 'categories', component: CategoriesComponent }
];

@NgModule({
  declarations: [
    App,

  ],
  imports: [
    BrowserModule,
    ReactiveFormsModule,
    HttpClientModule, // ✅ مهم عشان HttpClient يشتغل
    RouterModule.forRoot(routes)
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true } // إضافة الـ interceptor
  ],
  bootstrap: [App]
})
export class AppModule {}
