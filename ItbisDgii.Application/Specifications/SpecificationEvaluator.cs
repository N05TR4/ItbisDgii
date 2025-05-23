using ItbisDgii.Application.Interfaces;
using ItbisDgii.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace ItbisDgii.Application.Specifications
{
    public class SpecificationEvaluator<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            // Apply filtering
            if (specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);
            }

            // Include eager-loaded properties
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            // Include eager-loaded string properties
            query = specification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

            // Apply ordering
            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            else if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            // Apply paging
            if (specification.IsPagingEnabled)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }

            return query;
        }
    }
}
