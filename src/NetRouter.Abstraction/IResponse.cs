namespace NetRouter.Abstraction
{
    using System.Collections.Generic;
    using System.IO;

    public interface IResponse
    {
        ushort Status { get; set; }

        IDictionary<string, IEnumerable<string>> Headers { get; }

        Stream Body { get; set; }
    }
}