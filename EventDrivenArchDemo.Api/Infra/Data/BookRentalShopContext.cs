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
    }
}
