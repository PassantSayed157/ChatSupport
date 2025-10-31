using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class ChatRequestDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FirstMessage { get; set; } = string.Empty;
    }
}
