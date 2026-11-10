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
- [Projects](#projects)
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
AIFF. The Open dialog shows **Audio files** (all of these formats) first. You can also choose
**MP3 files**, **FLAC files**, **WAV files** or **All files**.

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

When the preview shows a picture, click it to open the picture in SweWolf File Explorer. If that
program is not found, the picture opens in the program Windows has for that type of file
(new in the next version).

**Use Image from the MP3 File** uses the picture (album art) stored inside the audio file.
It is only available when the audio file contains a picture. While it is checked, you can't
choose another picture.

#### Black Screen

The video is just black, 1280 × 720 pixels (2560 × 1440 if [Video Quality](#output) is set to
High). Useful when only the sound matters.

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

The preview on the right shows the selected file. Click it to open the picture, as on the
Single Image/Video tab.

**Effects** (new in the next version): the **Effects** box between the buttons and the preview
changes how the files follow each other. MP3 to MP4 remembers your choices for next time.

- **Transition**: what happens where one file is replaced by the next. It is the same for every
  switch. **None** is an immediate cut. Otherwise choose a type in the first list, and the exact
  variant (for example **Left** or **Right**) in the second list, which changes with the type:
  - **Fade**: Crossfade, Fast Fade, Slow Fade, Through Black (the first file fades out to black
    and the next one fades in from black), Through White, Through Gray, and Dissolve (the picture
    changes pixel by pixel).
  - **Wipe**: the next picture is uncovered by an edge that moves across (Left, Right, Up, Down,
    or from a corner). **Diagonal Wipe** moves diagonally, and **Soft Wipe** has a soft edge.
  - **Slide**: both pictures slide away together. In **Cover** the next picture slides in over the
    old one, and in **Reveal** the old picture slides away and uncovers the next.
  - **Shape**: Circle Open, Circle Close, Circle Crop, Rectangle Crop and Radial.
  - **Doors**: the next picture opens or closes like doors (vertical or horizontal).
  - **Slices** and **Wind**: the pictures are cut into thin strips that move or are blown away.
  - **Effect**: Pixelize, Horizontal Blur, Distance, Squeeze Horizontal, Squeeze Vertical and Zoom In.
  - **Random**: a different transition for every switch, picked from all of the above. Which ones
    were used is written to the log file.

  Nothing is added at the very start or the very end of the video.
- **Length (seconds)**: the total time of one transition. During the transition the two files are
  on screen at the same time, centered on the Start Time of the new file: half of the time before
  it and half after it. So every file still appears when the list says, and the audio is not
  affected. The default is 0.5 seconds, which is quick but clearly visible. A file that is shorter
  than the transition gets a shorter transition.
- **Slow Zoom In**: every picture starts at its normal size and slowly zooms in (to 110%) for as
  long as it is shown, so a still picture feels alive. The zoom is toward the center of the
  picture. Videos are not zoomed. This makes the conversion slower, especially for a long video
  with a few pictures.

You can use both effects at the same time.

The video gets the size of the first picture or clip in the list (2560 × 1440 if
[Video Quality](#output) is set to High). Other files that have a different
shape get black bars at the sides or at the top and bottom. (With Slow Zoom In the black bars
zoom along with the picture.)

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

The file extension decides the format: `.mp4` or `.mkv`. To change it, click **MP4** or **MKV** in
the label above the field (new in the next version), type the other extension, or click **...**
and choose the format under **Save as type**. See [MP4 or MKV?](#mp4-or-mkv).

If you type only a file name, without a folder, the file is saved in the same folder as the
audio file.

If the folder does not exist, MP3 to MP4 offers to create it. If the file already exists, you are
asked whether to overwrite it.

**Video Quality** sets the size of the video. MP3 to MP4 remembers your choice for next time.

- **Standard (720p)**: the picture is made 1280 pixels wide. This gives the smallest file and the
  fastest conversion.
- **High (1440p for YouTube)**: the video is 2560 × 1440 pixels, and is encoded at a higher quality,
  which takes longer. A picture that is not 16:9 is not stretched: it gets black bars at the sides
  or at the top and bottom.

Choose **High** if you upload to YouTube and the picture looks blurry when the quality is set to
**Auto**. YouTube makes smaller copies of every video you upload, and it treats videos of 1440p and
larger better than smaller ones. The picture itself does not get more detail: a small picture is
only enlarged.

### Converting

- **Convert** starts the conversion. The progress bar and **Estimated remaining time** show how
  far it has come.
- **Cancel** stops the conversion. The unfinished video file is deleted.
- **Clear** empties all the fields, so you can start over. It also closes the open
  [project](#projects), like **File > New Project**.

When the conversion is finished, MP3 to MP4 plays a sound (you can change this in
[Settings](#settings)) and the **Open File** and **Open Log File** buttons become available:

- **Open File**: plays the video in SweWolf Media Player. If SweWolf Media Player is not installed
  (next to MP3 to MP4, or in its own folder beside it), the video opens in your default video player.
  Right-click the button and choose **Open With** to pick another program in the Windows "Open with"
  window.
- **Open Folder** (always available): opens the folder of the video, with the video selected when it
  exists. It uses SweWolf File Explorer if that is installed, else Windows File Explorer.
- **Open Log File**: shows FFmpeg's report of the conversion. Useful when something went wrong.

**Watching while it is being created (new in the next version):** when the output is an MKV file,
**Open File** can become available while the conversion is still running. This happens after about
10 seconds of video have been created, and only if the estimated remaining time is clearly shorter than
the length of the video. Then the conversion stays ahead of the player. With slower settings (for example
High video quality with many images) the conversion takes longer than the video, so the button waits
until the conversion is finished. The video player shows the part that is done so far. You can't skip
ahead, because the player doesn't know the full length yet. MP4 files can't be played until the
conversion is finished.

If you close MP3 to MP4 during a conversion, you are asked first, and the unfinished video file
is deleted.

## Projects

*New in the next version.*

A project is everything you have filled in in the main window, saved as one file: the audio file,
the trim range, the picture, or the list of pictures and videos with their times, the effects and
the output file. Save a project when you want to continue later, or to make the same video again
with small changes. The settings under **Tools > Settings** are not part of a project.

The commands are in the **File** menu:

- **New Project** (Ctrl+N): closes the project and empties the window. The **Clear** button does
  the same.
- **Open Project...**: opens a project file.
- **Recent Projects**: the projects you opened or saved last, with the newest first. Click one to
  open it, or press its number. **Clear List** at the bottom empties the list. If a project is no
  longer where it was, you are asked whether to remove it from the list. You choose how many
  projects are shown in [Settings](#projects-1).
- **Save Project** (Ctrl+S): saves the project. The first time, you are asked for a file name.
- **Save Project As...** (Ctrl+Shift+S): saves the project under another name. You then continue
  in the new file, and the old one stays as it is.
- **Undo All Changes**: puts the project back to what it was when you opened it. You are asked
  first.
- **Autosave** and **Open Backup Folder**: see below.
- **Exit**: closes MP3 to MP4.

The title bar shows the name of the open project, for example `My Song.json (Autosave) - MP3 to MP4`.

### Autosave

With **File > Autosave** checked, every change you make to an open project is saved to its file
about a second later, and the title bar shows **(Autosave)**. You are not asked anything when you
close the window. Autosave is on from the start.

With **Autosave** unchecked, the project is only saved when you choose **Save Project**. A star
after the name in the title bar (`My Song.json*`) means there are changes that are not saved, and
you are asked whether to save them when you close the window, open another project or choose
**New Project**.

Work that has not been saved as a project yet is never saved by itself, and you are never asked
about it.

To try something out without changing your project, uncheck **Autosave** first, or make a copy with
**Save Project As...** and try it there. If you changed the project by mistake, use
**Undo All Changes** or a backup copy.

### Backup Copies

Just before an opened project is changed for the first time, a copy of the file as it was is saved
in `%AppData%\SweWolfSoftware\MP3toMP4\Backups`. The copy has the date and time in its name, for
example `My Song 2026-11-09 14.32.json`. The same happens to a file that you replace with
**Save Project As...**. Backup copies are deleted automatically after 30 days.

**File > Open Backup Folder** opens that folder. The copies are normal project files: open one
with **Open Project...**, and use **Save Project As...** to put it back where you want it.

### The Project File

A project file is a text file in the JSON format, so you can look at it in a text editor or a web
browser. Its name ends with `.json` unless you choose something else in
[Settings](#projects-1). It starts with the line `"app": "SweWolf MP3 to MP4"`. That line, not the
file name, is how MP3 to MP4 recognizes its projects.

Every file is saved with its full path and also with its path relative to the project file. If you
move or rename a folder that holds both the project and its files, the project still finds them.
When files can't be found, you are told which ones when you open the project.

You can also start the program with a project file, see [Create Shortcut](#create-shortcut).

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

### Projects

*New in the next version.* See [Projects](#projects).

- **File Extension**: the ending of the file name that **Save Project As...** suggests. Choose
  `.json`, `.mp3tomp4.json` or `.mp3tomp4` from the list, or type your own. It makes no
  difference when a project is opened.
- **Default Project Folder**: the folder where **Save Project As...** and **Open Project...**
  start. Leave it empty to start in the folder of the audio file. Click **...** to browse for a
  folder.
- **Autosave Projects**: the same switch as **File > Autosave**.
- **Recent Projects to Show**: how many projects **File > Recent Projects** lists, from 0 to 9.
  With 0 the menu item is hidden. The last nine projects are always remembered, so if you
  choose a higher number later, the older ones are shown again.

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

or with a [project](#projects) file, which is then opened (new in the next version):

```
MP3toMP4.exe "C:\Music\My Song.json"
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
| **Ctrl+N** | New Project |
| **Ctrl+S** | Save Project |
| **Ctrl+Shift+S** | Save Project As |
| **Ctrl+E** | Convert |
| **Ctrl+O** | Open File (the finished video) |
| **Ctrl+Shift+O** | Open Folder |
| **Alt+B** | Browse for an audio file |
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
