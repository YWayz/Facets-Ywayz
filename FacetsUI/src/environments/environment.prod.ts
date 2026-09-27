// The production build is served by Facets.Api from the same App Service (it publishes into wwwroot),
// so the API is always on the same host as the page. Using the page's own origin means the same
// build works on facets-uat, the production domain, or any new App Service without editing this file.
const origin = typeof window !== 'undefined' ? window.location.origin : '';

export const environment = {
    production: true,

    baseEndPoint: `${origin}/api`,
    baseWebEndPoint: origin
};
