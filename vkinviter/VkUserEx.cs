using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;


namespace vkinviter
{
    public static class VkUserEx
    {
        public static int AlSearchGetSummaryUserCount(string responseString)
        {
            Logger.LogMethod();

            /*
               "summary":"В этой группе найдено 2<span class=\"num_delim\"> <\/span>034 человека<span
            */
            string patternGetUsersCount = "<!json>{.*\"summary\":\"В этой группе найдено (.*) человек\\w?<span";
            MatchCollection mcUserCount = Regex.Matches(responseString, patternGetUsersCount);
            mcUserCount = Regex.Matches(mcUserCount[0].Groups[1].Value, "(\\d+)");
            int userCount = 0;
            foreach (Match match in mcUserCount)
            {
                userCount = userCount * 1000 + Convert.ToInt32(match.Groups[1].Value);
            }
            Logger.AddText("usercount=={0}", userCount);
            return userCount;
        }

        public static List<VkUser> SetCityIdToAllUsers(this List<VkUser> users, string cityId)
        {
            foreach (VkUser user in users)
            {
                user.CityId = cityId;
            }
            return users;
        }

        public static List<VkUser> AlSearchParsePeopleRow(string responseString)
        {
            Logger.LogMethod();
            /*
  <div class="people_row three_col_row clear_fix">
  <div class="img search_bigph_wrap fl_l" onmouseover="Searcher.bigphOver(this, 950328)">
    <a href="/liya_manilova" onclick="return nav.go(this, event);"><img class="search_item_img" src="http://cs314528.vk.me/v314528328/2faa/8Ex2360SBcE.jpg" /></a>
  </div>
  <div class="info fl_l">
    <div class="labeled name"><a href="/liya_manilova" onclick="return nav.go(this, event);">Екатерина Москвина</a></div><div class="labeled ">МГППУ '08</div><div class="online">Online</div>
  </div>
             */
            string patternId = "<div[^>]*onmouseover=\"Searcher.bigphOver\\(this, (\\d+)\\)\">";
            string patternHrefAndName = "<div class=\"labeled name\"><a href=\"/([^\"]+)\" onclick=\"return nav\\.go\\(this, event\\);\">([^<]+)<";
            MatchCollection mcIds = Regex.Matches(responseString, patternId);
            MatchCollection mcHrefAndNames = Regex.Matches(responseString, patternHrefAndName);

            if (mcIds.Count != mcHrefAndNames.Count)
            {
                Logger.AddText("mcIds.Count=={0}, mcHrefAndNames.Count=={1}",
                    mcIds.Count, mcHrefAndNames.Count);

                throw new Exception("mcIds.Count!=mcHrefAndNames.Count");
            }

            List<VkUser> listVkUser = new List<VkUser>();
            VkUser vkUser;
            for (int i = 0; i < mcIds.Count; i++)
            {
                vkUser = new VkUser()
                {
                    Id = mcIds[i].Groups[1].Value,
                    Href = mcHrefAndNames[i].Groups[1].Value,
                    Name = mcHrefAndNames[i].Groups[2].Value,

                };
                Logger.AddText(vkUser.ToString());
                listVkUser.Add(vkUser);
            }
            return listVkUser;
        }

        public static List<VkUser> ParseFriendsList(string responseString)
        {
            List<VkUser> listVkUser = new List<VkUser>();
            VkUser vkUser;
            //['1298','http://cs315422.vk.me/u01298/d_e13351b2.jpg','/id1298','2','0','Сергей Суворов','0','1','61','09','0','fe9642838dd1d04fb1']
            string pattern = @"\['(\d*)','[^']*','\/([\w\.]+)','[^']*','[^']*','([^']+)','[^']*','(\d+)','[^']*','[^']*','[^']*','(\w+)'\]";
            foreach (Match match in Regex.Matches(responseString, pattern, RegexOptions.Singleline))
            {
                vkUser = new VkUser()
                {
                    Id = match.Groups[1].Value,
                    Href = match.Groups[2].Value,
                    Name = match.Groups[3].Value,
                    IsActive = match.Groups[4].Value,
                    HashId = match.Groups[5].Value
                };
                Logger.AddText(vkUser.ToString());
                listVkUser.Add(vkUser);
            }
            string patternNorm = @"'http:\/\/cs\d+\.vk\.me";
            string patternDeactiveOrDeleted = @"'\/images\/\w+\.gif'";

            MatchCollection mcNorm = Regex.Matches(responseString, patternNorm);
            MatchCollection mcDeactiveOrDeleted = Regex.Matches(responseString, patternDeactiveOrDeleted);
            int deactiveUserCount =
                (from user in listVkUser
                 where user.IsActive == "0"
                 select user).Count();
            //if some user with "deactivated" avatar was marked as "active" - not a big deal
            if (deactiveUserCount > mcDeactiveOrDeleted.Count)
            {
                throw new Exception("deactiveUserCount > mcDeactiveOrDeleted.Count");
            }

            int httpCount = mcNorm.Count + mcDeactiveOrDeleted.Count;
            if (listVkUser.Count != httpCount)
            {
                throw new Exception("http total count != user count");
            }

            return listVkUser;
        }

        public static List<VkUser> GetHasIdFrom(this List<VkUser> usersInTheCity, List<VkUser> friendsInTheGroup)
        {
            Logger.LogMethod();

            foreach (VkUser user in usersInTheCity)
            {
                VkUser userFounded = friendsInTheGroup.Find(
                    delegate(VkUser userInThecity)
                    {
                        return userInThecity.Id == user.Id;
                    });
                if (userFounded != null)
                {
                    user.HashId = userFounded.HashId;
                    user.IsActive = userFounded.IsActive;
                }
                else
                {
                    Logger.AddText("User {0} name {1} isn't found in friends list!", user.Id, user.Name);
                }
            }
            return usersInTheCity;
        }

        public static bool AlSearchParseHasMore(string responseString)
        {
            Logger.LogMethod();

            Match match = Regex.Match(responseString, "<!json>{.*\"has_more\":(\\w*),", RegexOptions.None);
            bool hasMore = Convert.ToBoolean(match.Groups[1].Value);
            Logger.AddText("hasMore==" + hasMore.ToString());
            return hasMore;
        }

        public static int AlSearchParseOffset(string responseString)
        {
            Logger.LogMethod();

            Match match = Regex.Match(responseString, "<!json>{.*\"offset\":(\\d*),", RegexOptions.None);
            int offset = Convert.ToInt32(match.Groups[1].Captures[0].Value);
            Logger.AddText("offset==" + offset.ToString());
            return offset;
        }
    }
}
