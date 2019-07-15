using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiResort_Simulator
{
    public static class MessageFeed
    {
        private static Queue<string> news = new Queue<string>();
        public static void RegisterMessage(string message)
        {
            news.Enqueue(message);
        }

        public static void PrintNews(Action<string> PrintFunction)
        {
            while (news.Count > 0)
            {
                PrintFunction(news.Dequeue());
            }
        }
    }
}
