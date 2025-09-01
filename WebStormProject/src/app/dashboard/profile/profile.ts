import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.html',
  styleUrls: ['./profile.scss'],
  standalone: false
})
export class ProfileComponent implements OnInit {
  user: any = {};
  errorMessage = '';

  constructor(private http: HttpClient, private authService: AuthService) {}

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    const token = this.authService.getToken();
    if (!token) {
      this.errorMessage = 'You must be logged in to view your profile';
      return;
    }

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    this.http.get('https://localhost:7091/api/User/profile', { headers }).subscribe({
      next: (res: any) => {
        this.user = res;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Failed to load profile';
      }
    });
  }
}
