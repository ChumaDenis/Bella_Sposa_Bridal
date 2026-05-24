export enum AppointmentType { First = 0, Second = 1, Vip = 2 }

export interface CreateAppointmentDto {
  firstName: string;
  lastName: string;
  phone: string;
  email: string | null;
  appointmentDateTime: string; // ISO string
  type: AppointmentType;
  notes: string | null;
  viewedDressIds: string[];
}

export interface AppointmentDto {
  id: string;
  firstName: string;
  lastName: string;
  status: string;
}
