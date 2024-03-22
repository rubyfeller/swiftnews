namespace LikeService.Models
{
    public interface ILikeStoreDatabaseSettings
    {
        string LikeCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}