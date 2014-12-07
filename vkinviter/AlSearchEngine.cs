using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vkinviter
{
    public class AlSearchEngine
    {
        private const int SORTIDMAX = 1;
        private const int STATUSIDMAX = 7;
        private const int SEXIDMAX = 2;
        private const int AGEMAX = 80;
        private const int AGEVKMIN = 14;
        private const int AGEMYMIN = 12;

        private bool freezeAge = false;
        public int SortId { get; set; }
        public int StatusId { get; set; }
        public int SexId { get; set; }
        public int AgeVal { get; set; }
        public AlSearchEngine()
        {
            SortId = 0;
            StatusId = 0;
            SexId = 0;
            AgeVal = AGEMYMIN;
        }

        public bool IsFirstStep()
        {
            if (SortId == 0
                && StatusId == 0
                && SexId == 0
                && AgeVal == AGEMYMIN)
                return true;
            return false;
        }
        private bool NextSubStep()
        {
            Logger.LogMethod();

            while (SortId < SORTIDMAX)
            {
                SortId += 1;
                return true;
            }

            while (StatusId < STATUSIDMAX)
            {
                StatusId += 1;
                SortId = 0;
                return true;
            }

            while (SexId < SEXIDMAX)
            {
                SexId += 1;
                SortId = 0;
                StatusId = 0;
                return true;
            }

            SexId = 0;
            SortId = 0;
            StatusId = 0;
            return false;
        }
        public bool NextStep(int offset)
        {
            Logger.LogMethod(offset);

            //if we can choose everyone without additional filtration
            if (IsFirstStep() && offset < 1000)
                return false;

            //use filter without age
            while (NextSubStep() && AgeVal == AGEMYMIN)
                return true;
            
            //commented due to useless

            ////filter by Ages
            //while ((AgeVal < AGEMAX) && (!freezeAge))
            //{
            //    //if not zero age has more than 1000
            //    //go and apply additional filters
            //    if ((AgeVal > AGEMYMIN) && (offset >= 1000))
            //    {
            //        Logger.AddText("freezeAge = true");
            //        freezeAge = true;
            //        break;
            //    }
            //    AgeVal += 1;
            //    return true;
            //}
            
            //if (freezeAge)
            //{
            //    //apply "standart" filter as addition for age
            //    while (NextSubStep())
            //        return true;

            //    Logger.AddText("freezeAge = false");
            //    freezeAge = false;

            //    if ((AgeVal < AGEMAX))
            //    {
            //        AgeVal += 1;
            //        return true;
            //    }
            //}
            return false;           
        }
        public string GetSearchUrlAddition(int offset)
        {
            string url = string.Empty;

            if (SexId > 0)
                url += "&c%5Bsex%5D=" + SexId;

            if (SortId > 0)
                url += "&c%5Bsort%5D=" + SortId;

            if (StatusId > 0)
                url += "&c%5Bstatus%5D=" + StatusId;

            if (offset > 0)
                url += "&offset=" + offset;

            if (AgeVal == AGEVKMIN - 1)
                url += string.Format("&c%5Bage_to%5D={0}", AGEVKMIN);
            else if ((AgeVal >= AGEVKMIN) && (AgeVal < AGEMAX))
                url += string.Format("&c%5Bage_from%5D={0}&c%5Bage_to%5D={0}", AgeVal);
            else if (AgeVal == AGEMAX)
                url += string.Format("&c%5Bage_from%5D={0}", AgeVal);

            return url;
        }
    }
}
