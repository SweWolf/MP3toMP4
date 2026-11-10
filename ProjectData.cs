namespace MP3toMP4
{
    /// <summary>
    /// What an MP3 to MP4 project file contains: everything in the main window that describes
    /// the video to make. Not the values of the Settings window, and not the logs.
    /// The envelope ("app", "formatVersion", "appVersion") comes from ProjectFiles.cs.
    ///
    /// Times (Start at, End at, Start Time, Duration) are saved as the text in the window. That
    /// text does not depend on the Windows language: both "." and "," are accepted as the
    /// decimal separator when it is read.
    ///
    /// Adding a property is safe: an older version ignores what it doesn't know, and a property
    /// that is missing in an older file keeps the default given here.
    /// </summary>
    internal sealed class ProjectData : ProjectEnvelope
    {
        public ProjectFileRef AudioFile { get; set; } = new();
        public ProjectTrim Trim { get; set; } = new();

        /// <summary>The selected tab: "Single" or "Multiple".</summary>
        public string Tab { get; set; } = "Single";

        public ProjectSingle Single { get; set; } = new();

        /// <summary>The list on the Multiple Images/Videos tab.</summary>
        public List<ProjectRow> Rows { get; set; } = [];

        public ProjectEffects Effects { get; set; } = new();
        public bool IncludeLyrics { get; set; }
        public ProjectOutput Output { get; set; } = new();
    }

    /// <summary>Trim to a Range.</summary>
    internal sealed class ProjectTrim
    {
        public bool Enabled { get; set; }
        public string Start { get; set; } = "";
        public string End { get; set; } = "";
        public string FadeIn { get; set; } = "0";
        public string FadeOut { get; set; } = "0";
    }

    /// <summary>The Single Image/Video tab.</summary>
    internal sealed class ProjectSingle
    {
        /// <summary>"File", "Black" or "Text".</summary>
        public string Source { get; set; } = "File";
        public ProjectFileRef ImageFile { get; set; } = new();
        public bool UseImageFromAudioFile { get; set; }
        public string Text { get; set; } = "";
    }

    /// <summary>One row of the list on the Multiple Images/Videos tab.</summary>
    internal sealed class ProjectRow
    {
        public ProjectFileRef File { get; set; } = new();
        /// <summary>As typed; blank when the program works it out.</summary>
        public string StartTime { get; set; } = "";
        /// <summary>Only a Duration the user typed; blank when the program works it out.</summary>
        public string Duration { get; set; } = "";
    }

    /// <summary>The Effects box on the Multiple Images/Videos tab.</summary>
    internal sealed class ProjectEffects
    {
        /// <summary>"none", "random" or the xfade name of the transition.</summary>
        public string Transition { get; set; } = "none";
        public decimal TransitionSeconds { get; set; } = 0.5m;
        public bool SlowZoom { get; set; }
    }

    /// <summary>The Output box.</summary>
    internal sealed class ProjectOutput
    {
        public ProjectFileRef File { get; set; } = new();
        /// <summary>"Standard" or "High".</summary>
        public string VideoQuality { get; set; } = "Standard";
    }
}
