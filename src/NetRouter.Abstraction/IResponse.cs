namespace NetRouter.Abstraction
{
    using System.Collections.Generic;

    public interface IResponse
    {
        ushort Status { get; set; }

        IDictionary<string, IEnumerable<string>> Headers { get; }

        IMessageBody Body { get; }
    }
}