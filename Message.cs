using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaemonTest
{
    public class Message
    {
        public Message()
        {
            To = new HashSet<UserStructure>();
            CC= new HashSet<UserStructure>();
            BCC = new HashSet<UserStructure>();
        }
        public int MessageID { get; set; }
        public string ApplicationID { get; set; }
        public string From { get; set; }
        public HashSet<UserStructure> To { get; set; }
        public HashSet<UserStructure> CC { get; set; }
        public HashSet<UserStructure> BCC { get; set; }
        public string TextBody { get; set; }
        public bool IsHtml { get; set; }
        public string BundlingKey { get; set; }
        public int DelayTime { get; set; }
        public DateTime? TimestampSent { get; set; }
        public SendingStrategy SendingStrategy { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public DateTime TimestampScheduled { get; set; }
    }
    public enum SendingStrategy
    {
        Normal,
        Bundling,
        DelayWithSendLastMail,
        GroupMailing,
        BundlingWithDelay
    }
}
