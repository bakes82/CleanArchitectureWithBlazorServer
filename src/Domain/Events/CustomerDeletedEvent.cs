namespace CleanArchitecture.Blazor.Domain.Events;

public class CustomerDeletedEvent : DomainEvent
{
    public CustomerDeletedEvent(Customer item)
    {
        Item = item;
    }

    public Customer Item { get; }
}