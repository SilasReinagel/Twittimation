using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twittimation.IO;
using Twittimation.Twitter;

namespace Twittimation.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void App_SaveCredentials_CredentialsSaved()
        {
            var storage = new InMemoryStorage();
            var app = CreateApp(storage);

            var result = app.Execute("savecredentials username consumerKey consumerKeySecret accessToken accessTokenSecret");

            Assert.IsTrue(result.Succeeded());
            Assert.AreEqual("consumerKey", storage.Get<Credentials>(nameof(Credentials)).ConsumerKey);
        }

        private Cli CreateApp(IStorage storage)
        {
            return Program.Init(new ConsoleLog(), storage, new TwitterStub());
        }
    }
}
