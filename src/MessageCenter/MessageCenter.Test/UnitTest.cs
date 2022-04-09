using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCenter.Test
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void Test()
        {
            const string CHANNEL = "����ͨ��";

            string result = string.Empty;
            MessageCenter.Subscribe<UnitTest, string>(this, CHANNEL, (s, e) => result = e);

            string data = "��������";
            MessageCenter.Send(this, CHANNEL, data);

            Assert.IsTrue(result != string.Empty);
            Assert.IsTrue(result == data);
        }

    }
}