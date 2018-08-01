namespace NetRouter.Abstraction
{
    using System.IO;

    public interface IMessageBody
    {
        Stream Content { get; set; }
    }
}