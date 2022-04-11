using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MessageCenterTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void Test()
        {
            const string CHANNEL = "测试通道";

            string result = string.Empty;
            MessageCenter.Subscribe<UnitTest, string>(this, CHANNEL, (s, e) => result = e);

            string data = "测试数据";
            MessageCenter.Send(this, CHANNEL, data);

            Assert.IsTrue(result != string.Empty);
            Assert.IsTrue(result == data);
        }
    }
}
