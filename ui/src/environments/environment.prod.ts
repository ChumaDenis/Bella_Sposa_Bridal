export const environment = {
  production: true,
  apiBase: (window as any).__env?.apiBase
    || 'https://bellasposabridal-production.up.railway.app/api'
};
