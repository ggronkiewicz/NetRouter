namespace NetRouter.Filters.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class EnumParseException<T> : Exception
        where T : struct
    {
    }
}
