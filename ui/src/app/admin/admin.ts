import {
  Component, ChangeDetectionStrategy, OnInit, inject,
  signal, computed, ChangeDetectorRef
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AdminService, AddPhotoPayload } from '../core/services/admin.service';
import { DressListDto, DressDetailDto, SILHOUETTE_LABELS } from '../core/models/dress.model';

const PHOTO_TYPES = [
  { value: 9, label: 'Hero' },
  { value: 0, label: 'Front' },
  { value: 1, label: 'Back' },
  { value: 2, label: 'Fabric Detail' },
  { value: 3, label: 'Corset' },
  { value: 4, label: 'Train' },
  { value: 5, label: 'Sleeves' },
  { value: 6, label: 'In Motion' },
  { value: 7, label: 'Close-Up' },
  { value: 8, label: 'Mobile Vertical' },
];

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminComponent implements OnInit {
  private adminService = inject(AdminService);
  private cdr         = inject(ChangeDetectorRef);

  dresses      = signal<DressListDto[]>([]);
  selected     = signal<DressDetailDto | null>(null);
  loading      = signal(true);
  detailLoading = signal(false);
  uploading    = signal(false);
  uploadError  = signal<string | null>(null);
  newPhotoType = signal(9); // Hero default
  dragOver     = signal(false);

  readonly photoTypes = PHOTO_TYPES;
  readonly silhouetteLabels = SILHOUETTE_LABELS;

  ngOnInit() {
    this.adminService.getAllDresses().subscribe({
      next: d => { this.dresses.set(d); this.loading.set(false); this.cdr.markForCheck(); },
      error: () => { this.loading.set(false); this.cdr.markForCheck(); }
    });
  }

  selectDress(id: string) {
    this.detailLoading.set(true);
    this.selected.set(null);
    this.uploadError.set(null);
    this.adminService.getDress(id).subscribe({
      next: d => { this.selected.set(d); this.detailLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.detailLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    this.dragOver.set(false);
    const files = event.dataTransfer?.files;
    if (files?.length) this.processFiles(Array.from(files));
  }

  onDragOver(event: DragEvent) { event.preventDefault(); this.dragOver.set(true); }
  onDragLeave() { this.dragOver.set(false); }

  onFileInput(event: Event) {
    const files = (event.target as HTMLInputElement).files;
    if (files?.length) this.processFiles(Array.from(files));
    (event.target as HTMLInputElement).value = '';
  }

  private processFiles(files: File[]) {
    const dress = this.selected();
    if (!dress) return;
    this.uploadError.set(null);

    const imageFiles = files.filter(f => f.type.startsWith('image/'));
    if (!imageFiles.length) { this.uploadError.set('Please select image files only.'); return; }

    this.uploading.set(true);
    let completed = 0;

    imageFiles.forEach((file, i) => {
      this.adminService.uploadPhoto(file).subscribe({
        next: ({ url }) => {
          const existingCount = this.selected()?.photos.length ?? 0;
          const payload: AddPhotoPayload = {
            url,
            altText: dress.name,
            type: this.newPhotoType(),
            order: existingCount + i + 1
          };
          this.adminService.addPhoto(dress.id, payload).subscribe({
            next: () => {
              completed++;
              if (completed === imageFiles.length) {
                this.uploading.set(false);
                this.refreshSelected();
              }
            },
            error: () => {
              this.uploadError.set('Photo uploaded but failed to save. Try again.');
              this.uploading.set(false);
              this.cdr.markForCheck();
            }
          });
        },
        error: () => {
          this.uploadError.set('Upload failed. Check your connection and try again.');
          this.uploading.set(false);
          this.cdr.markForCheck();
        }
      });
    });
  }

  deletePhoto(photoId: string) {
    const dress = this.selected();
    if (!dress) return;
    this.adminService.deletePhoto(dress.id, photoId).subscribe({
      next: () => this.refreshSelected(),
      error: () => { this.uploadError.set('Failed to delete photo.'); this.cdr.markForCheck(); }
    });
  }

  toggleActive(id: string, isActive: boolean) {
    this.adminService.toggleActive(id, !isActive).subscribe({
      next: () => {
        this.dresses.update(list => list.map(d =>
          d.id === id ? { ...d, isActive: !isActive } : d
        ));
        const sel = this.selected();
        if (sel?.id === id) this.selected.update(s => s ? { ...s, isActive: !isActive } : s);
        this.cdr.markForCheck();
      }
    });
  }

  private refreshSelected() {
    const id = this.selected()?.id;
    if (!id) return;
    this.adminService.getDress(id).subscribe({
      next: d => { this.selected.set(d); this.cdr.markForCheck(); }
    });
  }

  photoTypeLabel(type: number): string {
    return PHOTO_TYPES.find(t => t.value === type)?.label ?? 'Photo';
  }

  get activeCount() { return this.dresses().filter(d => d.isActive).length; }
  get totalCount()  { return this.dresses().length; }
}
