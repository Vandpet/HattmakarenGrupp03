using HattmakarenWebbAppGrupp03.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.ViewComponents
{
    public class UnreadMessagesViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public UnreadMessagesViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int? currentEmployeeId = HttpContext.Session.GetInt32("EmployeeId");

            if (currentEmployeeId == null)
            {
                return View(0);
            }

            var unreadCount = await _context.ConversationParticipants
                .Where(cp => cp.EmployeeId == currentEmployeeId.Value && !cp.IsArchived)
                .SelectMany(cp => cp.Conversation.Messages
                    .Where(m =>
                        m.SentAtUtc > (cp.LastReadAtUtc ?? DateTime.MinValue) &&
                        m.SenderEmployeeId != currentEmployeeId.Value))
                .CountAsync();

            return View(unreadCount);
        }
    }
}