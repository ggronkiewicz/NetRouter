namespace NetRouter.Message
{
    using global::NetRouter.Abstraction;
    using System.IO;

    public class StreamMessageBody : IMessageBody
    {
        public StreamMessageBody(Stream stream)
        {
            this.Content = stream;
        }

        public Stream Content { get; set; }
    }
}