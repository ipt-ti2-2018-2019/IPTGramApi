using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IPTGram.Data
{
    public class PostLike
    {
        [Required]
        public long PostId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }
    }
}