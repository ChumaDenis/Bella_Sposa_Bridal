import { Component, ChangeDetectionStrategy, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent {
  private router = inject(Router);
  readonly currentYear = new Date().getFullYear();

  navTo(section: string) {
    if (this.router.url === '/') {
      document.getElementById(section)?.scrollIntoView({ behavior: 'smooth' });
    } else {
      this.router.navigate(['/']).then(() => {
        setTimeout(() => {
          document.getElementById(section)?.scrollIntoView({ behavior: 'smooth' });
        }, 100);
      });
    }
  }
}
