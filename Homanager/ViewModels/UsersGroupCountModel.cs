using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Homanager.Models;

namespace Homanager.ViewModels
{
    public class UsersGroupCountModel
    {
        public string UserEmail { get; set; }
        public int GroupsCount { get; set; }

        public int GroupOpenMonth { get; set; }
    }
}
