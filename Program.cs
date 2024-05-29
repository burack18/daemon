
using DaemonTest;
using DaemonTest.extentions;
using System.Text;
using System.Text.RegularExpressions;

HashSet<UserStructure> users = new HashSet<UserStructure>()
{
    new UserStructure("aksak@mail.com","aksak"),
    new UserStructure("aksak2@mail.com","aksak2")
};
HashSet<UserStructure> receivers = new HashSet<UserStructure>()
{
    new UserStructure("aksak3@mail.com","aksak3"),
    new UserStructure("aksak4@mail.com","aksak4")
};
Dictionary<string, HashSet<Message>> bundelMessages = new Dictionary<string, HashSet<Message>>()
{
};
var standardMessage = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 0,
    MessageID = 1,
    SendingStrategy = SendingStrategy.Normal,
    TextBody = "Hello Standart",
    TimestampScheduled = DateTime.Now

};
var data = new Dictionary<string, string>
{
    { "{{FirstName}}", "omer Faruk" },
    {"{{LastName}}","Aksak" }
};

var messageWithData = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 0,
    MessageID = 1,
    SendingStrategy = SendingStrategy.Normal,
    TextBody = "Hello {{FirstName}} {{LastName}}",

    Data = data,
    TimestampScheduled = DateTime.Now
};

var standardMessageWithBundling = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    BundlingKey = "bnd",
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 3,
    MessageID = 1,
    TextBody = "Hello standardMessageWithBundling 1",
    SendingStrategy = SendingStrategy.Bundling,
    Data = data,
    TimestampScheduled = DateTime.Now
};
var standardMessage2WithBundling = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    BundlingKey = "bnd",
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 3,
    MessageID = 1,
    SendingStrategy = SendingStrategy.Bundling,
    TextBody = "Hello from  standardMessageWithBundling 2",
    Data = data,
    TimestampScheduled = DateTime.Now
};

var messageWithSendLastDelay = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    BundlingKey = "bnd",
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 1,
    MessageID = 1,
    SendingStrategy = SendingStrategy.DelayWithSendLastMail,
    TextBody = "Hello from messageWithSendLastDelay1",
    Data = data,
    TimestampScheduled = DateTime.Now
};

var messageWithSendLastDelay2 = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    BundlingKey = "bnd",
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 1,
    MessageID = 1,
    SendingStrategy = SendingStrategy.DelayWithSendLastMail,
    TextBody = "Hello from messageWithSendLastDelay2",
    Data = data,
    TimestampScheduled = DateTime.Now.AddMinutes(1),
};
var messageWithSendLastDelay3 = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    BundlingKey = "bnd2",
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 1,
    MessageID = 1,
    SendingStrategy = SendingStrategy.DelayWithSendLastMail,
    TextBody = "Hello from messageWithSendLastDelay",
    Data = data,
    TimestampScheduled = DateTime.Now.AddMinutes(1),
    
};


var messageWithSendLastDelay4 = new Message()
{
    ApplicationID = Guid.NewGuid().ToString(),
    CC = users,
    To = receivers,
    BundlingKey = "bnd",
    From = "ofa@mail.com",
    IsHtml = false,
    DelayTime = 3,
    MessageID = 1,
    SendingStrategy = SendingStrategy.DelayWithSendLastMail,
    TextBody = "Hello from messageWithSendLastDelay4",
    Data = data,
    TimestampScheduled = DateTime.Now.AddMinutes(10),
};

List<Message> messages = new()
{
    standardMessageWithBundling ,
    standardMessage ,
    messageWithData,
    standardMessage2WithBundling,
    messageWithSendLastDelay,
    messageWithSendLastDelay2,
    messageWithSendLastDelay3,
    messageWithSendLastDelay4,
};

Console.WriteLine("App starting.");

// fetch mails





// Application



