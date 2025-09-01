import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { AboutComponent } from './about'; // مسار صحيح

const routes: Routes = [
  { path: '', component: AboutComponent }
];

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    AboutComponent
  ]
})
export class AboutModule {}
