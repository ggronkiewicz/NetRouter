namespace NetRouter.Exceptions
{
    using System;
    using NetRouter.Abstraction.Filters;

    public class NetRouterFilterException : Exception
    {
        public NetRouterFilterException(Exception ex, IFilter filter)
            : base($"Error in filter '{filter?.GetType().Name}' - {ex.Message}", ex)
        {

        }
    }
}
