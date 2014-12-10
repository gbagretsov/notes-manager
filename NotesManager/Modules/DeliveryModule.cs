using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Web;
using NotesManager.Models;

namespace NotesManager.Modules
{
    public class DeliveryModule : IHttpModule
    {
        static Timer timer;
        long interval = 30000; // 30 секунд
        static object synclock = new object();

        public void Init(HttpApplication app)
        {
            timer = new Timer(new TimerCallback(SendEmail), null, 0, interval);
        }

        private void SendEmail(object obj)
        {
            lock (synclock)
            {
                DateTime dd = DateTime.Now;
                UsersNotesContext nc = new UsersNotesContext();
                foreach (Note n in nc.Notes)
                {
                    if (n.EventDate.Date < dd.Date)
                        n.NeedToNotify = false;

                    else if (n.EventDate.Date == dd.Date && n.NeedToNotify)
                    {
                        UsersNotesContext uc = new UsersNotesContext();
                        User user = uc.Users.Find(n.CreatorId);
                        if (user.Notify)
                        {
                            // Настройки smtp-сервера, с которого мы и будем отправлять письмо
                            SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587);
                            smtp.EnableSsl = true;
                            smtp.Credentials = new System.Net.NetworkCredential("zametochnik@gmail.com", "zametochnik1");

                            // Наш email с заголовком письма
                            MailAddress from = new MailAddress("zametochnik@gmail.com", "Заметочник");
                            // Кому отправляем
                            MailAddress to = new MailAddress(user.Email);
                            // Создаем объект сообщения
                            MailMessage m = new MailMessage(from, to);
                            // Тема письма
                            m.Subject = "Важное событие!";
                            // Текст письма
                            m.Body = user.Nickname + ", не пропустите важное событие!\n" + n.Content + "\nС уважением,\nВаш Заметочник";
                            smtp.Send(m);                            
                        }

                        n.NeedToNotify = false;
                    }
                }
                nc.SaveChanges();
            }
        }

        public void Dispose()
        { }

    }
}