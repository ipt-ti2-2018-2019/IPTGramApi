using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPTGram.Data
{
    public class Post
    {
        public Post()
        {
            Likes = new HashSet<PostLike>();
            Comments = new HashSet<Comment>();
        }

        [Key]
        public long Id { get; set; }

        public string Caption { get; set; }

        public DateTimeOffset PostedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public string ImageFileName { get; set; }

        [Required]
        public string ImageContentType { get; set; }

        [Required]
        public string UserId { get; set; }

        public ICollection<PostLike> Likes { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}