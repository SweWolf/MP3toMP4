# MP3 to MP4 Help

MP3 to MP4 turns an audio file into a video file. The video shows a picture, a video clip,
several pictures one after another, a black screen or a few lines of text, while your audio
plays. This is useful for uploading music to YouTube and other video sites, which only accept
video files.

## Contents

- [Getting Started](#getting-started)
- [The Main Window](#the-main-window)
  - [MP3 File](#mp3-file)
  - [Trim to a Range](#trim-to-a-range)
  - [Single Image/Video](#single-imagevideo)
  - [Multiple Images/Videos](#multiple-imagesvideos)
  - [Include Lyrics Tag](#include-lyrics-tag)
  - [Output](#output)
  - [Converting](#converting)
- [MP4 or MKV?](#mp4-or-mkv)
- [Settings](#settings)
- [Create Shortcut](#create-shortcut)
- [Drag and Drop, and Paste](#drag-and-drop-and-paste)
- [Keyboard Shortcuts](#keyboard-shortcuts)
- [Log Files](#log-files)
- [Troubleshooting](#troubleshooting)

## Getting Started

### What You Need

- Windows 10 or Windows 11.
- **FFmpeg**, the free program that does the actual conversion. Download it from
  [ffmpeg.org](https://ffmpeg.org/download.html) and either put `ffmpeg.exe` in the same folder
  as MP3 to MP4, or install it so that it is on the system PATH.
  MP3 to MP4 also looks in `C:\Program Files\FFmpeg\bin`.

You can see which FFmpeg version is found under **Help > About**.

### Your First Video

1. Click the **...** button next to **MP3 File** and choose your audio file.
2. If there is a picture with the same name as the audio file in the same folder (for example
   `My Song.jpg` next to `My Song.mp3`), it is selected automatically. Otherwise, click the
   **...** button next to **Image or Video File** and choose a picture.
3. Check the file name under **MP4 or MKV File**. It is filled in for you.
4. Click **Convert**.

When the conversion is finished, click **Open File** to watch the video.

## The Main Window

The window has an **Input** area at the top, where you choose what goes into the video, and an
**Output** area below it, where you choose where the video is saved. The buttons at the bottom
start and control the conversion.

### MP3 File

The audio file to use. Click **...** to browse for it, drag it onto the field from File Explorer,
or type or paste its path.

Despite the name, you can use most audio formats: MP3, WAV, FLAC, M4A, AAC, OGG, Opus, WMA and
AIFF. In the Open dialog, choose **Audio files** to see all of them.

The length of the audio is shown to the right of the field.

When you choose an audio file, MP3 to MP4 also:

- looks for a picture with the same name in the same folder,
- checks whether the file contains a picture (album art) and lyrics,
- fills in the output file name.

### Trim to a Range

Check **Trim to a Range** to use only part of the audio. Four more fields appear:

- **Start at**: where the video starts. Leave it empty to start at the beginning.
- **End at**: where the video ends. Leave it empty to continue to the end.
- **Fade in (s)**: the number of seconds over which the sound fades in at the start.
  0 means no fade.
- **Fade out (s)**: the number of seconds over which the sound fades out at the end.
  0 means no fade.

You can write the times in several ways:

| You Write | It Means |
|-----------|----------|
| `90` | 90 seconds |
| `7.5` or `7,5` | 7.5 seconds |
| `2:07` | 2 minutes and 7 seconds |
| `1:23:45` | 1 hour, 23 minutes and 45 seconds |
| `45%` | 45% of the audio's length |

A percentage is useful when you convert several files of different lengths with the same
settings: it is worked out again for each file.

You can also fade in and out without trimming: check **Trim to a Range**, leave **Start at** and
**End at** empty, and enter the fade lengths.

When the box is unchecked, the whole audio file is used and the fade fields are ignored.

### Single Image/Video

Use this tab to show one thing during the whole video. Choose one of the three options on the
left side of the tab. The preview on the right shows what the video will look like.

#### Image or Video File

Shows a picture or a video clip.

- Pictures: JPG, PNG, BMP, GIF and WebP.
- Videos: MP4, MOV, AVI, MKV, WebM and M4V. A clip that is shorter than the audio starts again
  from the beginning until the audio ends. The clip's own sound is not used.

Click **...** to browse, choose one of the pictures in the drop-down list, or drag a file onto the
field or the preview. See also [Drag and Drop, and Paste](#drag-and-drop-and-paste).

**Use Image from the MP3 File** uses the picture (album art) stored inside the audio file.
It is only available when the audio file contains a picture. While it is checked, you can't
choose another picture.

#### Black Screen

The video is just black, 1280 × 720 pixels. Useful when only the sound matters.

#### Text

White text, centered on a black screen. Type the text in the box next to **Text**. Each line in
the box becomes one line in the video. The text size is chosen automatically, so that all lines
fit.

The box is filled in for you with the artist and, on the next line, the song title in quotes, taken
from the audio file's tags. If the file has no title, the file name is used. You can change the
text as you like. Once you have changed it, it is kept even if you choose another audio file.

### Multiple Images/Videos

Use this tab to show several pictures or clips one after another, for example one picture per
song in a long mix.

Add files with **Add...** (you can select several at once), or drag them from File Explorer onto
the list. If you have already chosen a picture on the **Single Image/Video** tab, it is added as
the first row when you open this tab.

The list has three columns:

- **File**: the picture or video. Files in the same folder as the audio file, or in a subfolder
  of it, are shown with a short name (for example `Images\cover.jpg`). You can also type a
  file name here: it is looked for in the audio file's folder.
- **Start Time**: when this file appears in the video. The first row always starts at 0:00.
  Times are written in the same way as in [Trim to a Range](#trim-to-a-range) (but not as a
  percentage), and they count from the start of the video, so after any trimming. If you leave
  it blank, the row starts when the row above it ends (see below).
- **Duration**: how long the file is shown. A video shows its own full length in gray. Type a
  shorter time to show only the first part of it. The last row can't be changed: it lasts until
  the end of the video.

When you change a Start Time, the rows are sorted by time again automatically. A row with a
blank Start Time stays below the row above it and moves along with it.

**Blank Start Times:** you don't have to fill in every Start Time. Nothing is calculated until you
click **Convert**. A row with a blank Start Time starts when the row above it ends, that is after
the Duration you typed, or after the full length of a video. Because a picture has no length of
its own, you must type a Duration for it, unless it is followed by a row with a Start Time, or it
is one of the pictures at the end of the list. Those pictures share the time that is left (up to
the next Start Time you typed, or the end of the video) equally, in whole seconds. The last of
them gets what is left over, so it may be shown a little longer. For example, if you add only
pictures and leave all the Start Times blank, they share the whole video equally.

When a Start Time you typed is earlier than the end of the video above it, that video is cut
off. When it is later, the video is repeated until the next row starts. The last row is also
repeated until the end of the audio, and files that would start after the end of the audio are
left out.

While the video is being made, the list and the tabs are locked and the current row follows the
progress. When the conversion is finished, the first row is selected again.

The buttons next to the list:

- **▲** and **▼**: move the selected file up or down. Only the file moves: the Start Times stay
  where they are. This way you can change which picture is shown when, without retyping the times.
- **Delete**: removes the selected row.

The preview on the right shows the selected file.

The video gets the size of the first picture or clip in the list. Other files that have a different
shape get black bars at the sides or at the top and bottom.

### Include Lyrics Tag

If the audio file contains lyrics, check **Include Lyrics Tag** to copy them into the video file.
Music players that support lyrics (for example iTunes, MusicBee and foobar2000) can then show
them.

The lyrics are **not** shown in the picture of the video. The option is only available when the
audio file contains lyrics. MP3 to MP4 remembers your choice for next time.

### Output

**MP4 or MKV File** is where the video is saved. It is filled in automatically when you choose an
audio file: the same name as the audio file, in the same folder, or in the
**Default Output Folder** if you have set one in [Settings](#settings).

The file extension decides the format: `.mp4` or `.mkv`. To change it, type the other extension,
or click **...** and choose the format under **Save as type**. See [MP4 or MKV?](#mp4-or-mkv).

If you type only a file name, without a folder, the file is saved in the same folder as the
audio file.

If the folder does not exist, MP3 to MP4 offers to create it. If the file already exists, you are
asked whether to overwrite it.

### Converting

- **Convert** starts the conversion. The progress bar and **Estimated remaining time** show how
  far it has come.
- **Cancel** stops the conversion. The unfinished video file is deleted.
- **Clear** empties all the fields, so you can start over.

When the conversion is finished, MP3 to MP4 plays a sound (you can change this in
[Settings](#settings)) and three buttons become available:

- **Open File**: plays the video in your default video player.
- **Open Folder**: opens File Explorer with the video selected.
- **Open Log File**: shows FFmpeg's report of the conversion. Useful when something went wrong.

If you close MP3 to MP4 during a conversion, you are asked first, and the unfinished video file
is deleted.

## MP4 or MKV?

**MP4** plays almost everywhere: phones, TVs, browsers, video editors and all video sites.
Choose MP4 if you are unsure.

- MP3, M4A and AAC audio is copied into the video unchanged, so there is no loss of quality.
- Other formats (WAV, FLAC, OGG and so on) are converted to AAC, a high-quality compressed
  format.
- Trimmed or faded audio is always converted to AAC.

**MKV** keeps 100% of the audio quality, even for WAV and FLAC files.

- WAV and AIFF audio is stored as FLAC. FLAC is lossless compression, like a ZIP file for audio:
  the sound is exactly the same, but it takes about half the space. (32-bit WAV files are copied
  unchanged, because FLAC can't store them exactly.)
- All other audio is copied unchanged.
- Trimmed or faded audio is converted to FLAC, so no quality is lost there either.
- The files are bigger than MP4 files, and some TVs, phones and video editors can't play MKV.
  YouTube accepts MKV files.

Choose which one is suggested in **Tools > Settings > Default Output Format**.

## Settings

Open the settings with **Tools > Settings**. Click **OK** to save your changes, or **Cancel** to
close the window without saving.

### Updates

**Check for New Version at Startup**: when **Yes**, MP3 to MP4 checks on GitHub whether a new
version is available each time it starts. If there is one, **New Version Available** appears in green
at the right end of the menu bar. Click it to open the download page.

### Default Output Folder

The folder where new videos are saved. Leave it empty to save each video in the same folder as
its audio file. Click **...** to browse for a folder.

### Default Output Format

The format of the suggested output file:

- **MP4 (Recommended)**: always MP4.
- **MKV**: always MKV.
- **MKV When Converting from a Lossless Format**: MKV for WAV, FLAC and AIFF files, MP4 for all
  others.

This only decides what is suggested. You can still change the extension of each file under
**MP4 or MKV File**. See [MP4 or MKV?](#mp4-or-mkv).

### Audio

**Optimize Audio For** is used whenever the audio of an MP4 file has to be converted to AAC
(see [MP4 or MKV?](#mp4-or-mkv)):

- **Quality (Recommended)**: 384 kbit/s, the quality YouTube recommends for uploads.
- **Smaller File Size**: 192 kbit/s, about half the size of the audio.

It is not used for MKV files, which always keep the full quality.

### When the Conversion Is Finished

**Action** decides what happens when a conversion is finished:

- **Play a Sound**: plays the sound chosen under **Sound**. Click **▶** to listen to it. Choose
  **Custom Sound File...** at the end of the list to use your own WAV file, and select it under
  **Custom Sound File (WAV)**.
- **Message Box**: shows a message with the name of the new video file.
- **None**: does nothing.

## Create Shortcut

**Tools > Create Shortcut...** creates shortcuts to MP3 to MP4. Check where you want them:

- **Desktop**
- **Start Menu (Programs)**
- **Send To menu**: lets you right-click an audio file in File Explorer and choose
  **Send to > MP3 to MP4**. The program then opens with that file already chosen, and the
  picture and output fields filled in.

Choose whether the shortcuts are for the **Current user only** or for **All users**. All users may
require administrator rights. The Send To shortcut is always for the current user only.

You can also start the program from the command line with an audio file:

```
MP3toMP4.exe "C:\Music\My Song.mp3"
```

## Drag and Drop, and Paste

You can drag files from File Explorer onto:

- the **MP3 File** field,
- the **Image or Video File** field or its preview,
- the **MP4 or MKV File** field,
- the list on the **Multiple Images/Videos** tab (several files at once).

For pictures there are two more ways:

- **From a web browser**: drag a picture from a web page (for example in Chrome) onto the
  **Image or Video File** field. It is downloaded and used.
- **From the clipboard**: copy a picture in any program, click in the **Image or Video File**
  field and press **Ctrl+V**, or right-click the field and choose **Paste**.

Paths copied with **Copy as path** in File Explorer can be pasted into all the fields. The quotes
around them are removed automatically.

## Keyboard Shortcuts

| Key | Action |
|-----|--------|
| **F1** | Open this help |
| **Ctrl+E** | Convert |
| **Ctrl+O** | Open File (the finished video) |
| **Ctrl+Shift+O** | Open Folder |
| **Alt+B** | Browse for an MP3 file |
| **F6** | Go to the **MP3 File** field |

## Log Files

MP3 to MP4 writes a log file for every conversion. They are kept in
`%AppData%\SweWolfSoftware\MP3toMP4\Logs`. Logs older than 30 days are deleted automatically.

The **Help** menu has two commands for them:

- **Open FFmpeg Call Log**: a list of every FFmpeg command MP3 to MP4 has run. It is kept
  short automatically.
- **Open Log Folder**: opens the folder with all the log files in File Explorer.

The log of the latest conversion can also be opened with the **Open Log File** button.

## Troubleshooting

**"FFmpeg not found."**
MP3 to MP4 can't find `ffmpeg.exe`. Put it in the same folder as MP3 to MP4, or install FFmpeg
so that it is on the system PATH. See [What You Need](#what-you-need).

**"Conversion failed: FFmpeg exited with code ..."**
Something went wrong inside FFmpeg. Click **Open Log File** and look near the end of the log for
lines with "Error" in them. They usually say what the problem is, for example a damaged input
file.

**"... contains the character ..., which is not allowed in file and folder names."**
The output file or folder name contains a character Windows doesn't allow, such as `?`, `*`, `:`
or `|`. Remove it and try again.

**The output file must have the .mp4 or .mkv extension.**
End the output file name with `.mp4` or `.mkv`.

**Use Image from the MP3 File or Include Lyrics Tag can't be checked.**
The audio file doesn't contain a picture or lyrics. Hover over **Include Lyrics Tag** to see
why it is not available.

**The picture I dragged from a web page doesn't work.**
Some web sites don't allow their pictures to be downloaded this way. Save the picture to your
computer first (right-click it and choose **Save image as**), and then use that file.
