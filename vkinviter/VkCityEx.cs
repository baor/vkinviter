using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;

namespace vkinviter
{
    public static class VkCityEx
    {
        public static List<VkCity> ParseReponse(string response)
        {
            Logger.LogMethod();

            List<VkCity> listVkCity = new List<VkCity>();
            Regex newRegex = new Regex(@"\[{1,2}([^\[\]]*)\]{1,2},?");
            MatchCollection matchCollection = newRegex.Matches(response);
            foreach (Match match in matchCollection)
            {
                string result = match.Groups[1].Value.Replace(@"<br />", "");
                string[] splittedResult = Regex.Split(result, "\',\'");
                string strId = splittedResult[0].Replace("\'", "");
                string name = splittedResult[1].Replace("\'", "");
                string area = splittedResult[2].Replace("\'", "");
                string countryId = splittedResult[3].Replace("\'", "");

                listVkCity.Add(new VkCity()
                {
                    Id = strId,
                    Name = name,
                    Area = area,
                    CountryId = countryId
                });
            }

            return listVkCity;
        }

        public static string GetCityIdByName(string countryId, string cityName)
        {
            Logger.LogMethod(countryId, cityName);

            string url = string.Format("{0}select_ajax.php?act=a_get_cities&country={1}&str={2}",
                HttpConnector.HTTPVKCOM, countryId, cityName);

            string responseString = HttpConnector.SendHttpWebRequestAndGetResponse(url, HttpMethod.GET)
                .ResponseText;
            List<VkCity> listVkCity = VkCityEx.ParseReponse(responseString);
            return listVkCity[0].Id.ToString();

        }

        public static List<string> GetListOfCityIdByNames(string countryId, string cityNames, BackgroundWorker bw = null)
        {
            Logger.LogMethod(countryId, cityNames, bw);

            List<string> listCityIds = new List<string>();
            string currentCityName;
            MatchCollection mcCities = Regex.Matches(cityNames, "([\\w-]+)", RegexOptions.None);
            for(int i=0; i< mcCities.Count; i++)
            {
                Thread.Sleep(HttpConnector.TIMEOUT);
                currentCityName = GetCityIdByName(countryId, mcCities[i].Groups[0].Value);
                listCityIds.Add(currentCityName);

                if (bw != null)
                    bw.ReportProgress(i * 100 / mcCities.Count);
            }

            return listCityIds;

        }
    }
}
