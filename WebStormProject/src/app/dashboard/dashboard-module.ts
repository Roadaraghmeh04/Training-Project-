import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // ضروري للـ ngModel في TasksComponent
import { DashboardRoutingModule} from './dashboard-routing-module';

import { DashboardComponent } from './dashboard.component';
import { TasksComponent } from './tasks/tasks';
import { CategoriesComponent } from './categories/categories';
import { ProfileComponent } from './profile/profile';

@NgModule({
  declarations: [
    DashboardComponent,
    TasksComponent,
    CategoriesComponent,
    ProfileComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    DashboardRoutingModule
  ]
})
export class DashboardModule {}
