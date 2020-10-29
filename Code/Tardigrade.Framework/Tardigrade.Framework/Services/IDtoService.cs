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
    /// <typeparam name="TEntityKey">Unique identifier type for the model object type.</typeparam>
    /// <typeparam name="TDto">Data Transfer Object type associated with the service operations.</typeparam>
    /// <typeparam name="TDtoKey">Unique identifier type for the Data Transfer Object type.</typeparam>
    public interface IDtoService<TEntity, TEntityKey, TDto, in TDtoKey> : IObjectService<TDto, TDtoKey>
    {
        /// <summary>
        /// Alternate count method that is based upon the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{Model, ModelPk}.Count(Expression{Func{Model, bool}})"/>
        /// </summary>
        int Count(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Alternate count method that is based upon the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{Model, ModelPk}.CountAsync(Expression{Func{Model, bool}}, CancellationToken)"/>
        /// </summary>
        Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Alternate retrieve method that filters on the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{Model, ModelPk}.Retrieve(Expression{Func{Model, bool}}, PagingContext, Func{IQueryable{Model}, IOrderedQueryable{Model}}, Expression{Func{Model, object}}[])"/>
        /// </summary>
        IEnumerable<TDto> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes);

        /// <summary>
        /// Alternate retrieve method that filters on the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{Model, ModelPk}.RetrieveAsync(Expression{Func{Model, bool}}, PagingContext, Func{IQueryable{Model}, IOrderedQueryable{Model}}, CancellationToken, Expression{Func{Model, object}}[])"/>
        /// </summary>
        Task<IEnumerable<TDto>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes);
    }
}