using System.ComponentModel.DataAnnotations;

namespace PostService.Dtos;

public class PostCreateDTO
{
    [Required]
    [MinLength(5)]
    [MaxLength(280)]
    public required string Content { get; set; }
    public DateTime CreatedAt { get; set; }
}