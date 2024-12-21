using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using news.application.Exceptions;
using news.domain.Exceptions;
using news.infrastructure.Exceptions;

namespace news.api.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorsController : ControllerBase
    {
        private readonly ILogger<ErrorsController> _logger;

        public ErrorsController(ILogger<ErrorsController> logger)
        {
            _logger = logger;
        }

        [Route("/error-development")]
        public ActionResult<ProblemDetails> ErrorDevelopment()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            return exception switch
            {
                NewsInfrastructureException => HandleNewsInfrastructureException((NewsInfrastructureException)exception),
                NewsApplicationException => HandelNewsApplicationException((NewsApplicationException)exception),
                NewsDomainException => HandelNewsDomainException((NewsDomainException)exception),
                _ => Problem(),
            };

        }


        [Route("/error")]
        public ActionResult<ProblemDetails> Error()
        {
            Exception? exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            return exception switch
            {
                NewsInfrastructureException => HandleNewsInfrastructureException((NewsInfrastructureException)exception),
                NewsApplicationException => HandelNewsApplicationException((NewsApplicationException)exception),
                NewsDomainException => HandelNewsDomainException((NewsDomainException)exception),
                _ => Problem(),
            };

        }
        private ActionResult<ProblemDetails> HandleNewsInfrastructureException(NewsInfrastructureException exception)
        {
            return exception switch
            {
                NewsInfrastructurePersistenceConstraintException => HandleNewsInfrastructurePersistenceConstraintException((NewsInfrastructurePersistenceConstraintException)exception),
                _ => Problem()
            };

            ActionResult<ProblemDetails> HandleNewsInfrastructurePersistenceConstraintException(NewsInfrastructurePersistenceConstraintException ex)
            {
                //ActionResult<ProblemDetails> p = ProblemDetailsFactory.CreateProblemDetails(HttpContext,title: "resource command constraint", detail: ex.Message, statusCode: 422);
                var p = ProblemDetailsFactory.CreateProblemDetails(HttpContext, title: "resource command constraint", detail: ex.Message, statusCode: 422);

                p.Extensions.Add("errors", ex.Data);

                return p;
            }

        }
        private ActionResult<ProblemDetails> HandelNewsApplicationException(NewsApplicationException exception)
        {


            return exception switch
            {
                NewsApplicationResourceNotFoundException => Problem(title: "requested resource was not found", detail: exception.Message, statusCode: 404),
                NewsApplicationAccessToFutureArticleDeniedException => Problem(title: "access denied",detail:exception.Message, statusCode:403),// it still tells them that this resource exists
                _ => Problem(),
            };
        }

        private ActionResult<ProblemDetails> HandelNewsDomainException(NewsDomainException ex)
        {
            return ex switch
            {

                NewsDomainValidationException => HandleNewsDomainValidationException((NewsDomainValidationException)ex),
                _ => Problem(/*statusCode:500*/),
            };


            ActionResult<ProblemDetails> HandleNewsDomainValidationException(NewsDomainValidationException ex)
            {

                var p = ProblemDetailsFactory.CreateProblemDetails(HttpContext, title: "validation problem", detail: ex.Message, statusCode: 422);

                p.Extensions.Add("errors", ex.Data);

                return p;
            }

        }


    }
}
