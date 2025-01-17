using GraphQL;
using Lombiq.TrainingDemo.Models;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL.Queries;
using OrchardCore.DisplayManagement.Shapes;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using static Lombiq.TrainingDemo.GraphQL.Services.ContentItemTypeBuilder;

namespace Lombiq.TrainingDemo.GraphQL.Services;

// IGraphQLFilters can append conditions to the YesSql query, alter its result, or do both.
public class PersonAgeGraphQLFilter : IGraphQLFilter<ContentItem>
{
    private readonly IClock _clock;

    public PersonAgeGraphQLFilter(IClock clock) => _clock = clock;

    // While you can use this to execute some complex YesSql query it's best to stick with the IIndexAliasProvider
    // approach for such things.
    public Task<IQuery<ContentItem>> PreQueryAsync(IQuery<ContentItem> query, IResolveFieldContext context) =>
        Task.FromResult(query);

    // You can use this method to filter offline or in separate requests. This is less efficient but it's necessary if
    // the request can't be described as a single YesSql query. In this case we work off of a property that's not
    // indexed for demonstration's sake.
    public Task<IEnumerable<ContentItem>> PostQueryAsync(
        IEnumerable<ContentItem> contentItems,
        IResolveFieldContext context)
    {
        var (name, value) = context.Arguments.FirstOrDefault(
            argument => argument.Value.Value is int && argument.Key.StartsWithOrdinal(AgeFilterName));

        if (name != null && value.Value is int age)
        {
            var now = _clock.UtcNow;
            if (name == "age") name = "age_eq";
            var filterType = name[^2..]; // The name operator like gt, le, etc.

            contentItems = contentItems.Where(item =>
                item.As<PersonPart>()?.BirthDateUtc is { } birthDateUtc &&
                Filter((now - birthDateUtc).TotalYears(), age, filterType));
        }

        return Task.FromResult(contentItems);
    }

    private static bool Filter(int totalYears, int age, string filterType) =>
        filterType switch
        {
            "ge" => totalYears >= age,
            "gt" => totalYears > age,
            "le" => totalYears <= age,
            "lt" => totalYears < age,
            "ne" => totalYears != age,
            _ => totalYears == age,
        };
}

// END OF TRAINING SECTION: GraphQL

// NEXT STATION: Activities/ManagePersonsPermissionCheckerTask.cs
