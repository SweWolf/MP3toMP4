# MP3 to MP4 Converter

A clean Windows desktop application that combines an MP3 audio file with a static image to produce an MP4 video file — perfect for uploading music to YouTube or other video platforms.

---

## Features

- **Browse, drag & drop, or paste** your files — all input fields accept files dragged from Windows Explorer or straight from a web browser (e.g. Chrome), and the image field also supports pasting from the clipboard (Ctrl+V or right-click → Paste)
- **Auto-fill** — selecting an MP3 file automatically suggests matching image files from the same folder and pre-fills the output MP4 path
- **Embedded artwork** — if the MP3 contains album art in its ID3 tags, a checkbox lets you extract and use it with one click
- **Image preview** — the selected image is displayed instantly inside the application
- **Live progress bar** — shows conversion progress as a percentage with an estimated time remaining
- **Cancel at any time** — cancelling mid-conversion kills the FFmpeg process and deletes the incomplete output file
- **Create Shortcut** — easily add the app to your Desktop, Start Menu, or Send To menu via Setup → Create Shortcut

---

## Requirements

- **Windows 10 / 11**
- **.NET 10 Runtime** ([download](https://dotnet.microsoft.com/download))
- **FFmpeg** — place `ffmpeg.exe` next to the application, or install it anywhere on your system PATH  
  ([download FFmpeg](https://ffmpeg.org/download.html))

---

## Usage

1. Select or drag in an **MP3 file**
2. Select or drag in an **image file** (JPG, PNG, BMP, GIF, WebP) — or use the embedded artwork from the MP3
3. Confirm the **output MP4 path** (auto-filled, but can be changed)
4. Click **Convert**

### Command-line

You can pass an MP3 file directly as an argument, for example via a shortcut or the Send To menu:

```
MP3toMP4.exe "C:\Music\MySong.mp3"
```

The application will open with the MP3 pre-loaded and the image and output fields auto-filled.

---

## Built With

- [.NET 10 / Windows Forms](https://dotnet.microsoft.com/)
- [FFmpeg](https://ffmpeg.org/) — audio/video conversion engine
- [TagLibSharp](https://github.com/mono/taglib-sharp) — embedded MP3 artwork extraction

---

## License

To be decided at first public release.

---

*Developed by [SweWolf](https://github.com/SweWolf)*
