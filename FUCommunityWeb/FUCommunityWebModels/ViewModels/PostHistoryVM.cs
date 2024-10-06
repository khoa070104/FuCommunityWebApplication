using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuCommunityWebModels.Models;

namespace FuCommunityWebModels.ViewModels
{
    public class PostHistoryVM
    {
        public ApplicationUser User { get; set; } // Thông tin người dùng
        public List<Post> Post { get; set; }
    }

}
