using EventDrivenArchDemo.Api.Data;
using EventDrivenArchDemo.Api.Domain;
using EventDrivenArchDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventDrivenArchDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentsController : ControllerBase
    {
        private readonly BookRentalShopContext _context;

        public RentsController(BookRentalShopContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RentRequest request)
        {
            // Validate Book
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
                return NotFound($"Book with Id {request.BookId} not found.");

            // Validate Client
            var client = await _context.Clients.FindAsync(request.UserId);
            if (client == null)
                return NotFound($"Client with Id {request.UserId} not found.");

            // Create Rent
            var rent = new Rent
            {
                Book = book
                // Add other properties if needed
            };

            _context.Rents.Add(rent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), new { id = rent.Id }, rent);
        }
    }
}
