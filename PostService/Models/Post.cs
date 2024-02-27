using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostService.Models
{
    [Table("posts")]
    public class Post
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        [Column("id")]
        public int Id { get; set; }
        [Column("content")]
        public string Content { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        [Column("author")]
        public string Author { get; set; }
    }
}
