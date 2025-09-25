import { Component, ElementRef, HostBinding, HostListener, Input, OnDestroy, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

@Component({
  selector: 'app-video-card',
  standalone: true,
  imports: [
    MatIconModule, MatButtonModule
  ],
  templateUrl: './video-card.component.html',
  styleUrl: './video-card.component.scss'
})
export class VideoCardComponent {
  @Input() videoSrc: string | undefined;
  @Input() posterSrc: string | undefined;
  @Input() alt = 'preview';

  isPlaying = false;
  duration = 0;
  currentTime = 0;
  seeking = false;
  seekPreviewTime = 0;
  bufferPercent = 0;
  hoverVisible = false;
  thumbVisible = false;
  hoverX = 0;
  thumbX = 0;
  hoverTime = 0;
  volume = 1;
  muted = false;

  private _lastNonZeroVolume = 1;

  get PlayedPercent() {
    return this.duration ? (this.currentTime / this.duration) * 100 : 0;
  }

  @ViewChild('player') playerRef!: ElementRef<HTMLVideoElement>;
  @ViewChild('thumbVideo') thumbVideoRef!: ElementRef<HTMLVideoElement>;
  @ViewChild('seekWrap') seekWrapRef!: ElementRef<HTMLElement>;
  @ViewChild('thumbCanvas') thumbCanvasRef?: ElementRef<HTMLCanvasElement>;

  togglePlay(): void {
    const video = this.playerRef.nativeElement;

    if (video.paused) {
      video.play();
      this.isPlaying = true;
    }
    else {
      video.pause();
      this.isPlaying = false;
    }
  }

  toggleFullScreen(): void {
    const video = this.playerRef.nativeElement;

    if (video.requestFullscreen) {
      video.requestFullscreen();
    }
    else if ((video as any).webkitRequestFullscreen) { // For safary
      (video as any).webkitRequestFullscreen();
    }
    else if ((video as any).msRequestFullscreen) { // For IE
      (video as any).msRequestFullscreen();
    }
  }

  onLoadedMetaData(): void {
    const video = this.playerRef.nativeElement;

    this.duration = video.duration || 0;

    video.volume = this.volume;
    video.muted = this.muted;
  }

  onVolumeChange(): void {
    const video = this.playerRef.nativeElement;

    this.volume = video.volume;
    this.muted = video.muted || this.volume === 0;

    if (!this.muted && this.volume > 0) this._lastNonZeroVolume = this.volume;
  }

  onVolumeInput(event: Event): void {
    const video = this.playerRef.nativeElement;
    const value = Number((event.target as HTMLInputElement).value);

    this.volume = value;
    video.volume = value;

    if (value === 0) {
      video.muted = true;
      this.muted = true;
    }
    else {
      video.muted = false;
      this.muted = false;
      this._lastNonZeroVolume = value;
    }
  }

  toggleMute(): void {
    const video = this.playerRef.nativeElement;

    if (this.muted || this.volume === 0) {
      const restore = this._lastNonZeroVolume || 0.5;
      video.muted = false;
      video.volume = restore;
      this.volume = restore;
      this.muted = false;
    }
    else {
      this._lastNonZeroVolume = this.volume > 0 ? this.volume : (this._lastNonZeroVolume || 0.5);
      video.muted = true;
      this.muted = true;

      this.volume = 0;
    }
  }

  onTimeUpdate(): void {
    const v = this.playerRef.nativeElement;

    this.currentTime = v.currentTime || 0;
  }

  onProgress(): void {
    const video = this.playerRef.nativeElement;

    try {
      if (video.buffered.length) {
        const end = video.buffered.end(video.buffered.length - 1); // last buffer
        this.bufferPercent = this.duration ? Math.min((end / this.duration) * 100, 100) : 0;
      }
      else {
        this.bufferPercent = 0;
      }
    }
    catch {
      this.bufferPercent = 0;
    }
  }

  onSeekInput(event: Event): void {
    const value = Number((event.target as HTMLInputElement).value);

    this.seekPreviewTime = value;
  }

  onSeekCommit(event: Event): void {
    const v = this.playerRef.nativeElement;
    const value = Number((event.target as HTMLInputElement).value);

    v.currentTime = value;
    this.currentTime = value;
    this.seeking = false;
  }

  formatTime(second: number): string {
    if (!isFinite(second)) return '0:00';

    const minute = Math.floor(second / 60);
    const sec = Math.floor(second % 60);

    return `${minute}:${sec.toString().padStart(2, '0')}`;
  }
}
