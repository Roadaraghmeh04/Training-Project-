import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrls: ['./register.scss'],
  standalone: false
})
export class RegisterComponent implements OnInit {
  registerForm!: FormGroup;
  errorMessage = '';
  isLoading = false;
  hidePassword = true;
  hideConfirmPassword = true;

  constructor(private fb: FormBuilder, private router: Router, private authService: AuthService) {}

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordsMatchValidator });
  }

  // التحقق إنو الباسورد والكونفرم نفس الشي
  passwordsMatchValidator(group: AbstractControl): ValidationErrors | null {
    const password = group.get('password')?.value;
    const confirm = group.get('confirmPassword')?.value;
    return password === confirm ? null : { notMatching: true };
  }

  togglePasswordVisibility(field: 'password' | 'confirm') {
    if (field === 'password') {
      this.hidePassword = !this.hidePassword;
    } else {
      this.hideConfirmPassword = !this.hideConfirmPassword;
    }
  }

  onSubmit() {
    if (this.registerForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      const { username, email, password } = this.registerForm.value;

      this.authService.register(username, email, password).subscribe({
        next: () => {
          this.authService.login(email, password).subscribe({
            next: () => {
              this.router.navigate(['/dashboard']);
              this.isLoading = false;
            },
            error: (err) => {
              this.errorMessage = err.error?.message || 'Login after registration failed';
              this.isLoading = false;
            }
          });
        },
        error: (err) => {
          this.errorMessage = err.error?.message || 'Registration failed';
          this.isLoading = false;
        }
      });
    }
  }
}
