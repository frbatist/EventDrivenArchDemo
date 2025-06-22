using Microsoft.EntityFrameworkCore;
using EventDrivenArchDemo.Api.Domain;

namespace EventDrivenArchDemo.Api.Data
{
    public class BookRentalShopContext : DbContext
    {
        public BookRentalShopContext(DbContextOptions<BookRentalShopContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Rent> Rents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Authors
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, Name = "George Orwell" },
                new Author { Id = 2, Name = "Jane Austen" },
                new Author { Id = 3, Name = "J.K. Rowling" },
                new Author { Id = 4, Name = "Mark Twain" },
                new Author { Id = 5, Name = "Agatha Christie" }
            );

            // Seed Books
            modelBuilder.Entity<Book>().HasData(
                new Book { Id = 1, Title = "1984", AuthorId = 1 },
                new Book { Id = 2, Title = "Animal Farm", AuthorId = 1 },
                new Book { Id = 3, Title = "Pride and Prejudice", AuthorId = 2 },
                new Book { Id = 4, Title = "Harry Potter and the Sorcerer's Stone", AuthorId = 3 },
                new Book { Id = 5, Title = "Harry Potter and the Chamber of Secrets", AuthorId = 3 },
                new Book { Id = 6, Title = "Adventures of Huckleberry Finn", AuthorId = 4 },
                new Book { Id = 7, Title = "Murder on the Orient Express", AuthorId = 5 }
            );

            // Seed Clients
            modelBuilder.Entity<Client>().HasData(
                new Client { Id = 1, Name = "Alice Johnson" },
                new Client { Id = 2, Name = "Bob Smith" },
                new Client { Id = 3, Name = "Charlie Brown" },
                new Client { Id = 4, Name = "Diana Prince" },
                new Client { Id = 5, Name = "Ethan Hunt" }
            );
        }
    }
}
