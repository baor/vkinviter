using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Linq;

namespace vkinviter
{
    public class Inviter
    {
        public string Login;
        public string Password;

        public static string LOGINVKCOM = "login." + HttpConnector.VKCOM;
        

        private List<string> InviteResults;
        private Dictionary<string, string> DataForReport;

        public Inviter()
        {
            Logger.LogMethod();
            
            InviteResults = new List<string>();
            DataForReport = new Dictionary<string, string>();
        }
        
        public void Logon()
        {
            Logger.LogMethod();
            HttpConnector.HttpCookie = new CookieContainer();
            Action<string,string> AddToCookie = (cookieName, cookieValue) =>
                HttpConnector.HttpCookie.Add(new Uri(HttpConnector.HTTPVKCOM), new Cookie(cookieName, cookieValue));
            AddToCookie("remixchk", "5");
            AddToCookie("remixflash", "10.3.183");
            AddToCookie("remixdt", "0");
            

            string requestBody = "act=login";
            requestBody += "&q=1";
            requestBody += "&al_frame=1";
            requestBody += "&expire=1";
            requestBody += "&captcha_sid=";
            requestBody += "&captcha_key=";
            requestBody += "&from_host=vk.com";
            requestBody += "&from_protocol=http";
            requestBody += "&email=" + Login;
            requestBody += "&pass=" + Password;

            //////////////////////////////////////////////
            string location = HttpConnector.SendHttpWebRequestAndGetResponse(HttpConnector.HTTPVKCOM, HttpMethod.POST, LOGINVKCOM, LOGINVKCOM, requestBody, withoutLogging: true)
                .HttpWebResponse.Headers.Get("location");

            string setCookie = HttpConnector.SendHttpWebRequestAndGetResponse(location, HttpMethod.GET)
                .HttpWebResponse.Headers.Get("Set-Cookie");

            Match match = Regex.Match(setCookie, "remixsid=([\\d|a-f]*);", RegexOptions.None);
            string remixsid = match.Groups[1].Value;
            Logger.AddText("remixsid=={0}", remixsid);
            AddToCookie("remixsid", remixsid);
            AddToCookie("remixreg_sid", "");
            AddToCookie("remixrec_sid", "");
            AddToCookie("remixfeed", "*.*.*.*.*.ph%2Cvd%2Cnt%2Ctp%2Cgr%2Cev%2Cpr.*");
        }
         

        public List<VkUser> GetAllUsersInCity(string groupId, string countryId, string cityId, BackgroundWorker bw = null)
        {
            Logger.LogMethod(groupId, countryId, cityId, bw);

            AlSearchEngine alSearchEngine = new AlSearchEngine();

            int totalUserCount = 0;

            List<VkUser> vkUsersInTheCity = new List<VkUser>();
            List<VkUser> vkUsersSubset;

            string baseUrl = string.Format("{0}al_search.php",
                                    HttpConnector.HTTPVKCOM);

            int offset;
            bool hasMore;
            string postBody;
            do
            {
                offset = 0;
                hasMore = false;

                do
                {
                    postBody = string.Format("al=1&c%5Bcity%5D={0}&c%5Bcountry%5D={1}&c%5Bgroup%5D={2}&c%5Bname%5D=1&c%5Bphoto%5D=1&c%5Bsection%5D=people", cityId, countryId, groupId) 
                        + alSearchEngine.GetSearchUrlAddition(offset);

                    Thread.Sleep(HttpConnector.TIMEOUT);
                    HttpWebResponseEx resp = HttpConnector.SendHttpWebRequestAndGetResponse(baseUrl, HttpMethod.POST,
                        requestBody: postBody);

                    string responseString = resp.ResponseText;

                    if (alSearchEngine.IsFirstStep() && (offset == 0))
                    {
                        totalUserCount = VkUserEx.AlSearchGetSummaryUserCount(responseString);
                    }

                    if (totalUserCount == 0)
                        break;

                    vkUsersSubset = VkUserEx.AlSearchParsePeopleRow(responseString);
                    vkUsersInTheCity = vkUsersInTheCity.Union(vkUsersSubset, new VkUserComparer()).ToList();

                    hasMore = VkUserEx.AlSearchParseHasMore(responseString);
                    offset = VkUserEx.AlSearchParseOffset(responseString);

                    if (bw != null)
                        bw.ReportProgress(vkUsersInTheCity.Count * 100 / totalUserCount);

                } while (hasMore && offset > 0 && offset < 1000);
            } while (alSearchEngine.NextStep(offset));

            vkUsersInTheCity.SetCityIdToAllUsers(cityId);

            List<VkUser> allUsers = new List<VkUser>();
            allUsers.AddRange(vkUsersInTheCity);

            return allUsers;
        }

