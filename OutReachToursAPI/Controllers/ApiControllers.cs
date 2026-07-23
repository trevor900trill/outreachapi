using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OutReachToursAPI.Data;
using OutReachToursAPI.Models;

namespace OutReachToursAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            return await _context.Clients.Include(c => c.Activities).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Client>> CreateClient(Client client)
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetClients), new { id = client.Id }, client);
        }

        [HttpPut("{id}/stage")]
        public async Task<IActionResult> UpdateClientStage(string id, [FromQuery] string stageId)
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null) return NotFound();

            client.StageId = stageId;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ToursController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ToursController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tour>>> GetTours()
        {
            return await _context.Tours.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Tour>> CreateTour(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTours), new { id = tour.Id }, tour);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public UsersController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers() => await _context.Users.ToListAsync();
        
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromServices] OutReachToursAPI.Services.IEmailService emailService, User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Send Invite Email
            if (!string.IsNullOrEmpty(user.Email))
            {
                await emailService.SendEmailAsync(user.Email, "You've been invited to Outreach Tours CRM", 
                    "Welcome to Outreach Tours CRM! Log in at http://localhost:3000 to get started.", 
                    "<strong>Welcome to Outreach Tours CRM!</strong> <a href='http://localhost:3000'>Log in here</a> to get started.");
            }

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RolesController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomRole>>> GetRoles() => await _context.Roles.ToListAsync();

        [HttpPost]
        public async Task<ActionResult<CustomRole>> CreateRole(CustomRole role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRoles), new { id = role.Id }, role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, CustomRole roleUpdates)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            role.Name = roleUpdates.Name;
            role.Permissions = roleUpdates.Permissions;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null) return NotFound();
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public TransactionsController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<POSTransaction>>> GetTransactions() 
            => await _context.Transactions.OrderByDescending(t => t.Date).ToListAsync();

        [HttpPost]
        public async Task<ActionResult<POSTransaction>> CreateTransaction(
            [FromServices] OutReachToursAPI.Services.IEmailService emailService,
            [FromServices] OutReachToursAPI.Services.IPaymentService paymentService,
            POSTransaction tx)
        {
            _context.Transactions.Add(tx);
            await _context.SaveChangesAsync();

            var client = await _context.Clients.FindAsync(tx.ClientId);
            var tour = await _context.Tours.FindAsync(tx.TourId);
            var segment = tour?.Segment; // "Ultra Luxury", "Impact and Luxury", or "Impact"

            if (client != null && !string.IsNullOrEmpty(client.Email))
            {
                // Generate Paystack Link
                var paymentUrl = await paymentService.CreatePaymentLinkAsync(client.Email, (int)tx.AmountKES, tx.InvoiceNumber);

                // Send Email with Invoice and Payment Link
                var msg = $"Hello {client.Name},\n\nYour invoice {tx.InvoiceNumber} for {tx.AmountKES} KES has been generated. \nPay securely here: {paymentUrl}";
                var htmlMsg = $"<h3>Hello {client.Name},</h3><p>Your invoice <strong>{tx.InvoiceNumber}</strong> for {tx.AmountKES} KES is ready.</p><p><a href='{paymentUrl}'>Click here to pay securely via Paystack</a></p>";
                
                await emailService.SendEmailAsync(client.Email, $"Invoice {tx.InvoiceNumber} from Outreach Tours", msg, htmlMsg, segment);
            }

            return CreatedAtAction(nameof(GetTransactions), new { id = tx.Id }, tx);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class StagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public StagesController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PipelineStage>>> GetStages() 
            => await _context.Stages.OrderBy(s => s.Order).ToListAsync();
            
        [HttpPost]
        public async Task<ActionResult<PipelineStage>> CreateStage(PipelineStage stage)
        {
            _context.Stages.Add(stage);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetStages), new { id = stage.Id }, stage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStage(string id, PipelineStage updates)
        {
            var stage = await _context.Stages.FindAsync(id);
            if (stage == null) return NotFound();
            stage.Name = updates.Name;
            stage.Color = updates.Color;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStage(string id)
        {
            var stage = await _context.Stages.FindAsync(id);
            if (stage == null) return NotFound();
            _context.Stages.Remove(stage);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public NotificationsController(AppDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Notification>>> GetNotifications() 
            => await _context.Notifications.OrderByDescending(n => n.Time).ToListAsync();

        [HttpPost("mark-read")]
        public async Task<IActionResult> MarkAllRead()
        {
            var unread = await _context.Notifications.Where(n => !n.Read).ToListAsync();
            foreach (var n in unread) n.Read = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
