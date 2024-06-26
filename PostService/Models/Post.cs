using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostService.Models;

[Table("posts")]
public class Post
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]

    [Column("id")]
    public int Id { get; set; }
    [Column("content")]
    public string Content { get; set; }
    [Column("like_count")]
    public int LikeCount { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("user_id")]
    public string UserId { get; set; }

}