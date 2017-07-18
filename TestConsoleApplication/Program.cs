using System;
using System.Configuration;
using System.Net.Mail;
using ForCheng.SendEmail;

namespace TestConsoleApplication
{
    class Program
    {

        static void Main()
        {
          

            var toEmail = "XXX@qq.com";//目标邮箱
            var subjectInfo = "测试";//邮件标题
            var bodyInfo = "这是一封测试邮件"; //邮件正文


            //若想保证邮件发送成功，则还需要完善App.config配置文件中的邮件发送者的信息
            var mailconfig = (EmailConfigurationProvider)ConfigurationManager.GetSection("EmailConfigurationProvider");
            var email = new Email(mailconfig.Account, mailconfig.Password, mailconfig.Server, mailconfig.Port,
                mailconfig.IsSSL)
            {
                From = new MailAddress(mailconfig.Account, mailconfig.Name),
                Subject = subjectInfo,
                IsBodyHtml = true,
                Body = bodyInfo,
            };

            //发送邮件
            if (email.Send(toEmail))
            {
                Console.WriteLine("邮件发送成功!");
            }
            else
            {
                Console.WriteLine("邮件发送失败!");
            }
        }
    }
}
