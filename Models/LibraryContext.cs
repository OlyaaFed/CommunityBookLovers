using System;
using System.Collections.Generic;
using CommunityBookLovers.Models;
using Microsoft.EntityFrameworkCore;

namespace CommunityBookLovers;

public partial class LibraryContext : DbContext
{
    public LibraryContext()
    {
    }

    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Bookshelf> Bookshelves { get; set; }

    public virtual DbSet<Friendship> Friendships { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Message> Messages { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder.UseSqlServer("Server=DESKTOP-MN04145\\SQLEXPRESS;Database=Library;TrustServerCertificate=True;Trusted_Connection=True");
    //=> optionsBuilder.UseSqlServer("Server=44-4\\SQLEXPRESS;Database=Library;TrustServerCertificate=True;Trusted_Connection=True");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.ToTable("Author");

            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.Pages).HasColumnName("pages");
            entity.Property(e => e.PublicationYear).HasColumnName("publication_year");
            entity.Property(e => e.Title)
                .IsUnicode(false)
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Books_Author");

            entity.HasOne(d => d.Genre).WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_Books_Genre");
        });

        modelBuilder.Entity<Bookshelf>(entity =>
        {
            entity.HasKey(e => e.Bookshelf1);

            entity.ToTable("Bookshelf");

            entity.Property(e => e.Bookshelf1).HasColumnName("bookshelf");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.DataAdded).HasColumnName("data_added");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("state");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Book).WithMany(p => p.Bookshelves)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_Bookshelf_Books");

            entity.HasOne(d => d.User).WithMany(p => p.Bookshelves)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Bookshelf_User");
        });

        modelBuilder.Entity<Friendship>(entity =>
        {
            entity.ToTable("Friendship");

            entity.Property(e => e.FriendshipId).HasColumnName("friendship_id");
            entity.Property(e => e.FriendId).HasColumnName("friend_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Friend).WithMany(p => p.FriendshipFriends)
                .HasForeignKey(d => d.FriendId)
                .HasConstraintName("FK_Friendship_User1");

            entity.HasOne(d => d.User).WithMany(p => p.FriendshipUsers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Friendship_User");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK_genre");

            entity.ToTable("Genre");

            entity.Property(e => e.GenreId).HasColumnName("genre_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review");

            entity.Property(e => e.ReviewId).HasColumnName("review_id");
            entity.Property(e => e.BookId).HasColumnName("book_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.Text)
                .IsUnicode(false)
                .HasColumnName("text");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Book).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK_Review_Books");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Review_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(250)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("password");
        });
        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Messages");

            entity.HasKey(e => e.MessageId);

            entity.Property(e => e.MessageId).HasColumnName("message_id");
            entity.Property(e => e.SenderId).HasColumnName("sender_id");
            entity.Property(e => e.ReceiverId).HasColumnName("receiver_id");
            entity.Property(e => e.Content)
                  .IsUnicode(false)
                  .HasColumnName("content");
            entity.Property(e => e.Timestamp)
                  .HasColumnName("timestamp");

            entity.HasOne(d => d.Sender)
                  .WithMany(p => p.SentMessages)
                  .HasForeignKey(d => d.SenderId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_Message_Sender");

            entity.HasOne(d => d.Receiver)
                  .WithMany(p => p.ReceivedMessages)
                  .HasForeignKey(d => d.ReceiverId)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK_Message_Receiver");
        });



        OnModelCreatingPartial(modelBuilder);
    }

    internal void AcceptFriendRequest(User? user, User? friend)
    {
        throw new NotImplementedException();
    }

    internal object GetFriends(User currentUser)
    {
        throw new NotImplementedException();
    }

    internal object GetPendingFriendRequests(User currentUser)
    {
        throw new NotImplementedException();
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
