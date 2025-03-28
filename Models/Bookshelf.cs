using System;
using System.Collections.Generic;

namespace CommunityBookLovers;

public partial class Bookshelf
{
    public int Bookshelf1 { get; set; }

    public int? BookId { get; set; }

    public DateOnly? DataAdded { get; set; }

    public int? UserId { get; set; }

    public string? State { get; set; }

    public virtual Book? Book { get; set; }

    public virtual User? User { get; set; }
}
