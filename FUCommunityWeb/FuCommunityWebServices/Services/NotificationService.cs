using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class NotificationService
{
    private readonly ApplicationDbContext _context;

    public NotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateCommentNotification(string fromUserId, string toUserId, int postId, string message)
    {
        var notification = new Notification
        {
            UserID = toUserId,
            FromUserID = fromUserId,
            PostID = postId,
            Message = message,
            NotificationType = "Comment"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task CreateReplyNotification(string fromUserId, string toUserId, int postId, string message)
    {
        var notification = new Notification
        {
            UserID = toUserId,
            FromUserID = fromUserId,
            PostID = postId,
            Message = message,
            NotificationType = "Reply"
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Notification>> GetUnreadNotifications(string userId)
    {
        return await _context.Notifications
            .Include(n => n.FromUser)
            .Include(n => n.Post)
            .Where(n => n.UserID == userId && !n.IsRead)
            .OrderByDescending(n => n.CreatedDate)
            .Take(10)
            .Select(n => new Notification
            {
                NotificationID = n.NotificationID,
                Message = n.Message,
                CreatedDate = n.CreatedDate,
                IsRead = n.IsRead,
                PostID = n.PostID,
                FromUser = new ApplicationUser
                {
                    FullName = n.FromUser.FullName,
                    AvatarImage = n.FromUser.AvatarImage
                },
                Post = new Post
                {
                    Title = n.Post.Title
                }
            })
            .ToListAsync();
    }

    public async Task MarkAsRead(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification != null)
        {
            notification.IsRead = true;
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetUnreadCount(string userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserID == userId && !n.IsRead);
    }
} 