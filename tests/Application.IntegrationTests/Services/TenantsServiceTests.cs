using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces.MultiTenant;
using CleanArchitecture.Blazor.Domain.Entities;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Application.IntegrationTests.Services;

using static Testing;

public class TenantsServiceTests : TestBase
{
    [SetUp]
    public async Task InitData()
    {
        await AddAsync(new Tenant { Name = "Test1", Description = "Test1" });
        await AddAsync(new Tenant { Name = "Test2", Description = "Text2" });
    }

    [Test]
    public void ShouldLoadDataSource()
    {
        ITenantService tenantsService = CreateTenantsService();
        tenantsService.Initialize();
        int count = tenantsService.DataSource.Count();
        Assert.AreEqual(2, count);
    }

    [Test]
    public async Task ShouldUpdateDataSource()
    {
        await AddAsync(new Tenant { Name = "Test3", Description = "Test3" });
        ITenantService tenantsService = CreateTenantsService();
        await tenantsService.Refresh();
        int count = tenantsService.DataSource.Count();
        Assert.AreEqual(3, count);
    }
}