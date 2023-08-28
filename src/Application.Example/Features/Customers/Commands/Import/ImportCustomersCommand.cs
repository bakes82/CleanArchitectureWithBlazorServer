using Application.Example.Features.Customers.Caching;
using Application.Example.Features.Customers.DTOs;

namespace Application.Example.Features.Customers.Commands.Import;

public class ImportCustomersCommand : ICacheInvalidatorRequest<Result<int>>
{
    public ImportCustomersCommand(string fileName, byte[] data)
    {
        FileName = fileName;
        Data     = data;
    }

    public string                   FileName                { get; set; }
    public byte[]                   Data                    { get; set; }
    public string                   CacheKey                => CustomerCacheKey.GetAllCacheKey;
    public CancellationTokenSource? SharedExpiryTokenSource => CustomerCacheKey.SharedExpiryTokenSource();
}

public record class CreateCustomersTemplateCommand : IRequest<Result<byte[]>>
{
}

public class ImportCustomersCommandHandler : IRequestHandler<CreateCustomersTemplateCommand, Result<byte[]>>, IRequestHandler<ImportCustomersCommand, Result<int>>
{
    private readonly IApplicationDbContext                           _context;
    private readonly CustomerDto                                     _dto = new CustomerDto();
    private readonly IExcelService                                   _excelService;
    private readonly IStringLocalizer<ImportCustomersCommandHandler> _localizer;
    private readonly IMapper                                         _mapper;

    public ImportCustomersCommandHandler(IApplicationDbContext context, IExcelService excelService, IStringLocalizer<ImportCustomersCommandHandler> localizer, IMapper mapper)
    {
        _context      = context;
        _localizer    = localizer;
        _excelService = excelService;
        _mapper       = mapper;
    }
#nullable disable warnings
    public async Task<Result<int>> Handle(ImportCustomersCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement ImportCustomersCommandHandler method
        IResult<IEnumerable<CustomerDto>>? result = await _excelService.ImportAsync(request.Data, new Dictionary<string, Func<DataRow, CustomerDto, object?>>
                                                                                                  {
                                                                                                      // TODO: Define the fields that should be read from Excel, for example:
                                                                                                      {
                                                                                                          _localizer[_dto.GetMemberDescription(x => x.Name)], (row, item) => item.Name = row[_localizer[_dto.GetMemberDescription(x => x.Name)]]
                                                                                                                                                                  .ToString()
                                                                                                      },
                                                                                                      {
                                                                                                          _localizer[_dto.GetMemberDescription(x => x.Description)], (row, item) => item.Description =
                                                                                                              row[_localizer[_dto.GetMemberDescription(x => x.Description)]]
                                                                                                                  .ToString()
                                                                                                      }
                                                                                                  }, _localizer[_dto.GetClassDescription()]);
        if (result.Succeeded && result.Data is not null)
        {
            foreach (CustomerDto? dto in result.Data)
            {
                bool exists = await _context.Customers.AnyAsync(x => x.Name == dto.Name, cancellationToken);
                if (!exists)
                {
                    Customer? item = _mapper.Map<Customer>(dto);
                    // add create domain events if this entity implement the IHasDomainEvent interface
                    // item.AddDomainEvent(new CustomerCreatedEvent(item));
                    await _context.Customers.AddAsync(item, cancellationToken);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return await Result<int>.SuccessAsync(result.Data.Count());
        }

        return await Result<int>.FailureAsync(result.Errors);
    }

    public async Task<Result<byte[]>> Handle(CreateCustomersTemplateCommand request, CancellationToken cancellationToken)
    {
        // TODO: Implement ImportCustomersCommandHandler method 
        string[]? fields = new string[]
                           {
                               // TODO: Define the fields that should be generate in the template, for example:
                               _localizer[_dto.GetMemberDescription(x => x.Name)],
                               _localizer[_dto.GetMemberDescription(x => x.Description)]
                           };
        byte[]? result = await _excelService.CreateTemplateAsync(fields, _localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
    }
}