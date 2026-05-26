import {
  Component, ChangeDetectionStrategy, OnInit, inject,
  signal, ChangeDetectorRef, ViewChild, ElementRef
} from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AdminService, AddPhotoPayload, AddVideoPayload } from '../core/services/admin.service';
import { AuthService } from '../core/services/auth.service';
import { AppointmentService } from '../core/services/appointment.service';
import { SilhouetteService } from '../core/services/silhouette.service';
import { AtlierService } from '../core/services/atlier.service';
import { AtlierInfoDto } from '../core/models/atlier.model';
import { DressListDto, DressDetailDto, NavDressItem } from '../core/models/dress.model';
import { CollectionDto, CollectionNameDto } from '../core/models/collection.model';
import { SilhouetteTypeDto } from '../core/models/silhouette.model';
import { AppointmentDto, AppointmentFileDto, AppointmentTypeConfigDto, TimeSlotDto, DayScheduleDto } from '../core/models/appointment.model';

type Tab     = 'appointments' | 'dresses' | 'collections' | 'silhouettes' | 'settings';
type Mode    = 'idle' | 'create' | 'edit';
type ApptView = 'list' | 'detail' | 'create' | 'types' | 'schedule';

const SIDEBAR_PAGE = 20;
const INNER_PAGE   = 20;

const VIDEO_TYPES = [
  { value: 0, label: 'Reel' },
  { value: 1, label: '360 View' },
  { value: 2, label: 'Behind the Scenes' },
  { value: 3, label: 'Runway' },
];

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

export const APPT_STATUSES = [
  { value: 0, label: 'Pending',   color: '#c8a96e' },
  { value: 1, label: 'Confirmed', color: '#5a9a5a' },
  { value: 2, label: 'Cancelled', color: '#b05050' },
  { value: 3, label: 'Completed', color: '#6080a0' },
];

interface DressFormData {
  name: string; slug: string; tagline: string; description: string;
  silhouette: number; material: string; corsetType: string;
  trainDescription: string; color: string; hasSleeves: boolean;
  sleeveDescription: string; decoration: string;
  customTailoringAvailable: boolean; isActive: boolean;
  collectionIds: string[];
}

interface CollectionFormData {
  name: string; season: string; year: number;
  description: string; coverImageUrl: string; coverImageUrlMobile: string; isActive: boolean;
}

interface AdminApptFormData {
  firstName: string; lastName: string; phone: string; email: string;
  date: string; time: string; typeId: number; notes: string;
}

