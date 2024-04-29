using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechBlog.UnitTests.Fixtures
{
    public class HttpContextFixtures
    {
        public HttpContext GetResponseHttpContext(IResult response)
        {
            var context = new DefaultHttpContext
            {
                RequestServices = new ServiceCollection().AddLogging().BuildServiceProvider(),
                Response =
                {
                    Body = new MemoryStream(),
                },
            };

            response.ExecuteAsync(context).Wait();

            context.Response.Body.Position = 0;

            return context;
        }
    }
}
