using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForkBro.Common.BookmakerClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ForkBro.Tests
{
    [TestClass()]
    public class EventJsonConvertTests
    {
        [TestMethod()]
        public void GetDataAndFormatOddsSnapshotTest()
        {
            Common.Sport sport = Common.Sport.TableTennis;
            long id = 25529837;
            HttpRequest_favbet httpRequest = new HttpRequest_favbet();
            var result = httpRequest.GetDictionaryOdds(id, sport);
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText($"E:\\Workbench\\Projects\\Fork Bro\\ForkBro\\Logs\\Pools\\fav_{ sport }_E{ id}.json", str);//DEBUG
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void GetDataAndFormatOddsSnapshotTest1()
        {
            Common.Sport sport = Common.Sport.Basketball;
            long id = 230813162;
            HttpRequest_1xbet httpRequest = new HttpRequest_1xbet();
            var result = httpRequest.GetDictionaryOdds(id, sport);
            var str = Newtonsoft.Json.JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);

            File.WriteAllText($"E:\\Workbench\\Projects\\Fork Bro\\ForkBro\\Logs\\Pools\\1xbet_{ sport }_E{ id}.json", str);//DEBUG
            Assert.IsTrue(true);
        }
    }
}