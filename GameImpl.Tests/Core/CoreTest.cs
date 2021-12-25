using GameImpl.Support;
using NUnit.Framework;

namespace GameImpl.Tests.Core {
    [TestFixture]
    public class CoreTests {
        [Test]
        public void SaveDirConsistent() {
            // Write some test code.
            Assert.IsTrue(Savefile.GetSaveDirectory().EndsWith(Savefile.gamedir));
        }
    }
}
