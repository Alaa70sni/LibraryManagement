using Library_Management_System.Models;
using Library_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;



namespace Library_Management_System.Data
{
public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Librarian> Librarians { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }






    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {



        modelBuilder.Entity<Checkout>()
            .HasOne(c => c.Member)
            .WithMany(m => m.Checkouts) // This establishes the relationship
            .HasForeignKey(c => c.MemberId);

        modelBuilder.Entity<Checkout>()
            .HasOne(c => c.Book)
            .WithMany(b => b.Checkouts)
            .HasForeignKey(c => c.BookId);


        modelBuilder.Entity<BorrowedBook>()
            .HasOne(b => b.Book)
            .WithMany(b => b.BorrowedBooks)
            .HasForeignKey(b => b.BookId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Checkout>()
            .HasOne(c => c.Book)
            .WithMany(b => b.Checkouts)
            .HasForeignKey(c => c.BookId)
            .OnDelete(DeleteBehavior.Cascade); // This enables cascade delete


        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Book>().ToTable("Books");     
        modelBuilder.Entity<User>()
            .HasDiscriminator<string>("UserType")
            .HasValue<Member>("Member")
            .HasValue<Librarian>("Librarian");

        base.OnModelCreating(modelBuilder);

        
        // Configure Penalty property for BorrowedBook
        modelBuilder.Entity<BorrowedBook>()
            .Property(b => b.Penalty)
            .HasColumnType("decimal(18, 2)"); // Adjust precision and scale as needed

        // Configure Penalty property for Checkout
        modelBuilder.Entity<Checkout>()
            .Property(c => c.PenaltyExpenses)
            .HasColumnType("decimal(18, 2)"); // Adjust precision and scale as needed    

                    // Configure relationships if necessary
        modelBuilder.Entity<BorrowedBook>()
            .HasOne(b => b.Book)
            .WithMany() // Adjust if you have a collection of borrowed books in the Book class
            .HasForeignKey(b => b.BookId);
        modelBuilder.Entity<Checkout>()
            .HasKey(c => c.Id); // Define primary key

        modelBuilder.Entity<Checkout>()
            .HasOne(c => c.Member)
            .WithMany() // Assuming a one-to-many relationship
            .HasForeignKey(c => c.MemberId);


    }

    



    }
    

}
