namespace PostService.Dtos;

public class PostReadDTO {
    public int Id { get; set; }
    public required string Content { get; set; }
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public required string Author { get; set; }
}