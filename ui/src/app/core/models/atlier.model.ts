export interface AtlierInfoDto {
  address: string;
  fittingDurationMinutes: number;
  isFittingFree: boolean;
  maxGuests: number;
  appointmentRequired: boolean;
  phone: string;
  whatsApp: string | null;
  telegram: string | null;
  instagram: string | null;
  workingHours: string | null;
  vipPrice: number | null;
}
