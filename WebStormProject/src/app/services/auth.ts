import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'https://localhost:7091/api/User';

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, { email, password }).pipe(
      tap((res: any) => {
        if (res.token) {
          localStorage.setItem('jwt', res.token); // ✅ لازم اسم المفتاح يكون نفس اللي يستخدمه AuthGuard و interceptor
        }
      })
    );
  }


  register(username: string, email: string, password: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, {
      Username: username,
      Email: email,
      Password: password
    });
  }

  logout(): void {
    localStorage.removeItem('jwt');
  }

  getToken(): string | null {
    return localStorage.getItem('jwt'); // ← نفس الاسم
  }


  isAuthenticated(): boolean {
    const token = this.getToken();
    return !!token;
  }

}
