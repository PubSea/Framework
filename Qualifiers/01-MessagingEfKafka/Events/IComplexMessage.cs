namespace MessagingEfKafka.Events;

public interface IComplexMessage
{
    InsideMessage InsideMessage { get; set; }
    List<int> Numbers { get; set; }
    IList<int> InterfaceNumbers { get; set; }
    List<string> String { get; set; }
    Dictionary<long, string> Dict { get; set; }
    IDictionary<long, string> InterfaceDict { get; set; }
}
