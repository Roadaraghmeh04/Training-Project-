import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  standalone: false
})
export class LoginComponent {
  loginForm: FormGroup;
  hidePassword = true;
  isLoading = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private authService: AuthService
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
  }
  togglePasswordVisibility() {
    this.hidePassword = !this.hidePassword;
  }
  onSubmit() {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';

      const {email, password} = this.loginForm.value;

      this.authService.login(email, password).subscribe({
        next: (res) => {
          // JWT تم تخزينه تلقائيًا
          this.router.navigate(['/dashboard']).then(() => {
            this.isLoading = false;
          });
        },
        error: (err) => {
          if (err.status === 401) {
            // رسالة واضحة للمستخدم لما الحساب مش موجود أو كلمة السر غلط
            this.errorMessage = 'Invalid email or password / Account does not exist';
          } else {
            this.errorMessage = 'Something went wrong. Please try again.';
          }
          this.isLoading = false;
        }

      });
    }
  }
}
