using FuCommunityWebModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebModels.ViewModels
{
    public class ForumVM
    {
        public List<Course> Courses { get; set; }
        public List<Category> Categories { get; set; }  
    }
}
