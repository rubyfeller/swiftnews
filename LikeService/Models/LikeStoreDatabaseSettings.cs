namespace LikeService.Models
{
    public class LikeStoreDatabaseSettings : ILikeStoreDatabaseSettings
    {
        public string LikeCollectionName { get; set; } = string.Empty;
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}