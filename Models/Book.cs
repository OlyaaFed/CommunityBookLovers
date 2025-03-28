using System;
using System.Collections.Generic;

namespace CommunityBookLovers;

public partial class Book
{
    public int BookId { get; set; }

    public string? Title { get; set; }

    public int? AuthorId { get; set; }

    public int? GenreId { get; set; }

    public int? PublicationYear { get; set; }
    public string PathToFile { get; set; }
    public int? Pages { get; set; }
    public string? ImagePath { get; set; }
    public string? Description { get; set; }
    
    public virtual Author? Author { get; set; }

    public string State { get; set; } = "Без статуса";
    public virtual ICollection<Bookshelf> Bookshelves { get; set; } = new List<Bookshelf>();

    public virtual Genre? Genre { get; set; }

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
