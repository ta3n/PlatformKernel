using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Pagination;
using SharedKernel.Pagination.Binders;
using SharedKernel.Pagination.Extensions;
using SharedKernel.Pagination.Utilities;

namespace SharedKernel.Pagination.Test;

public class UnitTest1
{
    [Fact]
    public void Sort_ChangesDirectionAndCombinesOrders()
    {
        var sort = new Sort(Direction.Asc, "Name");
        var combined = sort.Descending().And(new Sort(Direction.Asc, "CreatedAt"));

        Assert.True(sort.IsSorted());
        Assert.Equal(Direction.Desc, combined.Orders[0].Direction);
        Assert.Equal("CreatedAt", combined.Orders[1].Property);
    }

    [Fact]
    public void Pageable_ComputesOffsetAndNavigation()
    {
        var pageable = Pageable.Of(2, 25, true, new Sort("Name"));

        Assert.Equal(25, pageable.Offset);
        Assert.True(pageable.HasPrevious);
        Assert.Equal(3, pageable.Next.PageNumber);
        Assert.Equal(0, pageable.First.PageNumber);
    }

    [Fact]
    public void QueryStringExtension_ReturnsSingleAndMultipleValues()
    {
        var queryString = new QueryString("?page=1&sort=name,desc&sort=createdAt");
        var values = queryString.GetParameterValues("sort");

        Assert.Equal("1", queryString.GetParameter("page"));
        Assert.Collection(
            values,
            value => Assert.Equal("name,desc", value),
            value => Assert.Equal("createdAt", value));
    }

    [Fact]
    public void GeneratePaginationHttpHeaders_WritesExpectedMetadata()
    {
        var page = new Page<TestItem>(
            [new TestItem("a"), new TestItem("b")],
            Pageable.Of(1, 2),
            total: 5);

        var headers = page.GeneratePaginationHttpHeaders();

        Assert.Equal("5", headers["X-Total-Count"]);
        Assert.Contains("\"totalCount\":5", headers["X-Pagination"].ToString(), StringComparison.Ordinal);
        Assert.Contains("\"hasNext\":true", headers["X-Pagination"].ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task PageableBinder_BindsPageableFromQueryString()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString("?page=2&size=50&enabled=true&sort=name,desc&sort=code");

        var bindingContext = new DefaultModelBindingContext
        {
            ModelMetadata = new EmptyModelMetadataProvider().GetMetadataForType(typeof(IPageable)),
            ModelName = "pageable",
            ActionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor()),
            ValueProvider = new QueryStringValueProvider(
                BindingSource.Query,
                httpContext.Request.Query,
                System.Globalization.CultureInfo.InvariantCulture)
        };

        var binder = new PageableBinder();
        await binder.BindModelAsync(bindingContext);

        var pageable = Assert.IsType<Pageable>(bindingContext.Result.Model);
        Assert.Equal(2, pageable.PageNumber);
        Assert.Equal(50, pageable.PageSize);
        Assert.True(pageable.IsEnabled);
        Assert.Equal("name", pageable.Sort.Orders[0].Property);
        Assert.Equal(Direction.Desc, pageable.Sort.Orders[0].Direction);
        Assert.Equal("code", pageable.Sort.Orders[1].Property);
    }

    private sealed record TestItem(string Name);
}
