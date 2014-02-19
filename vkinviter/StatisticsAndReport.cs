using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vkinviter
{
    public static class StatisticsAndReport
    {
        public static string CityNames { get; set; }
        public static int UserCountInCity { get; set; }

        public static Dictionary<string, int> InvitationResultsAndCount { get; set; }

        public static void ParseResultForReport(List<VkUser> users)
        {
            InvitationResultsAndCount = new Dictionary<string, int>();

            foreach (VkUser user in users)
            {
                if (!InvitationResultsAndCount.ContainsKey(user.InvitationResult))
                {
                    InvitationResultsAndCount.Add(user.InvitationResult, 1);
                }
                else
                {
                    InvitationResultsAndCount[user.InvitationResult] += 1;
                }
            }
        }

        public static string GetReport()
        {
            string output = string.Format("Выбранные города : {0}", CityNames);
            output += Environment.NewLine;
            output += string.Format("Всего в указанных городах - {0}", UserCountInCity);

            foreach (string key in InvitationResultsAndCount.Keys)
            {
                output += Environment.NewLine;
                output += string.Format("{0} - {1}", key.Replace('.',' '), InvitationResultsAndCount[key]);
            }

            return output;
        }

    }
}
