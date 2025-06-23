namespace EventDrivenArchDemo.Api.Domain
{
    /// <summary>
    /// Represents a book rental in the system.
    /// </summary>
    public class Rent
    {
        /// <summary>
        /// Represents a book rental in the system.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for the <see cref="Book"/> being rented.
        /// </summary>
        public required Book Book { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the <see cref="Client"/> who is renting the book.
        /// </summary>
        public required Client Client { get; set; }
    }
}
