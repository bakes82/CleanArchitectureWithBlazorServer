using Application.Example.Features.Customers.Caching;
using Application.Example.Features.Customers.DTOs;

namespace Application.Example.Features.Customers.Commands.AddEdit;

public class AddEditCustomerCommand : ICacheInvalidatorRequest<Result<int>>
{
    [Description("Id")]
    public int Id { get; set; }

    [Description("Name")]
    public string Name { get; set; } = String.Empty;

    [Description("Description")]
    public string? Description { get; set; }

    public string                   CacheKey                => CustomerCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CustomerDto, AddEditCustomerCommand>()
                .ReverseMap();
        }
    }
}

public class AddEditCustomerCommandHandler : IRequestHandler<AddEditCustomerCommand, Result<int>>
{
    private readonly IApplicationDbContext                           _context;
    private readonly IStringLocalizer<AddEditCustomerCommandHandler> _localizer;
    private readonly IMapper                                         _mapper;

    public AddEditCustomerCommandHandler(IApplicationDbContext context, IStringLocalizer<AddEditCustomerCommandHandler> localizer, IMapper mapper)
    {
        _context   = context;
        _localizer = localizer;
        _mapper    = mapper;
    }

    public async Task<Result<int>> Handle(AddEditCustomerCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement AddEditCustomerCommandHandler method 
        CustomerDto? dto = _mapper.Map<CustomerDto>(request);
        if (request.Id > 0)
        {
            Customer? item = await _context.Customers.FindAsync(new object[]
                                                                {
                                                                    request.Id
                                                                }, cancellationToken) ??
                             throw new NotFoundException($"Customer with id: [{request.Id}] not found.");
            item = _mapper.Map(dto, item);
            // raise a update domain event
            item.AddDomainEvent(new CustomerUpdatedEvent(item));
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
        else
        {
            Customer? item = _mapper.Map<Customer>(dto);
            // raise a create domain event
            item.AddDomainEvent(new CustomerCreatedEvent(item));
            _context.Customers.Add(item);
            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(item.Id);
        }
    }
}