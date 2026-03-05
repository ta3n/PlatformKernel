using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Liberty.Pagination.Binders;

public class PageableBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(
        ModelBinderProviderContext context
    )
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.ModelType == typeof(IPageable))
        {
            return new BinderTypeModelBinder(typeof(PageableBinder));
        }

        return null;
    }
}
