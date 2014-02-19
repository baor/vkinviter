using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vkinviter;
using System.Net;
using System.IO;
using System.Text;
namespace UnitTestProject
{
    [TestClass]
    public class InviterTest
    {
        [TestMethod]
        public void TestVkCity()
        {
            string response = @"[['1','Москва','','1'],['1005647','Менеуз-Москва','Бижбулякский район,<br /> Башкортостан ','1'],['1011210','355 км и 356 км автотрассы Москва-Киев','Брянский район,<br /> Брянская область','1']]";
            List<VkCity> listVkCity = VkCityEx.ParseReponse(response);
            Assert.IsTrue(listVkCity.Count == 3);
            Assert.AreEqual("1", listVkCity[0].Id);
            Assert.AreEqual("Москва", listVkCity[0].Name);
            Assert.AreEqual("", listVkCity[0].Area);
            Assert.AreEqual("1", listVkCity[0].CountryId);

            Assert.AreEqual("1011210", listVkCity[2].Id);
            Assert.AreEqual("355 км и 356 км автотрассы Москва-Киев", listVkCity[2].Name);
            Assert.AreEqual("Брянский район, Брянская область", listVkCity[2].Area);
            Assert.AreEqual("1", listVkCity[2].CountryId);
        }

        [TestMethod]
        public void TestVkUserAlSearchGetSummaryUserCount()
        {
            string logfilePath = @"..\..\testdata\TestVkUserParseAlSearchResult.txt";
            string response = string.Empty;

            using (StreamReader streamReader = new StreamReader(logfilePath, Encoding.UTF8))
            {
                response = streamReader.ReadToEnd();
            }
            int userCount = VkUserEx.AlSearchGetSummaryUserCount(response);
            Assert.AreEqual(2034, userCount);
        }
        [TestMethod]
        public void TestVkUserAlSearchParseResult()
        {
            string logfilePath = @"..\..\testdata\TestVkUserParseAlSearchResult.txt";
            string response = string.Empty;

            using (StreamReader streamReader = new StreamReader(logfilePath, Encoding.UTF8))
            {
                response = streamReader.ReadToEnd();
            }
            string cityId = "1";
            List<VkUser> listVkUsers = (VkUserEx.AlSearchParsePeopleRow(response)).SetCityIdToAllUsers(cityId);
            
            Assert.IsTrue(listVkUsers.Count == 3);
            Assert.AreEqual("1421583", listVkUsers[0].Id);
            Assert.AreEqual("tooniegirl", listVkUsers[0].Href);
            Assert.AreEqual("Мария Смолина", listVkUsers[0].Name);
            Assert.AreEqual(cityId, listVkUsers[0].CityId);


            Assert.AreEqual("5202819", listVkUsers[1].Id);
            Assert.AreEqual("amirsanova", listVkUsers[1].Href);
            Assert.AreEqual("Анастасия Мирсанова", listVkUsers[1].Name);
            Assert.AreEqual(cityId, listVkUsers[1].CityId);

            Assert.AreEqual("154914280", listVkUsers[2].Id);
            Assert.AreEqual("bizneschans", listVkUsers[2].Href);
            Assert.AreEqual("Максим Кузнецов", listVkUsers[2].Name);
            Assert.AreEqual(cityId, listVkUsers[2].CityId);
        }