        public List<VkUser> GetFriendsListForInventation(string eventId, BackgroundWorker bw = null)
        {
            Logger.LogMethod();
            List<VkUser> vkUsersInTheGroupTotal =  new List<VkUser>(); 
            List<VkUser> vkUsersInTheGroupSubSet;
            int offset = 0;
            decimal workProgerss = 0m;
            do
            {
                string url = string.Format("{0}friends",
                    HttpConnector.HTTPVKCOM);

                Thread.Sleep(HttpConnector.TIMEOUT);
                HttpWebResponseEx resp = HttpConnector.SendHttpWebRequestAndGetResponse(url, HttpMethod.POST,
                    requestBody: string.Format("act=get_section_friends&al=1&gid={0}&offset={1}&section=members&sugg_rev=0", eventId, offset));
                string responseString = resp.ResponseText;
                vkUsersInTheGroupSubSet = VkUserEx.ParseFriendsList(responseString);
                offset += vkUsersInTheGroupSubSet.Count;
                vkUsersInTheGroupTotal.AddRange(vkUsersInTheGroupSubSet);
                workProgerss += 0.25m;
                if (bw != null)
                    bw.ReportProgress((int)workProgerss);
            } while (vkUsersInTheGroupSubSet.Count != 0);

            return vkUsersInTheGroupTotal;
        }

        public void InviteCycle(string eventId, List<VkUser> usersInThecity, BackgroundWorker bw = null)
        {
            Logger.LogMethod();

            for (int i = 0; i < usersInThecity.Count; i++ )
            {

                if ((usersInThecity[i].HashId == null)
                    || (usersInThecity[i].HashId == string.Empty)
                    || (usersInThecity[i].IsActive == "0"))
                {
                    if ((usersInThecity[i].HashId == null)
                    || (usersInThecity[i].HashId == string.Empty))
                    {
                        usersInThecity[i].InvitationResult = "Не найдено в списке приглашений";
                    }
                    else if (usersInThecity[i].IsActive == "0")
                    {
                        usersInThecity[i].InvitationResult = "Отключено/неактивно";
                    }
                    if (bw != null)
                        bw.ReportProgress(i * 100 / usersInThecity.Count);
                    continue;
                }

                string requestBody = string.Format("act=a_invite&al=1&gid={0}&hash={1}&mid={2}",
                    eventId, usersInThecity[i].HashId, usersInThecity[i].Id);

                HttpWebResponseEx resp;
                KeyValuePair<string, string> answer = new KeyValuePair<string,string>(string.Empty, string.Empty);
                do
                {
                    if (bw != null)
                        bw.ReportProgress(i * 100 / usersInThecity.Count);

                    if (answer.Key == InivteAnswerCodes.CaptchaEnter)
                    {
                        requestBody += answer.Value;
                        Thread.Sleep(new TimeSpan(0, 0, 1));
                    }

                    resp = HttpConnector.SendHttpWebRequestAndGetResponse(
                        HttpConnector.HTTPVKCOM + "al_page.php",
                        HttpMethod.POST,
                        requestBody: requestBody);
                /*    if (resp.ResponseText == "The remote server returned an error: (501) Not Implemented.")
                    {
                        Logon();
                        resp = HttpConnector.SendHttpWebRequestAndGetResponse(
                        HttpConnector.HTTPVKCOM + "al_page.php",
                        HttpMethod.POST,
                        requestBody: requestBody);
                    }*/
                    answer = ParseInviteAnswer(resp.ResponseText);

                } while (answer.Key == InivteAnswerCodes.CaptchaEnter);

                usersInThecity[i].InvitationResult = answer.Value;
            }
        }

