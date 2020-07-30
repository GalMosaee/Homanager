using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Homanager.Models;

namespace Homanager.ViewModels
{
    public class UserGroupModel
    {
        public string GroupId { get; set; }
        public Group Group { get; set; }

        [DisplayName("Search Username")]
        public string SearchUserKey { get; set; }

        public IEnumerable<AppUser> SearchResultAppUsers { get; set; }
    }
}
