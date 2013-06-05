using Nancy;

namespace Trackifly.Server.RequestValidators
{
    public interface IRequestValidator
    {
        Response Validate(NancyContext nancyContext);
    }
}