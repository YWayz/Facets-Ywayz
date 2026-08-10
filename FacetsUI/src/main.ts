import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';

//ngZoneEventCoalescing To reduce change detection cycles - Event Bubbling
platformBrowserDynamic().bootstrapModule(AppModule,{ ngZoneEventCoalescing: true })
  .catch(err => console.error(err));
