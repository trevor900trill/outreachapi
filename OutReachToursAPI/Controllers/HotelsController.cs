using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Data;
using OutReachToursAPI.Models;
using OutReachToursAPI.Services;

namespace OutReachToursAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HotelsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IHotelAvailabilityService _hotelService;
        private readonly ILogger<HotelsController> _logger;

        public HotelsController(
            AppDbContext context, 
            IHotelAvailabilityService hotelService, 
            ILogger<HotelsController> logger)
        {
            _context = context;
            _hotelService = hotelService;
            _logger = logger;
        }

        /// <summary>
        /// Real-time live room availability search via Booking.com / RapidAPI live engine.
        /// </summary>
        [HttpGet("live-search")]
        public async Task<ActionResult<LiveHotelAvailabilityResult>> LiveSearch([FromQuery] LiveAvailabilityQuery query)
        {
            try
            {
                var result = await _hotelService.CheckAvailabilityAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing live hotel search for {CityCode}", query.CityCode);
                return StatusCode(500, new { message = "Failed to query live hotel room availability", error = ex.Message });
            }
        }

        /// <summary>
        /// Quick-select popular travel destinations across Kenya and East Africa.
        /// </summary>
        [HttpGet("destinations")]
        public ActionResult<IEnumerable<DestinationInfo>> GetDestinations()
        {
            return Ok(_hotelService.GetPopularDestinations());
        }

        /// <summary>
        /// Get all active calendar hotel room bookings and client reservation holds.
        /// </summary>
        [HttpGet("bookings")]
        public async Task<ActionResult<IEnumerable<HotelBooking>>> GetBookings()
        {
            var bookings = await _context.HotelBookings
                .OrderBy(b => b.CheckInDate)
                .ToListAsync();

            return Ok(bookings);
        }

        /// <summary>
        /// Book a hotel room or create a provisional reservation hold for a client.
        /// </summary>
        [HttpPost("bookings")]
        public async Task<ActionResult<HotelBooking>> CreateBooking(HotelBooking booking)
        {
            if (string.IsNullOrWhiteSpace(booking.Id))
            {
                booking.Id = $"hb_{Guid.NewGuid():N}";
            }

            if (string.IsNullOrWhiteSpace(booking.VoucherNumber))
            {
                booking.VoucherNumber = $"VCH-{DateTime.UtcNow:yyyyMM}-{Random.Shared.Next(1000, 9999)}";
            }

            if (string.IsNullOrWhiteSpace(booking.CreatedAt))
            {
                booking.CreatedAt = DateTime.UtcNow.ToString("o");
            }

            _context.HotelBookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookings), new { id = booking.Id }, booking);
        }

        /// <summary>
        /// Cancel or release a hotel room hold.
        /// </summary>
        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> DeleteBooking(string id)
        {
            var booking = await _context.HotelBookings.FindAsync(id);
            if (booking == null) return NotFound(new { message = "Booking not found" });

            _context.HotelBookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Get saved contracted partner properties.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hotel>>> GetHotels()
        {
            return await _context.Hotels
                .OrderBy(h => h.Destination)
                .ThenBy(h => h.Name)
                .ToListAsync();
        }

        /// <summary>
        /// Add or register a partner hotel in the system database.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Hotel>> CreateHotel(Hotel hotel)
        {
            if (string.IsNullOrWhiteSpace(hotel.Id))
            {
                hotel.Id = $"h_{Guid.NewGuid():N}";
            }

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetHotels), new { id = hotel.Id }, hotel);
        }
    }
}
