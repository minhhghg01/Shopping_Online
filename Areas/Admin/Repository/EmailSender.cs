using System.Net;
using System.Net.Mail;

namespace Shopping_Online.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
 {
     public Task SendEmailAsync(string email, string subject, string message)
     {
         var client = new SmtpClient("smtp.gmail.com", 587) 
         {
             EnableSsl = true, //bật bảo mật
             UseDefaultCredentials = false,
             Credentials = new NetworkCredential("nguyenminhhghg01@gmail.com", "rhnbfvyhkpclnayz")
         };

         return client.SendMailAsync(
             new MailMessage(from: "nguyenminhhghg01@gmail.com",
                             to: email,
                             subject,
                             message
                             ));
     }
 }
}