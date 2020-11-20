using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common.Mail
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
