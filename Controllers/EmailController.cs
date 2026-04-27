using Microsoft.AspNetCore.Mvc;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.Services;
using HattmakarenWebbAppGrupp03.Data;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class EmailController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _emailService;

        public EmailController(ApplicationDbContext context)
        {
            _context = context;
            _emailService = new EmailService(); 
        }

        public async Task<IActionResult> Index()
        {
            // Hämtar alla sparade mail från databasen, nyaste först
            var email = await _context.Email
                .OrderByDescending(e => e.ReceivedDate)
                .ToListAsync();
            await SyncEmails();
            return View(email);

        }

        public async Task<IActionResult> SyncEmails()
        {
            var latestEmails = await _emailService.GetLatestEmailsAsync();

            foreach (var email in latestEmails)
            {
                // Kolla om mailet redan finns i DB via MessageId så vi inte dubblerar
                if (!await _context.Email.AnyAsync(e => e.MessageId == email.MessageId))
                {
                    _context.Email.Add(email);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var email = await _context.Email
                .FirstOrDefaultAsync(m => m.Id == id);
            email.IsRead = true; 
            _context.Email.Update(email);
            await _context.SaveChangesAsync();


            if (email == null)
            {
                return NotFound();
            }

            return View(email);
        }
    }
}