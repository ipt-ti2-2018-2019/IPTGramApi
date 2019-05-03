using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPTGram.Data
{
    public class Comment
    {
        [Key]
        public long Id { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public DateTimeOffset PostedAt { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        [Required]
        public long PostId { get; set; }
    }
}