namespace NetRouter.Exceptions
{
    using System;

    public class NetRouterConfigurationException : Exception
    {
        public NetRouterConfigurationException(string message)
            : base(message)
        {
        }

        public NetRouterConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}