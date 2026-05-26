export interface AppointmentTypeConfigDto {
  id: number;
  name: string;
  price: number | null;
  description: string | null;
  mainDescription: string | null;
  detail: string | null;
  displayOrder: number;
  isActive: boolean;
}

export interface CreateAppointmentDto {
  firstName: string;
  lastName: string;
  phone: string;
  email: string | null;
  appointmentDateTime: string; // ISO string
  type: number;
  notes: string | null;
  viewedDressIds: string[];
}

export interface AppointmentFileDto {
  id: string;
  fileName: string;
  url: string;
  size: number;
  contentType: string;
  uploadedAt: string;
}

export interface AppointmentDto {
  id: string;
  firstName: string;
  lastName: string;
  phone: string;
  email: string | null;
  appointmentDateTime: string;
  typeId: number;
  type: string;
  status: string;
  notes: string | null;
  adminNotes: string | null;
  viewedDresses: any[];
  files: AppointmentFileDto[];
  createdAt: string;
}

export interface TimeSlotDto {
  id: string;
  time: string;
  isActive: boolean;
  displayOrder: number;
}

export interface DayScheduleDto {
  date: string;
  isClosed: boolean;
  customSlots: string[] | null;
}
