using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using Trackifly.Server.Models;
using Trackifly.Server.Modules;

namespace Trackifly.Server.Tests.Integration
{
    [TestFixture]
    public class UserTests : TestsSetup
    {
        [Test]
        public void User_should_be_stored_in_the_database_and_return_Created()
        {
            var browser = new Browser(with =>
            {
                with.Module<UserModule>();
                with.Dependencies(_dependencies);
            });

            var model = new UserModel();
            model.Username = "test";
            model.Password = "test";
            model.Name = "Test Testesen";
            model.Email = "test@test.net";

            var response = browser.Post("/user", with =>
            {
                with.HttpRequest();
                with.JsonBody(model);
            });

            Assert.That(response.StatusCode == HttpStatusCode.Created);
        }
    }
}