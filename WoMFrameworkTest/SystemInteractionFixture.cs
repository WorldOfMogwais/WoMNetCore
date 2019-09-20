namespace WoMFrameworkTest
{
    using System.IO;
    using WoMFramework.Tool;
    using Xunit;

    public class SystemInteractionFixture
    {
        public SystemInteractionFixture()
        {
            SystemInteraction.ReadData = f => File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(SystemInteractionFixture).Assembly.Location), f));
            SystemInteraction.DataExists = f => File.Exists(Path.Combine(Path.GetDirectoryName(typeof(SystemInteractionFixture).Assembly.Location), f));
            SystemInteraction.ReadPersistent = f => File.ReadAllText(Path.Combine(Path.GetDirectoryName(typeof(SystemInteractionFixture).Assembly.Location), f));
            SystemInteraction.PersistentExists = f => File.Exists(Path.Combine(Path.GetDirectoryName(typeof(SystemInteractionFixture).Assembly.Location), f));
            SystemInteraction.Persist = (f, c) => File.WriteAllText(Path.Combine(Path.GetDirectoryName(typeof(SystemInteractionFixture).Assembly.Location), f), c);
        }
    }

    [CollectionDefinition("SystemInteractionFixture")]
    public class SystemInteractionCollection : ICollectionFixture<SystemInteractionFixture>
    {
    }
}
