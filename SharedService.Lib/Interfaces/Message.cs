using SharedService.Lib.PubSub;

namespace SharedService.Lib.Interfaces
{
    public record Message<T> where T : class
    {
        public ProductEvent Action { get; set; }
        public T Payload { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
