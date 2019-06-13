﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Models.Persistence;

namespace Tardigrade.Framework.Services
{
    /// <summary>
    /// Base service interface for Data Transfer Objects (DTOs) of a particular type.
    /// </summary>
    /// <typeparam name="Model">Model object associated with the service operations.</typeparam>
    /// <typeparam name="ModelPk">Unique identifier type for the model object type.</typeparam>
    /// <typeparam name="Dto">Data Transfer Object type associated with the service operations.</typeparam>
    /// <typeparam name="DtoPk">Unique identifier type for the Data Transfer Object type.</typeparam>
    public interface IDtoService<Model, ModelPk, Dto, DtoPk> : IObjectService<Dto, DtoPk>
    {
        /// <summary>
        /// Alternate count method that is based upon the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{Model, ModelPk}.Count(Expression{Func{Model, bool}})"/>
        /// </summary>
        int Count(Expression<Func<Model, bool>> filter = null);

        /// <summary>
        /// Alternate retrieve method that filters on the Model type instead of the Data Transfer Object type.
        /// <see cref="IObjectService{Model, ModelPk}.Retrieve(Expression{Func{Model, bool}}, PagingContext, Func{IQueryable{Model}, IOrderedQueryable{Model}}, Expression{Func{Model, object}}[])"/>
        /// </summary>
        IEnumerable<Dto> Retrieve(
            Expression<Func<Model, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<Model>, IOrderedQueryable<Model>> sortCondition = null,
            params Expression<Func<Model, object>>[] includes);
    }
}