using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestApiDemo.Authorization
{
    public class IntranetHandler : AuthorizationHandler<IntranetRequirement>
    {
        private readonly HttpContext httpContext;

        public IntranetHandler(IHttpContextAccessor httpContext)
        {
            this.httpContext = httpContext?.HttpContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IntranetRequirement requirement)
        {

            var httpContext1 = context.Resource as HttpContext;
            //= filterContext.HttpContext;
            var intranetStr = httpContext1.Request.Headers["Intranet"].FirstOrDefault();
            if (intranetStr == null || intranetStr == "")
            {
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}
