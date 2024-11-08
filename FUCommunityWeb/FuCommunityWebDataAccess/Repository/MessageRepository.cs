using FuCommunityWebDataAccess.Data;
using FuCommunityWebModels.Models;

namespace FuCommunityWebDataAccess.Repository
{
    public class MessageRepository 
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

    }
} 