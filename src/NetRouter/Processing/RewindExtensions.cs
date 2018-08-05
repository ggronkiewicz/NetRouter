namespace NetRouter.Processing
{
    using System;
    using System.IO;
    using Microsoft.AspNetCore.WebUtilities;
    using NetRouter.Abstraction;

    internal static class RewindExtensions
    {
        internal const int DefaultBufferThreshold = 1024 * 30;

        private static string _tempDirectory;

        public static string TempDirectory
        {
            get
            {
                if (_tempDirectory == null)
                {
                    // Look for folders in the following order.
                    var temp = Environment.GetEnvironmentVariable("ASPNETCORE_TEMP") ??     // ASPNETCORE_TEMP - User set temporary location.
                               Path.GetTempPath();                                      // Fall back.

                    if (!Directory.Exists(temp))
                    {
                        throw new DirectoryNotFoundException(temp);
                    }

                    _tempDirectory = temp;
                }

                return _tempDirectory;
            }
        }

        public static void BufferingRequestStream(this IRequestContext requestContext)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }

            var inputStream = requestContext.Request.Body;

            if (inputStream != null && !inputStream.CanSeek)
            {
                var fileStream = new FileBufferingReadStream(inputStream, DefaultBufferThreshold, null, TempDirectory);
                requestContext.RegisterForDispose(fileStream);
                requestContext.Request.Body = fileStream;
            }
        }

        public static void BufferingResponseStream(this IRequestContext requestContext, IResponse response)
        {
            if (requestContext == null)
            {
                throw new ArgumentNullException(nameof(requestContext));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            var inputStream = response.Body;

            if (inputStream != null && !inputStream.CanSeek)
            {
                var fileStream = new FileBufferingReadStream(inputStream, DefaultBufferThreshold, null, TempDirectory);
                requestContext.RegisterForDispose(fileStream);
                response.Body = fileStream;
            }
        }
    }
}
