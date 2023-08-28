namespace Application.Example.Features.Customers.Specifications;

public class CustomerByIdSpec : Specification<Customer>
{
    public CustomerByIdSpec(int id)
    {
        Query.Where(q => q.Id == id);
    }
}