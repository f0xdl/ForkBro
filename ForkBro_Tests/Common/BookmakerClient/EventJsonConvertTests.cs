using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForkBro.Common.BookmakerClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForkBro.Tests
{
    [TestClass()]
    public class EventJsonConvertTests
    {
        [TestMethod()]
        public void GetDataAndFormatOddsSnapshotTest()
        {
            HttpRequest_1xbet httpRequest = new HttpRequest_1xbet();
            httpRequest.GetDictionaryOdds(230623899, Common.Sport.Tennis);
            Assert.Fail();
        }
    }
}