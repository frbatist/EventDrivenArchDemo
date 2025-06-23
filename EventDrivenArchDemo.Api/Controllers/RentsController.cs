using EventDrivenArchDemo.Api.Data;
using EventDrivenArchDemo.Api.Domain;
using EventDrivenArchDemo.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var book = await _context.Books.FindAsync(request.BookId);
            if (book == null)
                return NotFound($"Book with Id {request.BookId} not found.");

            
            var client = await _context.Clients.FindAsync(request.UserId);
            if (client == null)
                return NotFound($"Client with Id {request.UserId} not found.");

            // Create Rent
            var rent = new Rent
            {
                Book = book,
                Client = client
            };

            _context.Rents.Add(rent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Post), new { id = rent.Id }, rent);
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentDetailResponse>>> Get()
        {
            var rents = await _context.Rents
                .Select(r => new RentDetailResponse
                {
                    RentId = r.Id,
                    BookId = r.Book.Id,
                    BookTitle = r.Book.Title,
                    AuthorId = r.Book.AuthorId,
                    AuthorName = r.Book.Author != null ? r.Book.Author.Name : string.Empty,
                    ClientId = r.Client.Id,
                    ClientName = r.Client.Name
                })
                .ToListAsync();

            return Ok(rents);
        }
    }
}