        [TestMethod]
        public void TestVkUserParseFriendsList()
        {
            string logfilePath = @"..\..\testdata\TestVkUserParseFriendsList.txt";
            string response = string.Empty;

            using (StreamReader streamReader = new StreamReader(logfilePath, Encoding.UTF8))
            {
                response = streamReader.ReadToEnd();
            }
            List<VkUser> listVkUsers = VkUserEx.ParseFriendsList(response);
            Assert.IsTrue(listVkUsers.Count == 5);
            Assert.AreEqual("1298", listVkUsers[0].Id);
            Assert.AreEqual("Сергей Суворов", listVkUsers[0].Name);
            Assert.AreEqual("id1298", listVkUsers[0].Href);
            Assert.AreEqual("1", listVkUsers[0].IsActive);
            Assert.AreEqual("fe9642838dd1d04fb1", listVkUsers[0].HashId);


            Assert.AreEqual("2022", listVkUsers[1].Id);
            Assert.AreEqual("Дмитрий Данилов", listVkUsers[1].Name);
            Assert.AreEqual("apollo", listVkUsers[1].Href);
            Assert.AreEqual("1", listVkUsers[1].IsActive);
            Assert.AreEqual("ad3daa62453178fdc2", listVkUsers[1].HashId);

            Assert.AreEqual("18599", listVkUsers[2].Id);
            Assert.AreEqual("Сергей Балыков", listVkUsers[2].Name);
            Assert.AreEqual("id18599", listVkUsers[2].Href);
            Assert.AreEqual("0", listVkUsers[2].IsActive);
            Assert.AreEqual("d421f799bae7b27375", listVkUsers[2].HashId);

            Assert.AreEqual("31078", listVkUsers[3].Id);
            Assert.AreEqual("Надежда Удалилась", listVkUsers[3].Name);
            Assert.AreEqual("id31078", listVkUsers[3].Href);
            Assert.AreEqual("0", listVkUsers[3].IsActive);
            Assert.AreEqual("d9802b9035f4fb1a67", listVkUsers[3].HashId);

            Assert.AreEqual("630887", listVkUsers[4].Id);
            Assert.AreEqual("DELETED", listVkUsers[4].Name);
            Assert.AreEqual("id630887", listVkUsers[4].Href);
            Assert.AreEqual("0", listVkUsers[4].IsActive);
            Assert.AreEqual("a2f7881483cfe27a77", listVkUsers[4].HashId);
        }

        [TestMethod]
        public void TestGetHasIdFrom()
        {
            string logfilePath1 = @"..\..\testdata\TestVkUserParseAlSearchResult.txt";
            string response1 = string.Empty;
            string logfilePath2 = @"..\..\testdata\TestVkUserParseFriendsList.txt";
            string response2 = string.Empty;

            using (StreamReader streamReader = new StreamReader(logfilePath1, Encoding.UTF8))
            {
                response1 = streamReader.ReadToEnd();
            }
            using (StreamReader streamReader = new StreamReader(logfilePath2, Encoding.UTF8))
            {
                response2 = streamReader.ReadToEnd();
            }
            string cityId = "1";
            List<VkUser> usersInTheCity = (VkUserEx.AlSearchParsePeopleRow(response1)).SetCityIdToAllUsers(cityId);
            List<VkUser> friends = VkUserEx.ParseFriendsList(response2);

            friends[1].Id = "1421583";
            friends[2].Id = "5202819";
            usersInTheCity = usersInTheCity.GetHasIdFrom(friends);
            Assert.IsTrue(usersInTheCity.Count == 3);
            Assert.AreEqual(friends[1].HashId, usersInTheCity[0].HashId);
            Assert.AreEqual(friends[2].HashId, usersInTheCity[1].HashId);
            Assert.AreEqual(null, usersInTheCity[2].HashId);
        }

        [TestMethod]
        public void TestGetGroupIdByUrl()
        {
            string url = @"http://vk.com/v7paca";
            string groupId = Inviter.GetGroupIdByUrl(url);
            Assert.AreEqual("6206", groupId);
        }

        [TestMethod]
        public void TestGetCityIdByName()
        {
            string cityId = VkCityEx.GetCityIdByName("1", "Москва");
            Assert.AreEqual("1", cityId);
        }

        [TestMethod]
        public void GetListOfCityIdByNames()
        {
            List<string> listCityId = VkCityEx.GetListOfCityIdByNames("1", "Москва, Балашиха, Химки, Подольск");
            Assert.AreEqual(4, listCityId.Count);

            Assert.AreEqual("1", listCityId[0]);
            Assert.AreEqual("24", listCityId[1]);
            Assert.AreEqual("155", listCityId[2]);
            Assert.AreEqual("270", listCityId[3]);
        }
    }
}
