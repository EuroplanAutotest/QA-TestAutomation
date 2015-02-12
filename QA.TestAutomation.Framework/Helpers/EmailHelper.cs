using System;
using System.Collections.Generic;
using System.Threading;
using OpenPop.Mime;
using OpenPop.Pop3;

namespace QA.TestAutomation.Framework.Helpers
{
    public static class EmailHelper
    {
        public static List<Message> GetAllMessages(
            string hostname,
            int port,
            bool useSsl,
            string username,
            string password)
        {
            using (var client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);
                var messageCount = client.GetMessageCount();
                var allMessages = new List<Message>(messageCount);

                // Messages are numbered in the interval: [1, messageCount]
                // Ergo: message numbers are 1-based.
                // Most servers give the latest message the highest number
                for (int i = messageCount; i > 0; i--)
                {
                    allMessages.Add(client.GetMessage(i));
                }

                return allMessages;
            }
        }

        public static string HtmlToString(this Message message)
        {
            var html = message.FindFirstHtmlVersion();
            var str = html.BodyEncoding.GetString(html.Body);
            return str;
        }

        public static void WaitForNewMessage(
            string hostname,
            int port,
            bool useSsl,
            string username,
            string password,
            int messageCount,
            int timeout = 30)
        {
            var start = DateTime.Now;
            var mc = 0;
            using (var client = new Pop3Client())
            {
                while (mc < messageCount && (DateTime.Now - start).Seconds < timeout)
                {
                    Thread.Sleep(3000);
                    client.Connect(hostname, port, useSsl);
                    client.Authenticate(username, password);
                    mc = client.GetMessageCount();
                }
            }

            if (mc < messageCount)
            {
                throw new TimeoutException("Email has timed out");
            }
        }


        public static int GetMessageCount(
            string hostname,
            int port,
            bool useSsl,
            string username,
            string password)
        {
            using (var client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);
                return client.GetMessageCount();
            }
        }

        public static Message GetLastMessage(
            string hostname,
            int port,
            bool useSsl,
            string username,
            string password)
        {
            using (var client = new Pop3Client())
            {
                client.Connect(hostname, port, useSsl);
                client.Authenticate(username, password);
                var messageCount = client.GetMessageCount();
                return client.GetMessage( messageCount);
            }
        }
    }
}