        public KeyValuePair<string, string> ParseInviteAnswer(string responseString)
        {
            Logger.LogMethod();

            string valueStr = string.Empty;

            //<!--14692<!><!>0<!>6524<!>0<!><!int>0<!>Пользователь запретил приглашать себя на встречи.
            //<!--14692<!><!>0<!>6524<!>2<!>799332814008<!>0
            //<!--14692<!><!>0<!>6524<!>8<!>Ошибка доступа<!><!>3386884
            string patternAnsw = @"<!--\d*<!><!>\d*<!>\d*<!>(\d*)<!>(.*)";
            string patternCaptcha = @"(\d+)<!>\d*";
            string patternResult = @"<!int>\d+<!>(.*)";
            string patternAccessError = @"([^<>]*)<!><!>\d*";
            if (responseString == "The remote server returned an error: (501) Not Implemented.")
            {
                return new KeyValuePair<string, string>("501", "The remote server returned an error: (501) Not Implemented.");
            }
            MatchCollection mcResult = Regex.Matches(responseString, patternAnsw);
            MatchCollection mcSubResult;
            switch(mcResult[0].Groups[1].Value)
            {
                case InivteAnswerCodes.InivteWasSent:
                    mcSubResult = Regex.Matches(mcResult[0].Groups[2].Value, patternResult);
                    valueStr = mcSubResult[0].Groups[1].Value;
                    break;
                case InivteAnswerCodes.AccessError:
                    mcSubResult = Regex.Matches(mcResult[0].Groups[2].Value, patternAccessError);
                    Logon();
                    break;
                case InivteAnswerCodes.CaptchaEnter:
                    mcSubResult = Regex.Matches(mcResult[0].Groups[2].Value, patternCaptcha);
                    string captchaSid = mcSubResult[0].Groups[1].Value;
            
                    if (captchaSid != string.Empty)
                    {
                        string url = string.Format("{0}captcha.php?sid={1}&s=1", HttpConnector.HTTPVKCOM, captchaSid);
                        HttpWebResponseEx resp = HttpConnector.SendHttpWebRequestAndGetResponse(url, HttpMethod.GET, withoutLogging: true);
                        System.IO.Stream responseStream = resp.HttpWebResponse.GetResponseStream();
                        Bitmap bitmap = new Bitmap(responseStream);
                        CaptchaForm captchaForm = new CaptchaForm(bitmap);
                        CaptchaContainer.Key = string.Empty;
                        while (CaptchaContainer.Key == string.Empty)
                        {
                            captchaForm.ShowDialog();
                        }

                        valueStr = string.Format("&captcha_key={0}&captcha_sid={1}",
                            CaptchaContainer.Key, captchaSid);
                    }
                    break;
                default:
                    throw new ArgumentException();
            }

            return new KeyValuePair<string, string>(mcResult[0].Groups[1].Value, valueStr);
        }

        public static string GetGroupIdByUrl(string url)
        {
            Logger.LogMethod();

            HttpWebResponseEx resp = HttpConnector.SendHttpWebRequestAndGetResponse(url, HttpMethod.GET);
            string responseString = resp.ResponseText;
            //<a href="/audios-65378758" onclick="return nav.go(this, event);" class="module_header">
            string pattern = @"<a href=""/audios-(\d*)"" onclick=""return nav.go\(this, event\);"" class=""module_header"">";
            string groupId = Regex.Matches(responseString, pattern, RegexOptions.Singleline)[0].Groups[1].Value;

            return groupId;
        }
    }
}
