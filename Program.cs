
using DaemonTest;
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
HashSet<Message> bundelMessages= new HashSet<Message>()
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
    TextBody = "Hello",
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
    TextBody = "Hello from bundling standart 1",
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
    TextBody = "Hello from bundling standart 2",
    Data = data,
    TimestampScheduled = DateTime.Now
};

List<Message> messages = new List<Message>()
{
    standardMessageWithBundling ,
    standardMessage ,
    messageWithData,
    standardMessage2WithBundling
};

Console.WriteLine("App starting.");
// Application
while (true)
{
    Console.WriteLine("Messages in the queue" + messages.Count);
    //select first message
    var m = messages.FirstOrDefault();
    if (m != null)
    {
        Console.WriteLine("Message Sending");

        switch (m.SendingStrategy)
        {
            case SendingStrategy.Normal:
                SendMail(m);
                break;
            case SendingStrategy.Bundling:
                // BundleMails(m);
                SendMail(m);
                break;
            default:
                break;
        }

    }
    //send message
    Console.WriteLine($"Sleeping {DateTime.Now:HH:mm:ss}");
    Thread.Sleep(1000);
}

// void BundleMails()
// {
//     foreach (var m in messages)
//     {
//         if(bundelMessages.)
//         bundelMessages.Add()
//     }
// }

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


