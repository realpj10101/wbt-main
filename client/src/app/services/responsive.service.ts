import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ResponsiveService {
  isMobileViewSig = signal<boolean>(false);
}
