import { Component, ChangeDetectionStrategy, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AtlierService } from '../../core/services/atlier.service';
import { AtlierInfoDto } from '../../core/models/atlier.model';

@Component({
  selector: 'app-footer',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './footer.html',
  styleUrl: './footer.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class FooterComponent implements OnInit {
  private router = inject(Router);
  private atlierService = inject(AtlierService);
  readonly currentYear = new Date().getFullYear();

  atlier = signal<AtlierInfoDto | null>(null);

  ngOnInit() {
    this.atlierService.getInfo().subscribe({
      next: (info) => this.atlier.set(info),
      error: () => {}
    });
  }

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
