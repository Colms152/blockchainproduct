using System;
using System.Net.Mail;
//using System.Web.UI.WebControls;

namespace eventchain.Controllers
{
    public partial class SendMail //: System.Web.UI.Page
    {
        protected void btn_SendMessage_Click(object sender, EventArgs e)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

            smtpClient.Credentials = new System.Net.NetworkCredential("eventchainire@gmail.com", "Event_chain1!");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mailMessage = new MailMessage("eventchainire@gmail.com", "colms152@gmail.com");
            mailMessage.Subject = "Your Ticket";
            mailMessage.Body = "Test";

            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
