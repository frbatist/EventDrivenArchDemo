namespace EventDrivenArchDemo.Api.Domain
{
    /// <summary>
    /// Represents a client in the system.
    /// </summary>
    public class Client
    {
        /// <summary>
        /// Gets or sets the unique identifier for the client.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the client.
        /// </summary>
        public required string Name { get; set; }

    }
}
