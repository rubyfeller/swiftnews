using System.ComponentModel.DataAnnotations;

namespace PostService.Dtos;

public class PostCreateDTO {
    [Required]
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    [Required]
    public required string Author { get; set; }
}