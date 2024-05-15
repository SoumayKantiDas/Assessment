using System;
using System.Collections.Generic;

namespace Assessment.Models;

public partial class Video
{
    public int VideoId { get; set; }

    public int ProductId { get; set; }

    public string Title { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public int SizeInBytes { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual Product Product { get; set; } = null!;
}
