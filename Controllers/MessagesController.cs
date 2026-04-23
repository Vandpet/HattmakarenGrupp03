using HattmakarenWebbAppGrupp03.Data;
using HattmakarenWebbAppGrupp03.Models;
using HattmakarenWebbAppGrupp03.ViewModels.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HattmakarenWebbAppGrupp03.Controllers
{
    public class MessagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentEmployeeId()
        {
            return HttpContext.Session.GetInt32("EmployeeId");
        }

        public async Task<IActionResult> Index()
        {
            int? currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var conversations = await _context.ConversationParticipants
                .Where(cp => cp.EmployeeId == currentEmployeeId.Value && !cp.IsArchived)
                .Select(cp => new
                {
                    Participant = cp,
                    Conversation = cp.Conversation,
                    LastMessage = cp.Conversation.Messages
                        .OrderByDescending(m => m.SentAtUtc)
                        .Select(m => new
                        {
                            m.Id,
                            m.Content,
                            m.SentAtUtc,
                            SenderName = m.SenderEmployee.Name,
                            m.SenderEmployeeId
                        })
                        .FirstOrDefault(),
                    OtherParticipants = cp.Conversation.Participants
                        .Where(p => p.EmployeeId != currentEmployeeId.Value)
                        .Select(p => p.Employee.Name)
                        .ToList()
                })
                .OrderByDescending(x => x.LastMessage != null ? x.LastMessage.SentAtUtc : DateTime.MinValue)
                .ToListAsync();

            var viewModel = conversations.Select(x =>
            {
                string displayName;

                if (!string.IsNullOrWhiteSpace(x.Conversation.Title))
                {
                    displayName = x.Conversation.Title;
                }
                else if (x.OtherParticipants.Count == 0)
                {
                    displayName = "Bara du";
                }
                else if (x.OtherParticipants.Count == 1)
                {
                    displayName = x.OtherParticipants.First();
                }
                else
                {
                    displayName = string.Join(", ", x.OtherParticipants);
                }

                int unreadCount = x.Conversation.Messages.Count(m =>
                    m.SentAtUtc > (x.Participant.LastReadAtUtc ?? DateTime.MinValue)
                    && m.SenderEmployeeId != currentEmployeeId.Value);

                return new ConversationListItemViewModel
                {
                    ConversationId = x.Conversation.Id,
                    DisplayName = displayName,
                    LastMessagePreview = x.LastMessage?.Content.Length > 80
                        ? x.LastMessage.Content.Substring(0, 80) + "..."
                        : x.LastMessage?.Content ?? "",
                    LastSenderName = x.LastMessage?.SenderName ?? "",
                    LastMessageSentAtUtc = x.LastMessage?.SentAtUtc,
                    UnreadCount = unreadCount,
                    IsBroadcast = x.Conversation.IsBroadcast
                };
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Thread(int id)
        {
            int? currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            bool isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == id && cp.EmployeeId == currentEmployeeId.Value);

            if (!isParticipant)
            {
                return Forbid();
            }

            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                    .ThenInclude(p => p.Employee)
                .Include(c => c.Messages)
                    .ThenInclude(m => m.SenderEmployee)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (conversation == null)
            {
                return NotFound();
            }

            var currentParticipant = await _context.ConversationParticipants
                .FirstAsync(cp => cp.ConversationId == id && cp.EmployeeId == currentEmployeeId.Value);

            currentParticipant.LastReadAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            string displayName;
            var otherParticipants = conversation.Participants
                .Where(p => p.EmployeeId != currentEmployeeId.Value)
                .Select(p => p.Employee.Name)
                .ToList();

            if (!string.IsNullOrWhiteSpace(conversation.Title))
            {
                displayName = conversation.Title;
            }
            else if (otherParticipants.Count == 0)
            {
                displayName = "Bara du";
            }
            else if (otherParticipants.Count == 1)
            {
                displayName = otherParticipants.First();
            }
            else
            {
                displayName = string.Join(", ", otherParticipants);
            }

            var viewModel = new ConversationThreadViewModel
            {
                ConversationId = conversation.Id,
                DisplayName = displayName,
                Participants = conversation.Participants
                    .Select(p => p.Employee.Name)
                    .OrderBy(n => n)
                    .ToList(),
                Messages = conversation.Messages
                    .OrderBy(m => m.SentAtUtc)
                    .Select(m => new MessageItemViewModel
                    {
                        MessageId = m.Id,
                        SenderName = m.SenderEmployee.Name,
                        Content = m.Content,
                        SentAtUtc = m.SentAtUtc,
                        IsMine = m.SenderEmployeeId == currentEmployeeId.Value
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int conversationId, string newMessageContent)
        {
            int? currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(newMessageContent))
            {
                TempData["ErrorMessage"] = "Meddelandet får inte vara tomt.";
                return RedirectToAction(nameof(Thread), new { id = conversationId });
            }

            bool isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.EmployeeId == currentEmployeeId.Value);

            if (!isParticipant)
            {
                return Forbid();
            }

            var message = new Message
            {
                ConversationId = conversationId,
                SenderEmployeeId = currentEmployeeId.Value,
                Content = newMessageContent.Trim(),
                SentAtUtc = DateTime.UtcNow,
                IsSystemMessage = false
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id = conversationId });
        }

        public async Task<IActionResult> Create()
        {
            int? currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var employees = await _context.Employees
                .Where(e => !e.IsDeleted && e.EId != currentEmployeeId.Value)
                .OrderBy(e => e.Name)
                .Select(e => new EmployeeSelectionViewModel
                {
                    EmployeeId = e.EId,
                    FullName = e.Name
                })
                .ToListAsync();

            var viewModel = new CreateConversationViewModel
            {
                AvailableEmployees = employees
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateConversationViewModel model)
        {
            int? currentEmployeeId = GetCurrentEmployeeId();
            if (currentEmployeeId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var availableEmployees = await _context.Employees
                .Where(e => !e.IsDeleted && e.EId != currentEmployeeId.Value)
                .OrderBy(e => e.Name)
                .Select(e => new EmployeeSelectionViewModel
                {
                    EmployeeId = e.EId,
                    FullName = e.Name
                })
                .ToListAsync();

            model.AvailableEmployees = availableEmployees;

            List<int> recipientIds;

            if (model.SendToAll)
            {
                recipientIds = await _context.Employees
                    .Where(e => !e.IsDeleted && e.EId != currentEmployeeId.Value)
                    .Select(e => e.EId)
                    .ToListAsync();
            }
            else
            {
                recipientIds = model.SelectedEmployeeIds
                    .Distinct()
                    .Where(id => id != currentEmployeeId.Value)
                    .ToList();
            }

            if (!recipientIds.Any())
            {
                ModelState.AddModelError(string.Empty, "Du måste välja minst en mottagare, eller använda 'Skicka till alla'.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!model.SendToAll && recipientIds.Count == 1)
            {
                int recipientId = recipientIds[0];

                var existingConversationId = await _context.Conversations
                    .Where(c => !c.IsBroadcast)
                    .Where(c => c.Participants.Count == 2)
                    .Where(c => c.Participants.Any(p => p.EmployeeId == currentEmployeeId.Value))
                    .Where(c => c.Participants.Any(p => p.EmployeeId == recipientId))
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync();

                if (existingConversationId != 0)
                {
                    var newMessage = new Message
                    {
                        ConversationId = existingConversationId,
                        SenderEmployeeId = currentEmployeeId.Value,
                        Content = model.Content.Trim(),
                        SentAtUtc = DateTime.UtcNow,
                        IsSystemMessage = false
                    };

                    _context.Messages.Add(newMessage);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Thread), new { id = existingConversationId });
                }
            }

            var conversation = new Conversation
            {
                Title = string.IsNullOrWhiteSpace(model.Title) ? null : model.Title.Trim(),
                IsBroadcast = model.SendToAll,
                CreatedAtUtc = DateTime.UtcNow,
                CreatedByEmployeeId = currentEmployeeId.Value
            };

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            var participantIds = recipientIds.Append(currentEmployeeId.Value).Distinct().ToList();

            foreach (var participantId in participantIds)
            {
                _context.ConversationParticipants.Add(new ConversationParticipant
                {
                    ConversationId = conversation.Id,
                    EmployeeId = participantId,
                    JoinedAtUtc = DateTime.UtcNow,
                    LastReadAtUtc = participantId == currentEmployeeId.Value ? DateTime.UtcNow : null,
                    IsArchived = false
                });
            }

            _context.Messages.Add(new Message
            {
                ConversationId = conversation.Id,
                SenderEmployeeId = currentEmployeeId.Value,
                Content = model.Content.Trim(),
                SentAtUtc = DateTime.UtcNow,
                IsSystemMessage = false
            });

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Thread), new { id = conversation.Id });
        }
    }
}