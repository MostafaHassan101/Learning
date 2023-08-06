using System.Net.Mail;
using System.Net;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new SmtpClient("sandbox.smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("78239281a333cf", "4bc96c7b2d8fb6"),
                EnableSsl = true
            };
            client.Send("from@example.com", "to@example.com", "Hello world11", "testbody");
            Console.WriteLine("Sent");
            Console.ReadLine();
        }
    }
}