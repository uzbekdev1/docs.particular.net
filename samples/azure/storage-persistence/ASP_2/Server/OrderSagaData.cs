using System;
using System.Collections.Generic;
using System.Text;
using NServiceBus;

#region sagadata

public class OrderSagaData :
    IContainSagaData
{
    public Guid Id { get; set; }
    public string Originator { get; set; }
    public string OriginalMessageId { get; set; }
    public Guid OrderId { get; set; }
    public string OrderDescription { get; set; }

    public List<string> ListOfStrings { get; set; } = new List<string>
    {
        "Hello World"
    };

    public List<int> ListOfINts { get; set; } = new List<int>
    {
        43, 42
    };

    public Nested Nested { get; set; } = new Nested();

    public int IntValue { get; set; } = 1;
    public long LongValue { get; set; } = 1;
    public double DoubleValue { get; set; } = 1.24;
    public byte[] BinaryValue { get; set; } = Encoding.UTF8.GetBytes("Hello World");
    public DateTime DateTimeValue { get; set; } = DateTime.Now;
    public DateTimeOffset DateTimeOffsetValue { get; set; } = DateTimeOffset.Now;
    public bool BooleanValue { get; set; } = true;
    public decimal DecimalValue { get; set; } = 1.2m;
    public float FloatValue { get; set; } = 1.2f;

    public string PretendsToBeAnArray { get; set; } = "[ Garbage ]";
    public string PretendsToBeAnObject { get; set; } = "{ \"Garbage\" }";
}

public class Nested
{
    public string Foo { get; set; } = "Foo";
    public string Bar { get; set; } = "Bar";
}

#endregion