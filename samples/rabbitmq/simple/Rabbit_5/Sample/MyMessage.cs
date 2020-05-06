using NServiceBus;

public class MyMessage :
    IMessage
{
    static readonly string DataConst = new string('a', 8200);

    public string Data { get; set; } = DataConst;
}