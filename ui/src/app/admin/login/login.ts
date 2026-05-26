import { Component, signal, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class LoginComponent {
  private auth   = inject(AuthService);
  private router = inject(Router);

  username = '';
  password = '';
  error    = signal<string | null>(null);
  loading  = signal(false);

  submit() {
    if (!this.username || !this.password) {
      this.error.set('Please enter username and password');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    this.auth.login(this.username, this.password).subscribe({
      next: () => this.router.navigate(['/admin']),
      error: () => {
        this.error.set('Invalid username or password');
        this.loading.set(false);
      }
    });
  }

  onKeydown(event: KeyboardEvent) {
    if (event.key === 'Enter') this.submit();
  }
}
