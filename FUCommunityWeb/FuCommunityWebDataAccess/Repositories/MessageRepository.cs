using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FuCommunityWebDataAccess.Repositories
{
    public class MessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetMessagesBetweenUsers(string userId1, string userId2)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId1 && m.ReceiverId == userId2 ||
                           m.SenderId == userId2 && m.ReceiverId == userId1)
                .OrderBy(m => m.CreatedDate)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetUserChatList(string userId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<int> GetUnreadMessageCount(string userId)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        public async Task<Message> CreateMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task MarkMessagesAsRead(string senderId, string receiverId)
        {
            var unreadMessages = await _context.Messages
                .Where(m => m.SenderId == senderId &&
                           m.ReceiverId == receiverId &&
                           !m.IsRead)
                .ToListAsync();

            foreach (var message in unreadMessages)
            {
                message.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChatListItem>> GetChatListItems(string userId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId || m.ReceiverId == userId)
                .Select(m => m.SenderId == userId ? m.ReceiverId : m.SenderId)
                .Distinct()
                .Select(chatUserId => new ChatListItem
                {
                    UserId = chatUserId,
                    FullName = _context.Users.Where(u => u.Id == chatUserId).Select(u => u.FullName).FirstOrDefault(),
                    AvatarImage = _context.Users.Where(u => u.Id == chatUserId).Select(u => u.AvatarImage).FirstOrDefault(),
                    LastMessage = _context.Messages
                        .Where(m => m.SenderId == userId && m.ReceiverId == chatUserId ||
                                   m.SenderId == chatUserId && m.ReceiverId == userId)
                        .OrderByDescending(m => m.CreatedDate)
                        .Select(m => m.Content)
                        .FirstOrDefault(),
                    UnreadCount = _context.Messages
                        .Count(m => m.SenderId == chatUserId &&
                                   m.ReceiverId == userId &&
                                   !m.IsRead)
                })
                .ToListAsync();
        }
    }
}