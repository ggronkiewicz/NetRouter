namespace NetRouter.Message
{
    using System.Collections.Generic;
    using System.IO;
    using global::NetRouter.Abstraction;

    internal class DefaultResponse : IResponse
    {
        public DefaultResponse()
        {
            this.Headers = new Dictionary<string, IEnumerable<string>>();
        }

        public ushort Status { get; set; }

        public IDictionary<string, IEnumerable<string>> Headers { get; set; }

        public Stream Body { get; set; }
    }
}
