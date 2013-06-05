using Nancy;
using Nancy.TinyIoc;

namespace Trackifly.Server.RequestValidators
{
    public interface IRequestValidator
    {
        Response Validate(NancyContext nancyContext, TinyIoCContainer container);
    }
}