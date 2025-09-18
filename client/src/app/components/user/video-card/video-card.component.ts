import { Component, HostBinding, Input } from '@angular/core';

@Component({
  selector: 'app-video-card',
  standalone: true,
  imports: [],
  templateUrl: './video-card.component.html',
  styleUrl: './video-card.component.scss'
})
export class VideoCardComponent {
  @Input() videoSrc: string | undefined;
  @Input() posterSrc: string | undefined;
  @Input() alt = 'preview';

  @HostBinding('class.is-playing') isPlaying = false;

  duration = 0;
  currentTime = 0;
  buffered = 0;
  progress = 0;
  volume = 0.8;
  muted = false;
  showControls = false;

  private _isScrubbing = false;

  formatTime(sec: number): string {
    if (!isFinite(sec)) return '00:00';
    const second = Math.floor(sec % 60).toString().padStart(2, '0');
    const minute = Math.floor((sec / 60)).toString().padStart(2, '0');
    const hour = Math.floor(sec / 3600);
    return hour > 0 ? `${hour}:${minute}:${second}` : `${minute}:${second}`;
  }

  play(video: HTMLVideoElement): void {
    this.isPlaying = true;
    video.muted = this.muted;
    video.volume = this.volume;
    video.playsInline = true;

    video.play().catch(() => { })
  }

  pause(video: HTMLVideoElement): void {
    this.isPlaying = false;
    video.pause();
  }

  reset(video: HTMLVideoElement): void {
    this.pause(video);
    video.currentTime = 0;
  }

  // onLoadMetaData(vide: Html)
}
