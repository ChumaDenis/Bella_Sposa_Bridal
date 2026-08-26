declare global {
  interface Window {
    dataLayer?: unknown[];
  }
}

export function trackAppointmentBooked(): void {
  window.dataLayer = window.dataLayer || [];
  window.dataLayer.push({ event: 'book_appointment' });
}
