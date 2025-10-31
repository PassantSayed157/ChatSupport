using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Models
{
    public class MessageDto
    {
        public string Text { get; set; } = string.Empty;
        public bool IsUser { get; set; } = true;
        public DateTime SentAt { get; set; } = DateTime.Now;
    }
}
