using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebModels.Models
{
    public class MentorViewModel
    {
        public List<ApplicationUser> TopMentors { get; set; }
        public List<ApplicationUser> OtherMentors { get; set; }
    }
}
