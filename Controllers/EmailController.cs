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

        // Enkel metod för att visa innehållet i ett specifikt mail
        public async Task<IActionResult> Details(int id)
        {
            var email = await _context.Email.FirstOrDefaultAsync(e => e.Id == id);
            if (email == null) return NotFound();

            email.IsRead = true; // Markera som läst när man öppnar det
            await _context.SaveChangesAsync();

            return View(email);
        }
    }
}