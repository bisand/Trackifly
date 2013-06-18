using MongoDB.Driver;
using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using Trackifly.Data.Storage;
using Trackifly.Server.Configuration;
using Trackifly.Server.Helpers;
using Trackifly.Server.Models;
using Trackifly.Server.Modules;
using Trackifly.Server.RequestValidators;

namespace Trackifly.Server.Tests.Integration
{
    [TestFixture]
    public class LoginTests : TestsSetup
    {
        [Test]
        public void Login_should_respond_with_Ok()
        {
            var browser = new Browser(with =>
                {
                    with.Module<LoginModule>();
                    with.Dependencies(_dependencies);
                });

            var model = new LoginModel();
            model.Username = "test";
            model.Password = "test";

            var response = browser.Post("/login", with =>
                {
                    with.HttpRequest();
                    with.JsonBody(model);
                });

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }
    }
}