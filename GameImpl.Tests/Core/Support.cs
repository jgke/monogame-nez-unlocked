using GameImpl.Support;
using NUnit.Framework;
using System;

namespace GameImpl.Tests.Core {
    class ThrowingClass {
#pragma warning disable CS0169 // CS0169:The field 'ThrowingClass._field' is never used
        int _field;
#pragma warning restore CS0169 // CS0169:The field 'ThrowingClass._field' is never used
        public int Property {
            get {
                throw new InvalidOperationException("getter called");
            }
            set {
                throw new InvalidOperationException("setter called");
            }
        }
    }

    [TestFixture]
    public class LogTests {
        [Test]
        public void LoggerDoesntAccessProperties() {
            Assert.AreEqual("{\n  \"_field\": 0\n}", Logger.LogString(new ThrowingClass()).Replace("\r", ""));
        }
    }
}
