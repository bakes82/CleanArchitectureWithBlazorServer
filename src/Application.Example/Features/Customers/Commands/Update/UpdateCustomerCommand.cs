using Application.Example.Features.Customers.Caching;
using Application.Example.Features.Customers.DTOs;

namespace Application.Example.Features.Customers.Commands.Update;

public class UpdateCustomerCommand : ICacheInvalidatorRequest<Result<int>>
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
            CreateMap<CustomerDto, UpdateCustomerCommand>()
                .ReverseMap();
        }
    }
}

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<int>>
{
    private readonly IApplicationDbContext                          _context;
    private readonly IStringLocalizer<UpdateCustomerCommandHandler> _localizer;
    private readonly IMapper                                        _mapper;

    public UpdateCustomerCommandHandler(IApplicationDbContext context, IStringLocalizer<UpdateCustomerCommandHandler> localizer, IMapper mapper)
    {
        _context   = context;
        _localizer = localizer;
        _mapper    = mapper;
    }

    public async Task<Result<int>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement UpdateCustomerCommandHandler method 
        Customer? item = await _context.Customers.FindAsync(new object[]
                                                            {
                                                                request.Id
                                                            }, cancellationToken) ??
                         throw new NotFoundException($"Customer with id: [{request.Id}] not found.");
        ;
        CustomerDto? dto = _mapper.Map<CustomerDto>(request);
        item = _mapper.Map(dto, item);
        // raise a update domain event
        item.AddDomainEvent(new CustomerUpdatedEvent(item));
        await _context.SaveChangesAsync(cancellationToken);
        return await Result<int>.SuccessAsync(item.Id);
    }
}