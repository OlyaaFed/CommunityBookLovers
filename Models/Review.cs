using System;
using System.Collections.Generic;

namespace CommunityBookLovers;

public partial class Review
{
    public int ReviewId { get; set; }

    public int? UserId { get; set; }

    public int? BookId { get; set; }

    public int? Rating { get; set; }

    public string? Text { get; set; }

    public DateOnly? Date { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? User { get; set; }
}
