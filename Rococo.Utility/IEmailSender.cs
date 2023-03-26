using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Rococo.Utility
{
    public interface IEmailSender
    {
         Task SendEmailAsync(List<Message> messages);
    }
}
