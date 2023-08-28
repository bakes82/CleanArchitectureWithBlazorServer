namespace CleanArchitecture.Blazor.Domain.Events;

public class CustomerCreatedEvent : DomainEvent
{
    public CustomerCreatedEvent(Customer item)
    {
        Item = item;
    }

    public Customer Item { get; }
}