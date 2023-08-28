using Application.Example.Features.Customers.Caching;
using Application.Example.Features.Customers.DTOs;

namespace Application.Example.Features.Customers.Commands.Create;

public class CreateCustomerCommand : ICacheInvalidatorRequest<Result<int>>
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
            CreateMap<CustomerDto, CreateCustomerCommand>()
                .ReverseMap();
        }
    }
}

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<int>>
{
    private readonly IApplicationDbContext                   _context;
    private readonly IStringLocalizer<CreateCustomerCommand> _localizer;
    private readonly IMapper                                 _mapper;

    public CreateCustomerCommandHandler(IApplicationDbContext context, IStringLocalizer<CreateCustomerCommand> localizer, IMapper mapper)
    {
        _context   = context;
        _localizer = localizer;
        _mapper    = mapper;
    }

    public async Task<Result<int>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement CreateCustomerCommandHandler method 
        CustomerDto? dto  = _mapper.Map<CustomerDto>(request);
        Customer?    item = _mapper.Map<Customer>(dto);
        // raise a create domain event
        item.AddDomainEvent(new CustomerCreatedEvent(item));
        _context.Customers.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}