while (true)
{

    Console.WriteLine("Messages in the queue" + messages.Count);
    //select first message
    var m = messages.GroupBy(x => x.BundlingKey);


    List<Message>  normalMessages = messages.Where(x => x.SendingStrategy == SendingStrategy.Normal && x.TimestampSent==null).ToList();

    var bundlingWithDelay = messages.Where(x => x.SendingStrategy == SendingStrategy.BundlingWithDelay && x.TimestampSent == null).GroupBy(x => x.BundlingKey);
    var lastMessageStrategyGroups = messages.Where(x => x.SendingStrategy == SendingStrategy.DelayWithSendLastMail && x.TimestampSent == null).GroupBy(x => x.BundlingKey);
    var bundlingMessages = messages.Where(x => x.SendingStrategy == SendingStrategy.Bundling && x.TimestampSent == null).GroupBy(x => x.BundlingKey);




    var groupMailing = messages.Where(x => x.SendingStrategy == SendingStrategy.GroupMailing && x.TimestampSent == null).ToList();


    if (normalMessages.Count > 0)
    {
        foreach (var message in normalMessages)
        {
            SendMail(message);
            message.TimestampSent = DateTime.Now;
        }
    }

    if (bundlingMessages.Any())
    {
        foreach (var messageGroup in bundlingMessages)
        {
            var lastMessage = messageGroup.Last();
            var temp = lastMessage.TextBody;

            foreach (var message in messageGroup)
            {
                temp += message.TextBody;
                message.TimestampSent = DateTime.Now;
            }
            lastMessage.TextBody = temp;
            SendMail(lastMessage);
        }
    }


    foreach (var group in bundlingWithDelay)
    {
        var bundleGroup = BundleGroup(group);
        foreach (var message in bundleGroup)
        {
            SendMail(message);
        }
    }


    foreach (var lastMessageGroup in lastMessageStrategyGroups)
    {
        var lastMessages = LastMails(lastMessageGroup.First(), new List<Message>(), lastMessageGroup.ToList());
        foreach (var message in lastMessages)
        {
            if (DateTime.Now > message.TimestampScheduled.AddMinutes(message.DelayTime))
            {
                SendMail(message);
                message.TimestampSent= DateTime.Now;
                foreach (var mes in lastMessageGroup.Where(x => lastMessages.Any(y => y != x)))
                {
                    mes.TimestampSent = DateTime.Now;
                }
            }
        }
    }


    if (groupMailing.Count > 0)
    {

        for (int i = 0; i < groupMailing.Count; i += 50)
        {
            var mails = groupMailing.Skip(i).Take(50);
            Thread t = new(() =>
            {
                foreach (var mail in mails)
                {
                    SendMail(mail);
                }
            });
            t.Start();
        }
    }


    //send message
    Console.WriteLine($"Sleeping {DateTime.Now:HH:mm:ss}");
    Thread.Sleep(1000);
}

List<Message> BundleGroup(IGrouping<string, Message> messages)
{
    return GroupMessages(messages.First(), new List<Message>(), messages.ToList());
}



void SendMail(Message m)
{
    //Template Change
    if (m.Data != null && m.Data.Count > 0)
    {
        string messageContent = m.TextBody;
        foreach (var key in m.Data.Keys)
        {
            messageContent = Regex.Replace(messageContent, $"{key}", m.Data[key], RegexOptions.IgnoreCase);
        }
        m.TextBody = messageContent;
    }
    Console.WriteLine(m.TextBody);
    messages.Remove(m);
    Console.WriteLine("Message Sended");
}


List<Message> LastMails(Message? m, List<Message> newList, List<Message> messages)
{
    if (m == null || !messages.Any()) return newList;
    var nl = messages.Where(x => x != m && x.TimestampScheduled < m.TimestampScheduled.AddMinutes(m.DelayTime));

    if (!nl.Any())
    {
        newList.Add(m);
        var next = messages.Where(x => x != m).FirstOrDefault();
        return LastMails(next, newList, messages.Where(x => x != m && !newList.Any(y => y == x)).ToList());
    }

    var last = nl.OrderByDescending(x => x.TimestampScheduled).First();

    return LastMails(last, newList, messages.Where(x => x != m && !newList.Any(y => y == x)).ToList());
}
List<Message> GroupMessages(Message? m, List<Message> newList, List<Message> messages)
{
    if (m == null || !messages.Any()) return newList;
    var nl = messages.Where(x => x != m && x.TimestampScheduled < m.TimestampScheduled.AddMinutes(m.DelayTime));
    var temp = m.TextBody;

    foreach (var mes in nl)
    {
        temp += nl;
    }
    m.TextBody = temp;

    if (!nl.Any())
    {
        newList.Add(m);
        var next = messages.Where(x => x != m).FirstOrDefault();
        return LastMails(next, newList, messages.Where(x => x != m && !newList.Any(y => y == x)).ToList());
    }

    var last = nl.OrderByDescending(x => x.TimestampScheduled).First();
    last.TextBody = temp;
    return LastMails(last, newList, messages.Where(x => x != m && !newList.Any(y => y == x)).ToList());
}
