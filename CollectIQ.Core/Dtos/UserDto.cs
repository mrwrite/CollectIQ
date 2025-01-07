using CollectIQ.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Core.Dtos
{
    public class UserDto : User
    {
        public string? Token { get; set; }
    }
}
