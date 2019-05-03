using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace IPTGram.Data
{
    public class User : IdentityUser
    {
        public User() : this(null)
        {
        }

        public User(string userName) : base(userName)
        {
            Posts = new HashSet<Post>();
            Likes = new HashSet<PostLike>();
            Comments = new HashSet<Comment>();
        }

        [Required]
        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public ICollection<Post> Posts { get; set; }

        public ICollection<PostLike> Likes { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}