namespace EventDrivenArchDemo.Api.Domain
{
    /// <summary>
    /// Represents a book in the system.
    /// </summary>
    public class Book
    {
        /// <summary>
        /// Gets or sets the unique identifier for the book.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the title for the book.
        /// </summary>
        public required string Title { get; set; }
        /// <summary>
        /// Gets or sets the <see cref="Author"> of the book.
        /// </summary>
        public required Author Author { get; set; }
    }
}
