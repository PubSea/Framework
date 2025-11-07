using PubSea.Framework.Events;

namespace MessagingEfKafka.Events;

public class ComplexMessage : IComplexMessage, IIntegrationEvent
{
    public InsideMessage InsideMessage { get; set; } = new();
    public List<int> Numbers { get; set; } = [];
    public List<string> String { get; set; } = [];
    public Dictionary<long, string> Dict { get; set; } = [];
    public IList<int> InterfaceNumbers { get; set; } = [];
    public IDictionary<long, string> InterfaceDict { get; set; } = new Dictionary<long, string>();
}

public class InsideMessage
{
    public long UniqueIdentifier { get; set; }
    public string FullName { get; set; } = null!;
}