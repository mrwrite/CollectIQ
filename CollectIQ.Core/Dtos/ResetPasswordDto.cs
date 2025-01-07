using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectIQ.Core.Dtos
{
    public class ResetPasswordDto
    {

        public string userId { get; set; }
        public string token { get; set; }
        public string newPassword { get; set; }


    }
}
