import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly authenticated = signal(false);

  readonly credentials = {
    email: 'admin@kinetra-route.com',
    password: 'kinetra123'
  } as const;

  login(email: string, password: string): boolean {
    const isValid =
      email.trim().toLowerCase() === this.credentials.email &&
      password === this.credentials.password;

    this.authenticated.set(isValid);
    return isValid;
  }

  logout(): void {
    this.authenticated.set(false);
  }

  isAuthenticated(): boolean {
    return this.authenticated();
  }
}
