import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  standalone: false
})
export class DashboardComponent {
  constructor(private router: Router) {}

  navigateTo(section: string) {
    switch(section) {
      case 'tasks':
        this.router.navigate(['/dashboard/tasks']);
        break;
      case 'categories':
        this.router.navigate(['/dashboard/categories']);
        break;
      case 'profile':
        this.router.navigate(['/dashboard/profile']);
        break;
      case 'logout':
        localStorage.removeItem('jwt');
        this.router.navigate(['/login']);
        break;
    }
  }
}
