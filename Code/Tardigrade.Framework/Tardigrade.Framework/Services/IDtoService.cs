using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Models.Persistence;

namespace Tardigrade.Framework.Services
{
    /// <summary>
    /// Base service interface for Data Transfer Objects (DTOs) of a particular type.
    /// </summary>
    /// <typeparam name="TEntity">Model object associated with the service operations.</typeparam>
    /// <typeparam name="TDto">Data Transfer Object type associated with the service operations.</typeparam>
    /// <typeparam name="TDtoKey">Unique identifier type for the Data Transfer Object type.</typeparam>
    public interface IDtoService<TEntity, TDto, in TDtoKey> : IObjectService<TDto, TDtoKey>
    {
        /// <summary>
        /// Alternate count method that is based upon the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{TEntity, TKey}.Count(Expression{Func{TEntity, bool}})"/>
        /// </summary>
        int Count(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Alternate count method that is based upon the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{TEntity, TKey}.CountAsync(Expression{Func{TEntity, bool}}, CancellationToken)"/>
        /// </summary>
        Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alternate retrieve method that filters on the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{TEntity, TKey}.Retrieve(Expression{Func{TEntity, bool}}, PagingContext, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        IEnumerable<TDto> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Alternate retrieve method that filters on the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{TEntity, TKey}.RetrieveAsync(Expression{Func{TEntity, bool}}, PagingContext, Func{IQueryable{TEntity}, IOrderedQueryable{TEntity}}, CancellationToken, Expression{Func{TEntity, object}}[])"/>
        /// </summary>
        Task<IEnumerable<TDto>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);
    }
}