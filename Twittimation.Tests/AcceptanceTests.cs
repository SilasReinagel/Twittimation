using Carvana;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Twittimation.IO;
using Twittimation.Twitter;

namespace Twittimation.Tests
{
    [TestClass]
    public class AcceptanceTests
    {
        [TestMethod]
        public void App_Setup_CredentialsSaved()
        {
            var storage = new InMemoryStorage();
            var app = CreateApp(storage);

            var result = app.Execute("setup username consumerKey consumerKeySecret accessToken accessTokenSecret");

            Assert.IsTrue(result.Succeeded());
            Assert.AreEqual("consumerKey", storage.Get<Credentials>(nameof(Credentials)).ConsumerKey);
        }
        
        [TestMethod] public void App_Run_Succeeded() => AssertSucceeded("run once");
        [TestMethod] public void App_Credentials_Succeeded() => AssertSucceeded("viewcredentials");
        [TestMethod] public void App_Help_Succeeded() => AssertSucceeded("help");
        [TestMethod] public void App_Tasks_Succeeded() => AssertSucceeded("viewtasks");
        [TestMethod] public void App_Tweet_Succeeded() => AssertSucceeded("tweet \"Hello, World\"");
        [TestMethod] public void App_Like_Succeeded() => AssertSucceeded("like 1234567");
        [TestMethod] public void App_SetMaxLikes_Succeeded() => AssertSucceeded("setmaxlikes jackdorsey 1");

        private void AssertSucceeded(string cmd) => AssertSucceeded(AppWithCredentials(), cmd);
        private void AssertSucceeded(Cli app, string cmd) => Assert.IsTrue(app.Execute(cmd).Succeeded());

        private Cli AppWithCredentials()
        {
            var app = CreateApp(new InMemoryStorage());
            
            app.Execute("setup username consumerKey consumerKeySecret accessToken accessTokenSecret")
                .OnFailure(x => Assert.Inconclusive("Unable to setup app."));

            return app;
        } 
        private Cli CreateApp(IStorage storage)
        {
            return Program.Init(new ConsoleLog(), storage, new TwitterStub());
        }
    }
}
