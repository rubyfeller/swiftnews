namespace PostService.Messaging {
    public interface IRabbitMQProducer {
        public void SendTestMessage < T > (T message);
    }
}