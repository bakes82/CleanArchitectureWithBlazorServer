using Application.Example.Features.Customers.DTOs;
using Application.Example.Features.Customers.Specifications;

namespace Application.Example.Features.Customers.Queries.Export;

public class ExportCustomersQuery : CustomerAdvancedFilter, IRequest<Result<byte[]>>
{
    public CustomerAdvancedPaginationSpec Specification => new CustomerAdvancedPaginationSpec(this);
}

public class ExportCustomersQueryHandler : IRequestHandler<ExportCustomersQuery, Result<byte[]>>
{
    private readonly IApplicationDbContext                         _context;
    private readonly CustomerDto                                   _dto = new CustomerDto();
    private readonly IExcelService                                 _excelService;
    private readonly IStringLocalizer<ExportCustomersQueryHandler> _localizer;
    private readonly IMapper                                       _mapper;

    public ExportCustomersQueryHandler(IApplicationDbContext context, IMapper mapper, IExcelService excelService, IStringLocalizer<ExportCustomersQueryHandler> localizer)
    {
        _context      = context;
        _mapper       = mapper;
        _excelService = excelService;
        _localizer    = localizer;
    }

    public async Task<Result<byte[]>> Handle(ExportCustomersQuery request, CancellationToken cancellationToken)
    {
        // TODO: Implement ExportCustomersQueryHandler method 
        List<CustomerDto> data = await _context.Customers.ApplySpecification(request.Specification)
                                               .OrderBy($"{request.OrderBy} {request.SortDirection}")
                                               .ProjectTo<CustomerDto>(_mapper.ConfigurationProvider)
                                               .AsNoTracking()
                                               .ToListAsync(cancellationToken);
        byte[] result = await _excelService.ExportAsync(data, new Dictionary<string, Func<CustomerDto, object?>>
                                                              {
                                                                  // TODO: Define the fields that should be exported, for example:
                                                                  { _localizer[_dto.GetMemberDescription(x => x.Id)], item => item.Id },
                                                                  { _localizer[_dto.GetMemberDescription(x => x.Name)], item => item.Name },
                                                                  { _localizer[_dto.GetMemberDescription(x => x.Description)], item => item.Description }
                                                              }, _localizer[_dto.GetClassDescription()]);
        return await Result<byte[]>.SuccessAsync(result);
        ;
    }
}
#nullable disable warnings
public class CustomersExportSpecification : Specification<Customer>
{
    public CustomersExportSpecification(ExportCustomersQuery query)
    {
        DateTime today = DateTime.Now.Date;
        DateTime start = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 00:00:00", CultureInfo.CurrentCulture);
        DateTime end   = Convert.ToDateTime(today.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) + " 23:59:59", CultureInfo.CurrentCulture);
        DateTime last30day = Convert.ToDateTime(today.AddDays(-30)
                                                     .ToString("yyyy-MM-dd", CultureInfo.CurrentCulture) +
                                                " 00:00:00", CultureInfo.CurrentCulture);

        Query.Where(q => q.Name != null)
             .Where(q => q.Name!.Contains(query.Keyword) || q.Description!.Contains(query.Keyword), !string.IsNullOrEmpty(query.Keyword))
             .Where(q => q.CreatedBy == query.CurrentUser.UserId, query.ListView == CustomerListView.My && query.CurrentUser is not null)
             .Where(q => q.Created >= start && q.Created <= end, query.ListView == CustomerListView.CreatedToday)
             .Where(q => q.Created >= last30day, query.ListView                 == CustomerListView.Created30Days);
    }
}