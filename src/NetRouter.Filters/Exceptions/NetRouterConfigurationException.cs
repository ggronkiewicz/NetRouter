namespace NetRouter.Filters.Exceptions
{
    using System;

    public class FiltersConfigurationException : Exception
    {
        public FiltersConfigurationException(string message)
            : base(message)
        {
        }

        public FiltersConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}