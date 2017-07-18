using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace ForCheng.SendEmail
{
    /// <summary>
    /// 发送邮件的类
    /// </summary>
    public class Email
    {
        #region Private fields
        private readonly MailMessage _mailMessage;   //主要处理发送邮件的内容（如：收发人地址、标题、主体、图片等等）
        private readonly SmtpClient _smtpClient; //主要处理用smtp方式发送此邮件的配置信息（如：邮件服务器、发送端口号、验证方式等等）

        #endregion

        #region Constructor
        ///<summary>
        /// 构造函数
        ///</summary>
        ///<param name="username">发件邮箱</param>
        ///<param name="password">发件邮箱的密码</param>
        ///<param name="server">发件箱的邮件服务器地址</param>
        ///<param name="port">发送邮件所用的端口号（stmp协议默认为25）</param>
        ///<param name="sslEnable">true表示对邮件内容进行socket层加密传输，false表示不加密</param>
        public Email(string username, string password, string server, int port, bool sslEnable)
        {
            _smtpClient = new SmtpClient(server, port)
            {
                EnableSsl = sslEnable,
                Credentials = new NetworkCredential(username, password)
            };
            _mailMessage = new MailMessage();
        }

        #endregion

        #region Public methods

        public MailAddress From
        {
            get { return _mailMessage.From; }
            set { _mailMessage.From = value; }
        }

        public string Subject
        {
            get { return _mailMessage.Subject; }
            set { _mailMessage.Subject = value; }
        }

        public string Body
        {
            get { return _mailMessage.Body; }
            set { _mailMessage.Body = value; }
        }

        public bool IsBodyHtml
        {
            get { return _mailMessage.IsBodyHtml; }
            set { _mailMessage.IsBodyHtml = value; }
        }

        #endregion

        #region Public methods

        ///<summary>
        /// 添加附件
        ///</summary>
        ///<param name="paths">附件的路径集合</param>
        public void AddAttachments(IEnumerable<string> paths)
        {
            try
            {
                foreach (var item in paths)
                {
                    var data = new Attachment(item, MediaTypeNames.Application.Octet);
                    var disposition = data.ContentDisposition;
                    disposition.CreationDate = File.GetCreationTime(item);
                    disposition.ModificationDate = File.GetLastWriteTime(item);
                    disposition.ReadDate = File.GetLastAccessTime(item);
                    _mailMessage.Attachments.Add(data);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 清除附件
        /// </summary>
        public void ClearAttachments()
        {
            _mailMessage.Attachments.Clear();
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toEmail">收件人的地址</param>
        /// <returns>发送成功返回true,否则返回false</returns>
        public bool Send(string toEmail)
        {
            var to = new MailAddress(toEmail, toEmail);
            return Send(to);
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人的地址</param>
        /// <returns>发送成功返回true,否则返回false</returns>
        public bool Send(MailAddress to)
        {
            return Send(new [] { to });
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="toAddresses">收件人的地址集合</param>
        /// <returns>发送成功返回true,否则返回false</returns>
        public bool Send(IEnumerable<MailAddress> toAddresses)
        {
            try
            {
                _mailMessage.To.Clear();
                foreach (var item in toAddresses)
                {
                    _mailMessage.To.Add(item);
                }
                _smtpClient.Send(_mailMessage);
            }
            catch
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}