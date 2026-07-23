import { Component, EventEmitter, Input, Output, inject } from '@angular/core';
import { ImageService } from '../../core/image.service';

/**
 * Reusable photo upload control: pick a file from the device, upload it, preview
 * the result, and emit the stored URL (or null when removed). Used by the
 * add-plant form and the garden edit panel.
 */
@Component({
  selector: 'app-photo-picker',
  standalone: true,
  templateUrl: './photo-picker.component.html',
  styleUrl: './photo-picker.component.scss'
})
export class PhotoPickerComponent {
  @Input() imageUrl?: string | null;
  @Output() imageUrlChange = new EventEmitter<string | null>();

  private readonly images = inject(ImageService);

  uploading = false;
  error?: string;

  onFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) {
      return;
    }

    this.uploading = true;
    this.error = undefined;
    this.images.upload(file).subscribe({
      next: ({ url }) => {
        this.uploading = false;
        this.imageUrl = url;
        this.imageUrlChange.emit(url);
      },
      error: () => {
        this.uploading = false;
        this.error = 'Upload failed. Use a JPG, PNG or WebP under 5 MB.';
      }
    });

    // Allow re-selecting the same file later.
    input.value = '';
  }

  remove(): void {
    this.imageUrl = null;
    this.error = undefined;
    this.imageUrlChange.emit(null);
  }
}
