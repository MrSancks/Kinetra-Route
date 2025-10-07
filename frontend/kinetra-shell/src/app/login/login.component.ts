import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent {
  readonly form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  private readonly submissionAttempted = signal(false);
  readonly hasError = computed(() => this.submissionAttempted() && this.form.invalid);
  readonly invalidCredentials = signal(false);
  readonly hintCredentials = this.authService.credentials;

  constructor(
    private readonly fb: FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ) {}

  onSubmit(): void {
    this.submissionAttempted.set(true);
    this.invalidCredentials.set(false);

    if (this.form.invalid) {
      return;
    }

    const { email, password } = this.form.getRawValue();
    const success = this.authService.login(email, password);

    if (!success) {
      this.invalidCredentials.set(true);
      return;
    }

    this.router.navigate(['/dashboard']);
  }
}
