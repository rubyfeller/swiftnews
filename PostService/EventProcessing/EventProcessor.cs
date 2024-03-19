namespace PostService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        public void ProcessEvent(string message)
        {
            throw new NotImplementedException();
        }
        
        public void AddLike() {
            Console.WriteLine("Added like");
        }
    }

}