using dialogbot.Models;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;

namespace dialogbot.Services
{
    public class StudentService
    {
        public LoginResponse Login(Login login)
        {

            using (var client = new HttpClient())
            {
                
                var json = JsonConvert.SerializeObject(login); // or JsonSerializer.Serialize if using System.Text.Json

                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); // use MediaTypeNames.Application.Json in Core 3.0+ and Standard 2.1+
                var loginUrl = new Uri("https://dilkapchatbotapi.azurewebsites.net/api/Login");
                //var loginUrl = new Uri("https://localhost:44341/api/Login");

                var response =  client.PostAsync(loginUrl, stringContent);
                response.Wait();

                var result = response.Result;
                var loginResponse = new LoginResponse();
                if (result.IsSuccessStatusCode)
                {
                    loginResponse = JsonConvert.DeserializeObject<LoginResponse>(result.Content.ReadAsStringAsync().Result);                     
                }
                return loginResponse;

            }


        }

        public void SendEmail(Email email)
        {


            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential("iam.mohiniawate@gmail.com", "12345@Mohini"),
                EnableSsl = true
            };


            client.Send("iam.mohiniawate@gmail.com", email.To, email.Subject, email.Body);


            //MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            //mail.From = new MailAddress("iam.mohiniawate@gmail.com");
            //mail.To.Add(email.To);
            //mail.Subject =email.Subject;
            //mail.Body = email.Body;

            //SmtpServer.Port = 587;
            //SmtpServer.Credentials = new System.Net.NetworkCredential("iam.mohiniawate@gmail.com", "12345@Mohini");
            //SmtpServer.EnableSsl = true;

            //SmtpServer.Send(mail);
        }

        public async void SendEmailViaSendGrid(Email email)
        {          

          //  var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient("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");
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("mohiniawate275@outlook.com", "Dilkap ChatBot"),
                Subject = email.Subject,
                PlainTextContent = email.Body
            };
            msg.AddTo(new EmailAddress(email.To));
            var response = await client.SendEmailAsync(msg);

            // A success status code means SendGrid received the email request and will process it.
            // Errors can still occur when SendGrid tries to send the email. 
            // If email is not received, use this URL to debug: https://app.sendgrid.com/email_activity 
            Console.WriteLine(response.IsSuccessStatusCode ? "Email queued successfully!" : "Something went wrong!");

        }

    }
}
