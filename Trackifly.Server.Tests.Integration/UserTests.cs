using NUnit.Framework;
using Nancy;
using Nancy.Testing;
using Trackifly.Data.Models;
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

            var model = new UserModel
                {
                    Username = "test123",
                    Password = "test123",
                    Name = "Test Testesen",
                    Email = "test@test.net"
                };

            var response = browser.Post("/user", with =>
            {
                with.HttpRequest();
                with.JsonBody(model);
            });

            Assert.That(response.StatusCode == HttpStatusCode.Created);

            var user = response.Body.DeserializeJson<TrackingUser>();
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Username, model.Username);
            Assert.AreEqual(user.Email, model.Email);
            Assert.AreEqual(user.Name, model.Name);
            Assert.NotNull(user.Password);
            Assert.NotNull(user.Salt);

            response = browser.Delete(string.Format("/user/{0}/{1}", user.AccessToken.Token, user.Id), with => with.HttpRequest());

            Assert.That(response.StatusCode == HttpStatusCode.OK);
        }
    }
}