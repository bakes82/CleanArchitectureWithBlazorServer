using Application.Example.Features.Customers.Caching;

namespace Application.Example.Features.Customers.Commands.Delete;

public class DeleteCustomerCommand : ICacheInvalidatorRequest<Result<int>>
{
    public DeleteCustomerCommand(int[] id)
    {
        Id = id;
    }

    public int[]                    Id                      { get; }
    public string                   CacheKey                => CustomerCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
}

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<int>>

{
    private readonly IApplicationDbContext                          _context;
    private readonly IStringLocalizer<DeleteCustomerCommandHandler> _localizer;
    private readonly IMapper                                        _mapper;

    public DeleteCustomerCommandHandler(IApplicationDbContext context, IStringLocalizer<DeleteCustomerCommandHandler> localizer, IMapper mapper)
    {
        _context   = context;
        _localizer = localizer;
        _mapper    = mapper;
    }

    public async Task<Result<int>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement DeleteCheckedCustomersCommandHandler method 
        List<Customer> items = await _context.Customers.Where(x => request.Id.Contains(x.Id))
                                             .ToListAsync(cancellationToken);
        foreach (Customer item in items)
        {
            // raise a delete domain event
            item.AddDomainEvent(new CustomerDeletedEvent(item));
            _context.Customers.Remove(item);
        }

        int result = await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(result);
    }
}