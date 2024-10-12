using FuCommunityWebModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuCommunityWebModels.ViewModels
{
    public class PostVM
    {
        public List<Post> Posts { get; set; } = new List<Post>();
        public List<Category> Categories { get; set; } = new List<Category>();
        public CreatePostVM CreatePostVM { get; set; } = new CreatePostVM();
        public CategoryVM CategoryVM { get; set; } = new CategoryVM();
    }
}
