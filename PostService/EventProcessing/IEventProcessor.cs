namespace PostService.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message);
    }
}