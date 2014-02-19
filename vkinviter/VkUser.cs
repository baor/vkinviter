using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections;


namespace vkinviter
{
    public class VkUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Href { get; set; }
        public string HashId { get; set; }

        public string CityId { get; set; }
        public string IsActive { get; set; }

        public string InvitationResult { get; set; }
        
        public override string ToString()
        {
            return string.Format("Id={0} Name={1} Href={2} HashId={3} IsActive={4}",
                Id, Name, Href, HashId, IsActive);
        }
    }

    // Custom comparer for the VkUser class
    class VkUserComparer : IEqualityComparer<VkUser>
    {
        public bool Equals(VkUser x, VkUser y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the users ids are equal.
            return x.Id == y.Id;
        }

        // If Equals() returns true for a pair of objects 
        // then GetHashCode() must return the same value for these objects.

        public int GetHashCode(VkUser usr)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(usr, null)) return 0;

            return usr.Id.GetHashCode();
        }
    }
}
