using Newtonsoft.Json.Linq;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;


namespace Book_store.MyHelpers
{
    public class EmailSender
    {
        public static void SendEmail(string toEmail, string username, string subject, string message)

        {

            string apiKey = "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";



            Configuration.Default.ApiKey["api-key"] = apiKey;



            var apiInstance = new TransactionalEmailsApi();

            string SenderName = "BookShelf";

            string SenderEmail = "sharafmo1994@gmail.com";



            SendSmtpEmailSender emailSender = new SendSmtpEmailSender(SenderName, SenderEmail);

            SendSmtpEmailTo emailReceiver1 = new SendSmtpEmailTo(toEmail, username);



            List<SendSmtpEmailTo> To = new List<SendSmtpEmailTo>();

            To.Add(emailReceiver1);



            string HtmlContent = null;

            string TextContent = message;



            var sendSmtpEmail = new SendSmtpEmail(emailSender, To, null, null, HtmlContent, TextContent, subject);

            CreateSmtpEmail result = apiInstance.SendTransacEmail(sendSmtpEmail);

            Console.WriteLine("Response: \n" + result.ToJson());

        }

    }

}
    