interface TypeFormData {
  name: string; price: string; description: string; mainDescription: string; detail: string; displayOrder: number; isActive: boolean;
}

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './admin.html',
  styleUrl: './admin.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AdminComponent implements OnInit {
  @ViewChild('tabsScroll') tabsScroll!: ElementRef<HTMLElement>;

  private svc              = inject(AdminService);
  private auth             = inject(AuthService);
  private router           = inject(Router);
  private cdr              = inject(ChangeDetectorRef);
  private apptSvc          = inject(AppointmentService);
  private silhouetteService = inject(SilhouetteService);

  // ─── Tab ─────────────────────────────────────────────────────────
  tab = signal<Tab>('appointments');

  // ─── Dress state ─────────────────────────────────────────────────
  dresses           = signal<DressListDto[]>([]);
  collectionNames   = signal<CollectionNameDto[]>([]);
  selectedDress     = signal<DressDetailDto | null>(null);
  dressMode         = signal<Mode>('idle');
  dressListLoading  = signal(true);
  dressDetailLoading = signal(false);
  dressSaving       = signal(false);
  dressError        = signal<string | null>(null);

  // ─── Photo upload ────────────────────────────────────────────────
  uploading    = signal(false);
  uploadError  = signal<string | null>(null);
  newPhotoType = signal(9);
  dragOver     = signal(false);

  // ─── Photo reorder ───────────────────────────────────────────────
  dragPhotoIdx = signal<number | null>(null);
  dragOverIdx  = signal<number | null>(null);

  // ─── Video upload ────────────────────────────────────────────────
  uploadingVideo   = signal(false);
  videoUploadError = signal<string | null>(null);
  newVideoType     = signal(0);
  readonly videoTypes = VIDEO_TYPES;

  // ─── Soft-delete filters ──────────────────────────────────────────
  showDeletedDresses     = signal(false);
  showDeletedCollections = signal(false);

  // ─── Collection state ─────────────────────────────────────────────
  collections           = signal<CollectionDto[]>([]);
  selectedCollection    = signal<CollectionDto | null>(null);
  collectionMode        = signal<Mode>('idle');
  collectionListLoading = signal(true);
  collectionSaving      = signal(false);
  collectionError       = signal<string | null>(null);
  coverUploading        = signal(false);
  coverMobileUploading  = signal(false);

  // ─── Nav-order state ──────────────────────────────────────────────
  navDresses        = signal<NavDressItem[]>([]);
  navDressesLoading = signal(false);

  // ─── Appointments state ───────────────────────────────────────────
  appointments       = signal<AppointmentDto[]>([]);
  selectedAppt       = signal<AppointmentDto | null>(null);
  apptView           = signal<ApptView>('list');
  apptLoading        = signal(true);
  apptSaving         = signal(false);
  apptError          = signal<string | null>(null);
  apptStatusFilter   = signal<string>('all');
  apptDateFilter     = signal<string>('');
  apptSearch         = signal<string>('');

  apptForm: AdminApptFormData = this.blankApptForm();

  // Reschedule
  rescheduleDate        = signal('');
  rescheduleTime        = signal('');
  rescheduling          = signal(false);
  showReschedule        = signal(false);
  rescheduleSlots       = signal<string[]>([]);
  rescheduleSlotsLoading = signal(false);

  // Admin notes
  editingAdminNotes = signal(false);
  adminNotesDraft   = signal('');
  adminNotesSaving  = signal(false);

  // File attachments
  fileUploading   = signal(false);
  fileUploadError = signal<string | null>(null);

  // ─── Appointment types ────────────────────────────────────────────
  appointmentTypes  = signal<AppointmentTypeConfigDto[]>([]);
  typeMode          = signal<Mode>('idle');
  editingTypeId     = signal<number | null>(null);
  typeForm: TypeFormData = this.blankTypeForm();
  typeSaving        = signal(false);
  typeError         = signal<string | null>(null);

  // ─── Schedule ─────────────────────────────────────────────────────
  timeSlots        = signal<TimeSlotDto[]>([]);
  editingSlots     = signal<string[]>([]);
  slotsSaving      = signal(false);
  newSlotTime      = signal('');

  dayOverrideDate  = signal('');
  dayOverride      = signal<DayScheduleDto | null>(null);
  dayLoading       = signal(false);
  daySaving        = signal(false);
  dayIsClosed      = signal(false);
  dayEnabledSlots  = signal<string[]>([]);
  dayHasOverride   = signal(false);

  // ─── Lazy-load flags ─────────────────────────────────────────────
  private dressesLoaded = false;
  private collectionsLoaded = false;
  private collectionNamesLoaded = false;

  // ─── Infinite-scroll display counts ──────────────────────────────
  dressDisplayCount = signal(SIDEBAR_PAGE);
  collDisplayCount  = signal(SIDEBAR_PAGE);
  navDisplayCount   = signal(INNER_PAGE);
  apptDisplayCount  = signal(SIDEBAR_PAGE);

  get visibleDresses()      { return this.dresses().slice(0, this.dressDisplayCount()); }
  get hasMoreDresses()      { return this.dresses().length > this.dressDisplayCount(); }
  get visibleCollections()  { return this.collections().slice(0, this.collDisplayCount()); }
  get hasMoreCollections()  { return this.collections().length > this.collDisplayCount(); }
  get visibleNavDresses()   { return this.navDresses().slice(0, this.navDisplayCount()); }
  get hasMoreNavDresses()   { return this.navDresses().length > this.navDisplayCount(); }
  get filteredAppointments() {
    let list = this.appointments();
    const sf = this.apptStatusFilter();
    const df = this.apptDateFilter();
    const q  = this.apptSearch().trim().toLowerCase();
    if (sf !== 'all') list = list.filter(a => a.status.toLowerCase() === sf);
    if (df) list = list.filter(a => a.appointmentDateTime.startsWith(df));
    if (q) list = list.filter(a =>
      `${a.firstName} ${a.lastName}`.toLowerCase().includes(q) ||
      a.phone.toLowerCase().includes(q) ||
      (a.email ?? '').toLowerCase().includes(q)
    );
    return [...list].sort((a, b) => {
      const aDate = new Date(a.appointmentDateTime);
      const bDate = new Date(b.appointmentDateTime);
      const aIsPendingCallback = aDate.getUTCFullYear() >= 2099 && a.status === 'Pending';
      const bIsPendingCallback = bDate.getUTCFullYear() >= 2099 && b.status === 'Pending';
      if (aIsPendingCallback !== bIsPendingCallback) return aIsPendingCallback ? -1 : 1;
      if (aIsPendingCallback) return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      const diff = aDate.getTime() - bDate.getTime();
      return diff !== 0 ? diff : new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    });
  }
  get visibleAppointments() { return this.filteredAppointments.slice(0, this.apptDisplayCount()); }
  get hasMoreAppointments() { return this.filteredAppointments.length > this.apptDisplayCount(); }

  // ─── Form data ───────────────────────────────────────────────────
  dressForm: DressFormData           = this.blankDressForm();
  collectionForm: CollectionFormData = this.blankCollectionForm();

  // ─── Silhouettes state ────────────────────────────────────────────
  silhouettes           = signal<SilhouetteTypeDto[]>([]);
  silhouettesLoading    = signal(false);
  silhouetteNewName     = signal('');
  silhouetteSaving      = signal(false);
  silhouetteError       = signal<string | null>(null);
  silhouetteDeleteErr   = signal<string | null>(null);
  silhouetteEditingId   = signal<number | null>(null);
  silhouetteEditName    = signal('');
  silhouetteRenaming    = signal(false);
  silhouetteRenameError = signal<string | null>(null);
  dragSilhouetteIdx     = signal<number | null>(null);
  dragOverSilhouetteIdx = signal<number | null>(null);
  private silhouettesLoaded = false;

  // ─── Settings ────────────────────────────────────────────────────
  private atlierService     = inject(AtlierService);
  private atlierInfo: AtlierInfoDto | null = null;
  settingsForm              = { heroVideoDesktop: '', heroVideoMobile: '' };
  settingsSaving            = signal(false);
  settingsVideoUploading    = signal(false);
  settingsError             = signal<string | null>(null);
  private settingsLoaded    = false;

  // ─── Constants ───────────────────────────────────────────────────
  readonly photoTypes   = PHOTO_TYPES;
  readonly apptStatuses = APPT_STATUSES;

  get silhouetteOptions() {
    return this.silhouettes().map(s => ({ value: s.id, label: s.name }));
  }

  // ─── Date helpers ─────────────────────────────────────────────────
  private localDateStr(d: Date): string {
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
  }
  get todayStr(): string { return this.localDateStr(new Date()); }
  get adminMaxDate(): string { const d = new Date(); d.setMonth(d.getMonth() + 3); return this.localDateStr(d); }

  // ─── Computed ─────────────────────────────────────────────────────
  get activeDressCount()  { return this.dresses().filter(d => d.isActive).length; }
  get totalDressCount()   { return this.dresses().length; }
  get activeCollCount()   { return this.collections().filter(c => c.isActive).length; }
  get totalCollCount()    { return this.collections().length; }
  get totalApptCount()    { return this.appointments().length; }
  get pendingApptCount()  { return this.appointments().filter(a => a.status === 'Pending').length; }

  // ─── Lifecycle ────────────────────────────────────────────────────
  ngOnInit() {
    this.loadAppointments();
    this.loadAppointmentTypes();
  }

  // ─── Tab ──────────────────────────────────────────────────────────
  scrollTabs(dir: number) {
    this.tabsScroll?.nativeElement.scrollBy({ left: dir * 120, behavior: 'smooth' });
  }

  setTab(t: Tab) {
    this.tab.set(t);
    this.dressMode.set('idle');
    this.collectionMode.set('idle');
    if (t === 'appointments') this.apptView.set('list');
    if (t === 'dresses' && !this.dressesLoaded) { this.dressesLoaded = true; this.loadDresses(); }
    if (t === 'dresses' && !this.collectionNamesLoaded) { this.collectionNamesLoaded = true; this.loadCollectionNames(); }
    if (t === 'dresses' && !this.silhouettesLoaded) { this.silhouettesLoaded = true; this.loadSilhouettes(); }
    if (t === 'collections' && !this.collectionsLoaded) { this.collectionsLoaded = true; this.loadCollections(); }
    if (t === 'silhouettes' && !this.silhouettesLoaded) { this.silhouettesLoaded = true; this.loadSilhouettes(); }
    if (t === 'settings' && !this.settingsLoaded) { this.settingsLoaded = true; this.loadSettings(); }
  }

  // ─── Auth ─────────────────────────────────────────────────────────
  logout() {
    this.auth.logout();
    this.router.navigate(['/admin/login']);
  }

  // ═══════════════════════════════════════════════════════════════════
  //  APPOINTMENTS
  // ═══════════════════════════════════════════════════════════════════

  loadAppointments() {
    this.apptDisplayCount.set(SIDEBAR_PAGE);
    this.svc.getAllAppointments().subscribe({
      next: list => { this.appointments.set(list); this.apptLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.apptLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  loadAppointmentTypes() {
    this.svc.getAppointmentTypes().subscribe({
      next: t => { this.appointmentTypes.set(t); this.cdr.markForCheck(); }
    });
  }

  selectAppt(appt: AppointmentDto) {
    this.selectedAppt.set(appt);
    this.apptView.set('detail');
    this.showReschedule.set(false);
    this.editingAdminNotes.set(false);
    this.fileUploadError.set(null);
    this.apptError.set(null);
    this.cdr.markForCheck();
  }

  setApptView(v: ApptView) {
    this.apptView.set(v);
    this.apptError.set(null);
    if (v === 'schedule') this.loadTimeSlots();
    this.cdr.markForCheck();
  }

  // ─── Status update ────────────────────────────────────────────────
  updateApptStatus(appt: AppointmentDto, statusValue: number) {
    this.svc.updateAppointmentStatus(appt.id, statusValue).subscribe({
      next: () => {
        const statusLabel = APPT_STATUSES.find(s => s.value === statusValue)?.label ?? String(statusValue);
        this.appointments.update(list => list.map(a => a.id === appt.id ? { ...a, status: statusLabel } : a));
        if (this.selectedAppt()?.id === appt.id)
          this.selectedAppt.update(a => a ? { ...a, status: statusLabel } : a);
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Delete appointment ───────────────────────────────────────────
  deleteAppt(id: string) {
    if (!confirm('Delete this appointment?')) return;
    this.svc.deleteAppointment(id).subscribe({
      next: () => {
        this.appointments.update(list => list.filter(a => a.id !== id));
        if (this.selectedAppt()?.id === id) { this.selectedAppt.set(null); this.apptView.set('list'); }
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Reschedule ───────────────────────────────────────────────────
  openReschedule(appt: AppointmentDto) {
    const dt = new Date(appt.appointmentDateTime);
    const isCallback = dt.getUTCFullYear() >= 2099;
    const timeStr = isCallback ? '10:00' : dt.toISOString().split('T')[1].substring(0, 5);
    this.rescheduleTime.set(timeStr);
    this.showReschedule.set(true);
    this.onRescheduleDateChange(isCallback ? this.todayStr : dt.toISOString().split('T')[0]);
    this.cdr.markForCheck();
  }

  onRescheduleDateChange(date: string) {
    this.rescheduleDate.set(date);
    this.rescheduleSlots.set([]);
    if (!date) return;
    this.rescheduleSlotsLoading.set(true);
    this.apptSvc.getAvailableSlots(date).subscribe({
      next: slots => { this.rescheduleSlots.set(slots); this.rescheduleSlotsLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.rescheduleSlotsLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  cancelReschedule() { this.showReschedule.set(false); this.rescheduleSlots.set([]); this.cdr.markForCheck(); }

  // ─── Admin notes ─────────────────────────────────────────────────
  startEditAdminNotes() {
    this.adminNotesDraft.set(this.selectedAppt()?.adminNotes ?? '');
    this.editingAdminNotes.set(true);
    this.cdr.markForCheck();
  }

  cancelEditAdminNotes() {
    this.editingAdminNotes.set(false);
    this.cdr.markForCheck();
  }

  saveAdminNotes() {
    const appt = this.selectedAppt();
    if (!appt) return;
    this.adminNotesSaving.set(true);
    const notes = this.adminNotesDraft().trim() || null;
    this.svc.updateAdminNotes(appt.id, notes).subscribe({
      next: () => {
        this.appointments.update(list => list.map(a => a.id === appt.id ? { ...a, adminNotes: notes } : a));
        this.selectedAppt.update(a => a ? { ...a, adminNotes: notes } : a);
        this.adminNotesSaving.set(false);
        this.editingAdminNotes.set(false);
        this.cdr.markForCheck();
      },
      error: () => { this.adminNotesSaving.set(false); this.cdr.markForCheck(); }
    });
  }

  // ─── File attachments ────────────────────────────────────────────
  onApptFileInput(e: Event) {
    const files = (e.target as HTMLInputElement).files;
    if (!files?.length) return;
    this.uploadApptFiles(Array.from(files));
    (e.target as HTMLInputElement).value = '';
  }

  onApptFileDrop(e: DragEvent) {
    e.preventDefault();
    this.dragOver.set(false);
    const files = e.dataTransfer?.files;
    if (files?.length) this.uploadApptFiles(Array.from(files));
  }

  private uploadApptFiles(files: File[]) {
    const appt = this.selectedAppt();
    if (!appt) return;
    this.fileUploadError.set(null);
    this.fileUploading.set(true);
    let done = 0;
    const uploadedFiles: AppointmentFileDto[] = [];
    files.forEach(file => {
      this.svc.uploadAppointmentFile(appt.id, file).subscribe({
        next: (dto) => {
          uploadedFiles.push(dto);
          done++;
          if (done === files.length) {
            this.fileUploading.set(false);
            this.selectedAppt.update(a => a ? { ...a, files: [...(a.files ?? []), ...uploadedFiles] } : a);
            this.appointments.update(list => list.map(a =>
              a.id === appt.id ? { ...a, files: [...(a.files ?? []), ...uploadedFiles] } : a
            ));
            this.cdr.markForCheck();
          }
        },
        error: (err) => {
          done++;
          this.fileUploadError.set(err.error?.message ?? 'Upload failed');
          this.fileUploading.set(false);
          this.cdr.markForCheck();
        }
      });
    });
  }

  deleteApptFile(fileId: string) {
    const appt = this.selectedAppt();
    if (!appt) return;
    this.svc.deleteAppointmentFile(appt.id, fileId).subscribe({
      next: () => {
        this.selectedAppt.update(a => a ? { ...a, files: a.files.filter(f => f.id !== fileId) } : a);
        this.appointments.update(list => list.map(a =>
          a.id === appt.id ? { ...a, files: a.files.filter(f => f.id !== fileId) } : a
        ));
        this.cdr.markForCheck();
      }
    });
  }

  fileIcon(contentType: string): string {
    if (contentType.startsWith('image/')) return '🖼';
    if (contentType === 'application/pdf') return '📄';
    if (contentType.includes('word') || contentType.includes('document')) return '📝';
    if (contentType.includes('sheet') || contentType.includes('excel') || contentType.includes('csv')) return '📊';
    return '📎';
  }

  formatFileSize(bytes: number): string {
    if (bytes < 1024) return `${bytes} B`;
    if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
    return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
  }

  saveReschedule() {
    const appt = this.selectedAppt();
    if (!appt) return;
    const dt = `${this.rescheduleDate()}T${this.rescheduleTime()}:00Z`;
    const wasCallbackPending = new Date(appt.appointmentDateTime).getFullYear() >= 2099 && appt.status === 'Pending';
    this.rescheduling.set(true);
    this.svc.rescheduleAppointment(appt.id, dt).subscribe({
      next: () => {
        this.appointments.update(list => list.map(a => a.id === appt.id ? { ...a, appointmentDateTime: dt } : a));
        this.selectedAppt.update(a => a ? { ...a, appointmentDateTime: dt } : a);
        if (wasCallbackPending) this.updateApptStatus(appt, 1);
        this.rescheduling.set(false);
        this.showReschedule.set(false);
        this.cdr.markForCheck();
      },
      error: () => { this.rescheduling.set(false); this.cdr.markForCheck(); }
    });
  }

  // ─── Admin create appointment ─────────────────────────────────────
  startCreateAppt() {
    this.apptForm = this.blankApptForm();
    this.apptError.set(null);
    this.apptView.set('create');
    this.cdr.markForCheck();
  }

  cancelApptForm() { this.apptView.set('list'); this.apptError.set(null); this.cdr.markForCheck(); }

  saveAppt() {
    if (!this.apptForm.firstName.trim() || !this.apptForm.phone.trim() || !this.apptForm.date || !this.apptForm.time) {
      this.apptError.set('First name, phone, date and time are required.');
      return;
    }
    this.apptSaving.set(true);
    this.apptError.set(null);
    const dto = {
      firstName: this.apptForm.firstName,
      lastName: this.apptForm.lastName,
      phone: this.apptForm.phone,
      email: this.apptForm.email || null,
      appointmentDateTime: `${this.apptForm.date}T${this.apptForm.time}:00Z`,
      type: this.apptForm.typeId,
      notes: this.apptForm.notes || null,
      viewedDressIds: []
    };
    this.svc.createAppointment(dto).subscribe({
      next: (created: AppointmentDto) => {
        this.appointments.update(list => [created, ...list]);
        this.apptSaving.set(false);
        this.apptView.set('detail');
        this.selectedAppt.set(created);
        this.cdr.markForCheck();
      },
      error: (err: HttpErrorResponse) => {
        const msg = err.error?.message ?? 'Failed to create appointment';
        this.apptError.set(msg);
        this.apptSaving.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Appointment types CRUD ───────────────────────────────────────
  startCreateType() {
    this.typeForm = this.blankTypeForm();
    this.typeMode.set('create');
    this.editingTypeId.set(null);
    this.typeError.set(null);
    this.cdr.markForCheck();
  }

  startEditType(t: AppointmentTypeConfigDto) {
    this.typeForm = { name: t.name, price: t.price != null ? String(t.price) : '', description: t.description ?? '', mainDescription: t.mainDescription ?? '', detail: t.detail ?? '', displayOrder: t.displayOrder, isActive: t.isActive };
    this.typeMode.set('edit');
    this.editingTypeId.set(t.id);
    this.typeError.set(null);
    this.cdr.markForCheck();
  }

  cancelTypeForm() { this.typeMode.set('idle'); this.typeError.set(null); this.cdr.markForCheck(); }

  saveType() {
    if (!this.typeForm.name.trim()) { this.typeError.set('Name is required'); return; }
    this.typeSaving.set(true);
    this.typeError.set(null);
    const dto = {
      name: this.typeForm.name,
      price: this.typeForm.price ? parseFloat(this.typeForm.price) : null,
      description: this.typeForm.description || null,
      mainDescription: this.typeForm.mainDescription || null,
      detail: this.typeForm.detail || null,
      displayOrder: this.typeForm.displayOrder,
      isActive: this.typeForm.isActive
    };
    const onDone = () => {
      this.typeSaving.set(false);
      this.typeMode.set('idle');
      this.loadAppointmentTypes();
      this.cdr.markForCheck();
    };
    const onError = () => {
      this.typeError.set('Failed to save appointment type.');
      this.typeSaving.set(false);
      this.cdr.markForCheck();
    };
    if (this.typeMode() === 'edit') {
      this.svc.updateAppointmentType(this.editingTypeId()!, dto).subscribe({ next: onDone, error: onError });
    } else {
      this.svc.createAppointmentType(dto).subscribe({ next: onDone, error: onError });
    }
  }

  deleteType(id: number) {
    if (!confirm('Delete this appointment type? Existing appointments with this type will still show their type ID.')) return;
    this.svc.deleteAppointmentType(id).subscribe({
      next: () => { this.loadAppointmentTypes(); this.cdr.markForCheck(); }
    });
  }

  // ─── Schedule – time slots ────────────────────────────────────────
  loadTimeSlots() {
    this.svc.getTimeSlots().subscribe({
      next: slots => {
        this.timeSlots.set(slots);
        this.editingSlots.set(slots.map(s => s.time));
        this.cdr.markForCheck();
      }
    });
  }

  addSlot() {
    const t = this.newSlotTime();
    if (!t || this.editingSlots().includes(t)) return;
    this.editingSlots.update(s => [...s, t].sort());
    this.newSlotTime.set('');
    this.cdr.markForCheck();
  }

  removeSlot(time: string) {
    this.editingSlots.update(s => s.filter(t => t !== time));
    this.cdr.markForCheck();
  }

  saveSlots() {
    this.slotsSaving.set(true);
    this.svc.replaceTimeSlots(this.editingSlots()).subscribe({
      next: () => { this.slotsSaving.set(false); this.loadTimeSlots(); this.cdr.markForCheck(); },
      error: () => { this.slotsSaving.set(false); this.cdr.markForCheck(); }
    });
  }

  // ─── Schedule – day override ──────────────────────────────────────
  loadDayOverride() {
    const date = this.dayOverrideDate();
    if (!date) return;
    this.dayLoading.set(true);
    this.svc.getDaySchedule(date).subscribe({
      next: ds => {
        this.dayOverride.set(ds);
        this.dayHasOverride.set(true);
        this.dayIsClosed.set(ds.isClosed);
        const slots = ds.customSlots?.length ? ds.customSlots : this.timeSlots().map(s => s.time);
        this.dayEnabledSlots.set(slots);
        this.dayLoading.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.dayOverride.set(null);
        this.dayHasOverride.set(false);
        this.dayIsClosed.set(false);
        this.dayEnabledSlots.set(this.timeSlots().map(s => s.time));
        this.dayLoading.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  toggleDaySlot(time: string) {
    this.dayEnabledSlots.update(s =>
      s.includes(time) ? s.filter(t => t !== time) : [...s, time].sort()
    );
    this.cdr.markForCheck();
  }

  isDaySlotEnabled(time: string): boolean {
    return this.dayEnabledSlots().includes(time);
  }

  saveDayOverride() {
    const date = this.dayOverrideDate();
    if (!date) return;
    this.daySaving.set(true);
    const customSlots = this.dayIsClosed() ? null : this.dayEnabledSlots();
    this.svc.setDaySchedule(date, this.dayIsClosed(), customSlots).subscribe({
      next: () => {
        this.daySaving.set(false);
        this.dayHasOverride.set(true);
        this.cdr.markForCheck();
      },
      error: () => { this.daySaving.set(false); this.cdr.markForCheck(); }
    });
  }

  deleteDayOverride() {
    const date = this.dayOverrideDate();
    if (!date) return;
    this.svc.deleteDaySchedule(date).subscribe({
      next: () => {
        this.dayOverride.set(null);
        this.dayHasOverride.set(false);
        this.dayIsClosed.set(false);
        this.dayEnabledSlots.set(this.timeSlots().map(s => s.time));
        this.cdr.markForCheck();
      }
    });
  }

  viewDress(dressId: string) {
    this.dressMode.set('idle');
    this.tab.set('dresses');
    if (!this.dressesLoaded) { this.dressesLoaded = true; this.loadDresses(); }
    if (!this.collectionNamesLoaded) { this.collectionNamesLoaded = true; this.loadCollectionNames(); }
    this.selectDress(dressId);
    this.cdr.markForCheck();
  }

  homepageSlotDress(slot: number): DressListDto | undefined {
    return this.dresses().find(d => d.isHomepageFeatured && d.homepageFeaturedOrder === slot);
  }

  setHomepageFeatured(dressId: string, isFeatured: boolean, slot: number) {
    this.svc.setHomepageFeatured(dressId, isFeatured, slot).subscribe({
      next: () => {
        this.dresses.update(list => list.map(d => {
          if (d.id === dressId) return { ...d, isHomepageFeatured: isFeatured, homepageFeaturedOrder: isFeatured ? slot : 0 };
          if (isFeatured && d.isHomepageFeatured && d.homepageFeaturedOrder === slot) return { ...d, isHomepageFeatured: false, homepageFeaturedOrder: 0 };
          return d;
        }));
        const sel = this.selectedDress();
        if (sel?.id === dressId) this.selectedDress.update(d => d ? { ...d, isHomepageFeatured: isFeatured, homepageFeaturedOrder: isFeatured ? slot : 0 } : d);
        this.cdr.markForCheck();
      }
    });
  }

  // ─── Helpers ──────────────────────────────────────────────────────
  apptTypeName(typeId: number): string {
    return this.appointmentTypes().find(t => t.id === typeId)?.name ?? `Type ${typeId}`;
  }

  apptStatusColor(status: string): string {
    return APPT_STATUSES.find(s => s.label === status)?.color ?? '#888';
  }

  apptStatusValue(status: string): number {
    return APPT_STATUSES.find(s => s.label === status)?.value ?? 0;
  }

  formatApptDate(iso: string): string {
    const d = new Date(iso);
    if (isNaN(d.getTime())) return iso;
    if (d.getUTCFullYear() >= 2099) return 'Callback request';
    return d.toLocaleDateString('en-GB', { day: 'numeric', month: 'short', year: 'numeric', timeZone: 'UTC' })
         + ' · ' + d.toLocaleTimeString('en-GB', { hour: '2-digit', minute: '2-digit', timeZone: 'UTC' });
  }

  onApptListScroll(e: Event) {
    const el = e.target as HTMLElement;
    if (el.scrollHeight - el.scrollTop <= el.clientHeight + 120 && this.hasMoreAppointments) {
      this.apptDisplayCount.update(n => n + SIDEBAR_PAGE);
      this.cdr.markForCheck();
    }
  }

  // ═══════════════════════════════════════════════════════════════════
  //  DRESSES
  // ═══════════════════════════════════════════════════════════════════

  loadDresses() {
    this.dressDisplayCount.set(SIDEBAR_PAGE);
    this.svc.getAllDresses(this.showDeletedDresses()).subscribe({
      next: d => { this.dresses.set(d); this.dressListLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.dressListLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  toggleShowDeletedDresses() {
    this.showDeletedDresses.update(v => !v);
    this.loadDresses();
  }

  deleteDress(id: string) {
    if (!confirm('Delete this dress? It can be restored later.')) return;
    this.svc.deleteDress(id).subscribe({
      next: () => {
        if (this.showDeletedDresses()) {
          this.dresses.update(list => list.map(d => d.id === id ? { ...d, isDeleted: true, deletedAt: new Date().toISOString() } : d));
        } else {
          this.dresses.update(list => list.filter(d => d.id !== id));
        }
        if (this.selectedDress()?.id === id) { this.selectedDress.set(null); this.dressMode.set('idle'); }
        this.cdr.markForCheck();
      }
    });
  }

  restoreDress(id: string) {
    this.svc.restoreDress(id).subscribe({
      next: () => {
        this.dresses.update(list => list.map(d => d.id === id ? { ...d, isDeleted: false, deletedAt: null } : d));
        this.cdr.markForCheck();
      }
    });
  }

  loadCollectionNames() {
    this.svc.getCollectionNames().subscribe({
      next: names => { this.collectionNames.set(names); this.cdr.markForCheck(); },
      error: () => {}
    });
  }

  // ═══════════════════════════════════════════════════════════════════
  //  SILHOUETTES
  // ═══════════════════════════════════════════════════════════════════

  loadSilhouettes() {
    this.silhouettesLoading.set(true);
    this.silhouetteService.getAll().subscribe({
      next: list => { this.silhouettes.set(list); this.silhouettesLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.silhouettesLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  addSilhouette() {
    const name = this.silhouetteNewName().trim();
    if (!name) return;
    this.silhouetteSaving.set(true);
    this.silhouetteError.set(null);
    this.silhouetteService.create(name).subscribe({
      next: created => {
        this.silhouettes.update(list => [...list, created]);
        this.silhouetteNewName.set('');
        this.silhouetteSaving.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.silhouetteError.set('Failed to add silhouette.');
        this.silhouetteSaving.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  deleteSilhouette(id: number) {
    this.silhouetteDeleteErr.set(null);
    this.silhouetteService.delete(id).subscribe({
      next: res => {
        if (res.status === 204) {
          this.silhouettes.update(list => list.filter(s => s.id !== id));
        }
        this.cdr.markForCheck();
      },
      error: err => {
        const msg = err?.error?.message ?? 'Cannot delete this silhouette.';
        this.silhouetteDeleteErr.set(msg);
        this.cdr.markForCheck();
      }
    });
  }

  startEditSilhouette(s: SilhouetteTypeDto) {
    this.silhouetteEditingId.set(s.id);
    this.silhouetteEditName.set(s.name);
    this.silhouetteRenameError.set(null);
    this.cdr.markForCheck();
  }

  cancelEditSilhouette() {
    this.silhouetteEditingId.set(null);
    this.silhouetteEditName.set('');
    this.cdr.markForCheck();
  }

  saveRenameSilhouette() {
    const id = this.silhouetteEditingId();
    const name = this.silhouetteEditName().trim();
    if (id === null || !name) return;
    this.silhouetteRenaming.set(true);
    this.silhouetteRenameError.set(null);
    this.silhouetteService.rename(id, name).subscribe({
      next: updated => {
        this.silhouettes.update(list => list.map(s => s.id === id ? updated : s));
        this.silhouetteEditingId.set(null);
        this.silhouetteEditName.set('');
        this.silhouetteRenaming.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.silhouetteRenameError.set('Failed to rename silhouette.');
        this.silhouetteRenaming.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  onSilhouetteDragStart(e: DragEvent, index: number) {
    this.dragSilhouetteIdx.set(index);
    e.dataTransfer!.effectAllowed = 'move';
  }

  onSilhouetteDragOver(e: DragEvent, index: number) {
    e.preventDefault();
    e.dataTransfer!.dropEffect = 'move';
    if (this.dragOverSilhouetteIdx() !== index) {
      this.dragOverSilhouetteIdx.set(index);
      this.cdr.markForCheck();
    }
  }

  onSilhouetteDragLeave(e: DragEvent) {
    const related = e.relatedTarget as Node | null;
    const card = e.currentTarget as HTMLElement;
    if (!related || !card.contains(related)) {
      this.dragOverSilhouetteIdx.set(null);
      this.cdr.markForCheck();
    }
  }

  onSilhouetteDrop(e: DragEvent, dropIndex: number) {
    e.preventDefault();
    const dragIdx = this.dragSilhouetteIdx();
    this.dragSilhouetteIdx.set(null);
    this.dragOverSilhouetteIdx.set(null);
    if (dragIdx === null || dragIdx === dropIndex) { this.cdr.markForCheck(); return; }
    const list = [...this.silhouettes()];
    const [moved] = list.splice(dragIdx, 1);
    list.splice(dropIndex, 0, moved);
    this.silhouettes.set(list);
    this.cdr.markForCheck();
    this.silhouetteService.reorder(list.map(s => s.id)).subscribe();
  }

  onSilhouetteDragEnd() {
    this.dragSilhouetteIdx.set(null);
    this.dragOverSilhouetteIdx.set(null);
    this.cdr.markForCheck();
  }

  selectDress(id: string) {
    if (this.dressMode() !== 'idle') { this.dressMode.set('idle'); }
    this.dressDetailLoading.set(true);
    this.selectedDress.set(null);
    this.uploadError.set(null);
    this.svc.getDress(id).subscribe({
      next: d => { this.selectedDress.set(d); this.dressDetailLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.dressDetailLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  startCreateDress() {
    this.dressForm = this.blankDressForm();
    this.dressMode.set('create');
    this.selectedDress.set(null);
    this.dressError.set(null);
    this.cdr.markForCheck();
  }

  startEditDress() {
    const d = this.selectedDress();
    if (!d) return;
    this.dressForm = {
      name: d.name, slug: d.slug, tagline: d.tagline, description: d.description,
      silhouette: d.silhouette, material: d.material, corsetType: d.corsetType,
      trainDescription: d.trainDescription ?? '', color: d.color,
      hasSleeves: d.hasSleeves, sleeveDescription: d.sleeveDescription ?? '',
      decoration: d.decoration ?? '', customTailoringAvailable: d.customTailoringAvailable,
      isActive: d.isActive, collectionIds: [...(d.collectionIds ?? [])]
    };
    this.dressMode.set('edit');
    this.dressError.set(null);
    this.cdr.markForCheck();
  }

  cancelDressForm() { this.dressMode.set('idle'); this.dressError.set(null); this.cdr.markForCheck(); }

  saveDress() {
    if (!this.dressForm.name.trim()) { this.dressError.set('Name is required'); return; }
    this.dressSaving.set(true);
    this.dressError.set(null);
    const payload = {
      ...this.dressForm,
      silhouette: +this.dressForm.silhouette,
      trainDescription:  this.dressForm.trainDescription  || null,
      sleeveDescription: this.dressForm.sleeveDescription || null,
      decoration:        this.dressForm.decoration        || null,
    };
    const isEdit = this.dressMode() === 'edit';
    const obs = isEdit
      ? this.svc.updateDress(this.selectedDress()!.id, payload)
      : this.svc.createDress(payload);
    obs.subscribe({
      next: d => {
        this.selectedDress.set(d);
        this.dressMode.set('idle');
        this.dressSaving.set(false);
        this.loadDresses();
        this.cdr.markForCheck();
      },
      error: () => {
        this.dressError.set('Failed to save. Please try again.');
        this.dressSaving.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  toggleDressActive(id: string, isActive: boolean) {
    this.svc.toggleActive(id, !isActive).subscribe({
      next: () => {
        this.dresses.update(list => list.map(d => d.id === id ? { ...d, isActive: !isActive } : d));
        const sel = this.selectedDress();
        if (sel?.id === id) this.selectedDress.update(s => s ? { ...s, isActive: !isActive } : s);
        this.cdr.markForCheck();
      }
    });
  }

  toggleDressCollection(id: string) {
    const ids = this.dressForm.collectionIds;
    const idx = ids.indexOf(id);
    if (idx >= 0) ids.splice(idx, 1); else ids.push(id);
  }

  isDressCollSelected(id: string) { return this.dressForm.collectionIds.includes(id); }

  // ─── Photo upload ─────────────────────────────────────────────────
  onDrop(e: DragEvent) {
    e.preventDefault();
    this.dragOver.set(false);
    const files = e.dataTransfer?.files;
    if (files?.length) this.processFiles(Array.from(files));
  }
  onDragOver(e: DragEvent) { e.preventDefault(); this.dragOver.set(true); }
  onDragLeave() { this.dragOver.set(false); }

  onFileInput(e: Event) {
    const files = (e.target as HTMLInputElement).files;
    if (files?.length) this.processFiles(Array.from(files));
    (e.target as HTMLInputElement).value = '';
  }

  private processFiles(files: File[]) {
    const dress = this.selectedDress();
    if (!dress) return;
    this.uploadError.set(null);
    const imgs = files.filter(f => f.type.startsWith('image/') || f.name.match(/\.(jpe?g|png|webp|gif|heic|heif|avif|tiff?)$/i) !== null);
    if (!imgs.length) { this.uploadError.set('Please select image files only.'); return; }
    this.uploading.set(true);
    let done = 0;
    let failed = false;
    imgs.forEach((file, i) => {
      this.svc.uploadPhoto(file).subscribe({
        next: ({ url }) => {
          const p: AddPhotoPayload = {
            url, altText: dress.name,
            type: this.newPhotoType(),
            order: (this.selectedDress()?.photos.length ?? 0) + i + 1
          };
          this.svc.addPhoto(dress.id, p).subscribe({
            next: () => {
              done++;
              if (done + (failed ? 1 : 0) >= imgs.length || done === imgs.length) {
                this.uploading.set(false);
                this.refreshDress();
                this.cdr.markForCheck();
              }
            },
            error: (err: HttpErrorResponse) => {
              failed = true;
              const msg = err.error?.message ?? err.message ?? 'Unknown error';
              this.uploadError.set(`Photo saved to storage but DB save failed: ${msg}`);
              this.uploading.set(false);
              this.cdr.markForCheck();
            }
          });
        },
        error: (err: HttpErrorResponse) => {
          failed = true;
          const msg = err.error?.message ?? err.message ?? 'Unknown error';
          this.uploadError.set(`Upload failed: ${msg}`);
          this.uploading.set(false);
          this.cdr.markForCheck();
        }
      });
    });
  }

  deletePhoto(photoId: string) {
    const dress = this.selectedDress();
    if (!dress) return;
    this.svc.deletePhoto(dress.id, photoId).subscribe({
      next: () => this.refreshDress(),
      error: () => { this.uploadError.set('Failed to delete photo.'); this.cdr.markForCheck(); }
    });
  }

  // ─── Drag-and-drop reorder ────────────────────────────────────────
  onPhotoDragStart(e: DragEvent, index: number) {
    this.dragPhotoIdx.set(index);
    e.dataTransfer!.effectAllowed = 'move';
  }
  onPhotoDragOver(e: DragEvent, index: number) {
    e.preventDefault();
    e.dataTransfer!.dropEffect = 'move';
    if (this.dragOverIdx() !== index) { this.dragOverIdx.set(index); this.cdr.markForCheck(); }
  }
  onPhotoDragLeave(e: DragEvent) {
    const related = e.relatedTarget as Node | null;
    const card = (e.currentTarget as HTMLElement);
    if (!related || !card.contains(related)) { this.dragOverIdx.set(null); this.cdr.markForCheck(); }
  }
  onPhotoDrop(e: DragEvent, dropIndex: number) {
    e.preventDefault();
    const dragIdx = this.dragPhotoIdx();
    this.dragPhotoIdx.set(null);
    this.dragOverIdx.set(null);
    if (dragIdx === null || dragIdx === dropIndex) { this.cdr.markForCheck(); return; }
    const dress = this.selectedDress();
    if (!dress) return;
    const photos = [...dress.photos];
    const [moved] = photos.splice(dragIdx, 1);
    photos.splice(dropIndex, 0, moved);
    this.selectedDress.update(d => d ? { ...d, photos } : d);
    this.cdr.markForCheck();
    this.svc.reorderPhotos(dress.id, photos.map(p => p.id)).subscribe();
  }
  onPhotoDragEnd() { this.dragPhotoIdx.set(null); this.dragOverIdx.set(null); this.cdr.markForCheck(); }

  // ─── Download ─────────────────────────────────────────────────────
  downloadPhoto(url: string, type: number) {
    const ext = url.split('.').pop()?.split('?')[0] ?? 'jpg';
    const typeName = (PHOTO_TYPES.find(t => t.value === type)?.label ?? 'photo')
      .toLowerCase().replace(/\s+/g, '-');
    fetch(url)
      .then(r => { if (!r.ok) throw new Error(); return r.blob(); })
      .then(blob => {
        const a = document.createElement('a');
        a.href = URL.createObjectURL(blob);
        a.download = `${typeName}.${ext}`;
        a.click();
        URL.revokeObjectURL(a.href);
      })
      .catch(() => window.open(url, '_blank'));
  }

  private refreshDress() {
    const id = this.selectedDress()?.id;
    if (!id) return;
    this.svc.getDress(id).subscribe({ next: d => { this.selectedDress.set(d); this.cdr.markForCheck(); } });
  }

  photoTypeLabel(type: number) { return PHOTO_TYPES.find(t => t.value === type)?.label ?? 'Photo'; }

  // ─── Video methods ───────────────────────────────────────────────
  onVideoFileInput(e: Event) {
    const files = (e.target as HTMLInputElement).files;
    if (files?.length) this.processVideoFiles(Array.from(files));
    (e.target as HTMLInputElement).value = '';
  }

  private processVideoFiles(files: File[]) {
    const dress = this.selectedDress();
    if (!dress) return;
    this.videoUploadError.set(null);
    const videos = files.filter(f => f.type.startsWith('video/'));
    if (!videos.length) { this.videoUploadError.set('Please select video files only.'); return; }
    this.uploadingVideo.set(true);
    let done = 0;
    videos.forEach(file => {
      this.svc.uploadVideo(file).subscribe({
        next: ({ url }) => {
          const p: AddVideoPayload = { url, type: this.newVideoType() };
          this.svc.addVideo(dress.id, p).subscribe({
            next: () => {
              done++;
              if (done === videos.length) {
                this.uploadingVideo.set(false);
                this.refreshDress();
                this.cdr.markForCheck();
              }
            },
            error: (err: HttpErrorResponse) => {
              const msg = err.error?.message ?? err.message ?? 'Unknown error';
              this.videoUploadError.set(`Video saved to storage but DB save failed: ${msg}`);
              this.uploadingVideo.set(false);
              this.cdr.markForCheck();
            }
          });
        },
        error: (err: HttpErrorResponse) => {
          const msg = err.error?.message ?? err.message ?? 'Unknown error';
          this.videoUploadError.set(`Upload failed: ${msg}`);
          this.uploadingVideo.set(false);
          this.cdr.markForCheck();
        }
      });
    });
  }

  deleteVideo(videoId: string) {
    const dress = this.selectedDress();
    if (!dress) return;
    this.svc.deleteVideo(dress.id, videoId).subscribe({
      next: () => this.refreshDress(),
      error: () => { this.videoUploadError.set('Failed to delete video.'); this.cdr.markForCheck(); }
    });
  }

  videoTypeLabel(type: number) { return VIDEO_TYPES.find(t => t.value === type)?.label ?? 'Video'; }

  // ═══════════════════════════════════════════════════════════════════
  //  COLLECTIONS
  // ═══════════════════════════════════════════════════════════════════

  loadCollections() {
    this.collDisplayCount.set(SIDEBAR_PAGE);
    this.svc.getAllCollections(this.showDeletedCollections()).subscribe({
      next: c => { this.collections.set(c); this.collectionListLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.collectionListLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  toggleShowDeletedCollections() {
    this.showDeletedCollections.update(v => !v);
    this.loadCollections();
  }

  deleteCollection(id: string) {
    if (!confirm('Delete this collection? It can be restored later.')) return;
    this.svc.deleteCollection(id).subscribe({
      next: () => {
        if (this.showDeletedCollections()) {
          this.collections.update(list => list.map(c => c.id === id ? { ...c, isDeleted: true, deletedAt: new Date().toISOString() } : c));
        } else {
          this.collections.update(list => list.filter(c => c.id !== id));
        }
        if (this.selectedCollection()?.id === id) { this.selectedCollection.set(null); this.collectionMode.set('idle'); }
        this.cdr.markForCheck();
      }
    });
  }

  restoreCollection(id: string) {
    this.svc.restoreCollection(id).subscribe({
      next: () => {
        this.collections.update(list => list.map(c => c.id === id ? { ...c, isDeleted: false, deletedAt: null } : c));
        this.cdr.markForCheck();
      }
    });
  }

  selectCollection(c: CollectionDto) {
    this.selectedCollection.set(c);
    this.collectionMode.set('idle');
    this.collectionError.set(null);
    this.loadNavDresses(c.id);
    this.cdr.markForCheck();
  }

  loadNavDresses(collectionId: string) {
    this.navDressesLoading.set(true);
    this.navDisplayCount.set(INNER_PAGE);
    this.navDresses.set([]);
    this.svc.getNavDresses(collectionId).subscribe({
      next: d => { this.navDresses.set(d); this.navDressesLoading.set(false); this.cdr.markForCheck(); },
      error: () => { this.navDressesLoading.set(false); this.cdr.markForCheck(); }
    });
  }

  onDressListScroll(e: Event) {
    const el = e.target as HTMLElement;
    if (el.scrollHeight - el.scrollTop <= el.clientHeight + 120 && this.hasMoreDresses) {
      this.dressDisplayCount.update(n => n + SIDEBAR_PAGE); this.cdr.markForCheck();
    }
  }
  onCollListScroll(e: Event) {
    const el = e.target as HTMLElement;
    if (el.scrollHeight - el.scrollTop <= el.clientHeight + 120 && this.hasMoreCollections) {
      this.collDisplayCount.update(n => n + SIDEBAR_PAGE); this.cdr.markForCheck();
    }
  }
  onNavListScroll(e: Event) {
    const el = e.target as HTMLElement;
    if (el.scrollHeight - el.scrollTop <= el.clientHeight + 100 && this.hasMoreNavDresses) {
      this.navDisplayCount.update(n => n + INNER_PAGE); this.cdr.markForCheck();
    }
  }

  setNavOrder(dressId: string, order: number | null) {
    const col = this.selectedCollection();
    if (!col) return;
    this.svc.setNavOrder(col.id, dressId, order).subscribe({
      next: () => {
        this.navDresses.update(list => list.map(d => {
          if (d.id === dressId) return { ...d, navOrder: order };
          if (order !== null && d.navOrder === order) return { ...d, navOrder: null };
          return d;
        }));
        this.cdr.markForCheck();
      }
    });
  }

  navSlotDress(slot: number): NavDressItem | undefined {
    return this.navDresses().find(d => d.navOrder === slot);
  }

  startCreateCollection() {
    this.collectionForm = this.blankCollectionForm();
    this.collectionMode.set('create');
    this.selectedCollection.set(null);
    this.collectionError.set(null);
    this.cdr.markForCheck();
  }

  startEditCollection() {
    const c = this.selectedCollection();
    if (!c) return;
    this.collectionForm = {
      name: c.name, season: c.season ?? '',
      year: c.year, description: c.description ?? '',
      coverImageUrl: c.coverImageUrl ?? '',
      coverImageUrlMobile: c.coverImageUrlMobile ?? '',
      isActive: c.isActive
    };
    this.collectionMode.set('edit');
    this.collectionError.set(null);
    this.cdr.markForCheck();
  }

  cancelCollectionForm() { this.collectionMode.set('idle'); this.collectionError.set(null); this.cdr.markForCheck(); }

  saveCollection() {
    if (!this.collectionForm.name.trim()) { this.collectionError.set('Name is required'); return; }
    this.collectionSaving.set(true);
    this.collectionError.set(null);
    const payload = {
      ...this.collectionForm,
      year: +this.collectionForm.year,
      season:       this.collectionForm.season       || null,
      description:  this.collectionForm.description  || null,
      coverImageUrl: this.collectionForm.coverImageUrl || null,
      coverImageUrlMobile: this.collectionForm.coverImageUrlMobile || null,
    };
    const isEdit = this.collectionMode() === 'edit';
    const obs = isEdit
      ? this.svc.updateCollection(this.selectedCollection()!.id, payload)
      : this.svc.createCollection(payload);
    obs.subscribe({
      next: c => {
        this.selectedCollection.set(c);
        this.collectionMode.set('idle');
        this.collectionSaving.set(false);
        this.loadCollections();
        this.cdr.markForCheck();
      },
      error: () => {
        this.collectionError.set('Failed to save. Please try again.');
        this.collectionSaving.set(false);
        this.cdr.markForCheck();
      }
    });
  }

  toggleCollectionActive(id: string, isActive: boolean) {
    this.svc.toggleCollectionActive(id, !isActive).subscribe({
      next: () => {
        this.collections.update(list => list.map(c => c.id === id ? { ...c, isActive: !isActive } : c));
        if (this.selectedCollection()?.id === id)
          this.selectedCollection.update(c => c ? { ...c, isActive: !isActive } : c);
        this.cdr.markForCheck();
      }
    });
  }

  get featuredCollections() {
    return this.collections().filter(c => c.isFeatured).sort((a, b) => a.featuredOrder - b.featuredOrder);
  }

  setFeatured(id: string, isFeatured: boolean, order: number) {
    this.svc.setCollectionFeatured(id, isFeatured, order).subscribe({
      next: () => {
        this.collections.update(list => list.map(c => {
          if (c.id === id) return { ...c, isFeatured, featuredOrder: order };
          if (isFeatured && c.isFeatured && c.featuredOrder === order) return { ...c, isFeatured: false };
          return c;
        }));
        if (this.selectedCollection()?.id === id)
          this.selectedCollection.update(c => c ? { ...c, isFeatured, featuredOrder: order } : c);
        this.cdr.markForCheck();
      }
    });
  }

  featuredSlot(order: number): CollectionDto | undefined {
    return this.collections().find(c => c.isFeatured && c.featuredOrder === order);
  }

  onCoverInput(e: Event) {
    const files = (e.target as HTMLInputElement).files;
    if (!files?.length) return;
    this.coverUploading.set(true);
    this.svc.uploadPhoto(files[0]).subscribe({
      next: ({ url }) => { this.collectionForm.coverImageUrl = url; this.coverUploading.set(false); this.cdr.markForCheck(); },
      error: () => { this.coverUploading.set(false); this.cdr.markForCheck(); }
    });
    (e.target as HTMLInputElement).value = '';
  }

  onCoverMobileInput(e: Event) {
    const files = (e.target as HTMLInputElement).files;
    if (!files?.length) return;
    this.coverMobileUploading.set(true);
    this.svc.uploadPhoto(files[0]).subscribe({
      next: ({ url }) => { this.collectionForm.coverImageUrlMobile = url; this.coverMobileUploading.set(false); this.cdr.markForCheck(); },
      error: () => { this.coverMobileUploading.set(false); this.cdr.markForCheck(); }
    });
    (e.target as HTMLInputElement).value = '';
  }

  // ─── Blank forms ──────────────────────────────────────────────────
  private blankDressForm(): DressFormData {
    return { name: '', slug: '', tagline: '', description: '', silhouette: 0, material: '', corsetType: '', trainDescription: '', color: '', hasSleeves: false, sleeveDescription: '', decoration: '', customTailoringAvailable: false, isActive: true, collectionIds: [] };
  }
  private blankCollectionForm(): CollectionFormData {
    return { name: '', season: '', year: new Date().getFullYear(), description: '', coverImageUrl: '', coverImageUrlMobile: '', isActive: true };
  }
  private blankApptForm(): AdminApptFormData {
    return { firstName: '', lastName: '', phone: '', email: '', date: '', time: '', typeId: 0, notes: '' };
  }
  private blankTypeForm(): TypeFormData {
    return { name: '', price: '', description: '', mainDescription: '', detail: '', displayOrder: 0, isActive: true };
  }

  // ─── Settings ─────────────────────────────────────────────────────
  loadSettings() {
    this.atlierService.getInfo().subscribe({
      next: (info) => {
        this.atlierInfo = info;
        this.settingsForm.heroVideoDesktop = info.heroVideoDesktop || '';
        this.settingsForm.heroVideoMobile  = info.heroVideoMobile  || '';
        this.cdr.markForCheck();
      },
      error: () => {}
    });
  }

  uploadSettingsVideo(e: Event, target: 'desktop' | 'mobile') {
    const file = (e.target as HTMLInputElement).files?.[0];
    if (!file) return;
    this.settingsVideoUploading.set(true);
    this.settingsError.set(null);
    this.svc.uploadVideo(file).subscribe({
      next: ({ url }: { url: string }) => {
        if (target === 'desktop') this.settingsForm.heroVideoDesktop = url;
        else                      this.settingsForm.heroVideoMobile  = url;
        this.settingsVideoUploading.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.settingsError.set('Upload failed');
        this.settingsVideoUploading.set(false);
        this.cdr.markForCheck();
      }
    });
    (e.target as HTMLInputElement).value = '';
  }

  saveSettings() {
    if (!this.atlierInfo) return;
    this.settingsSaving.set(true);
    this.settingsError.set(null);
    const dto: AtlierInfoDto = {
      ...this.atlierInfo,
      heroVideoDesktop: this.settingsForm.heroVideoDesktop || null,
      heroVideoMobile:  this.settingsForm.heroVideoMobile  || null,
    };
    this.atlierService.upsert(dto).subscribe({
      next: (saved) => {
        this.atlierInfo = saved;
        this.settingsSaving.set(false);
        this.cdr.markForCheck();
      },
      error: () => {
        this.settingsError.set('Save failed');
        this.settingsSaving.set(false);
        this.cdr.markForCheck();
      }
    });
  }
}
