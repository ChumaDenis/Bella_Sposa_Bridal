declare global {
  interface Window {
    gtag?: (...args: unknown[]) => void;
  }
}

export function trackAppointmentBooked(): void {
  window.gtag?.('event', 'book_appointment');
}
