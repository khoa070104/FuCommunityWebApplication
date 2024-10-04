using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebModels.Models
{
    public class PostHistoryViewModel
    {
        public ApplicationUser User { get; set; } // Thông tin người dùng
        public List<Post> Post { get; set; }
    }

}
