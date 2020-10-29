using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Security.Privacy;

namespace Tardigrade.Framework.Services
{
    /// <summary>
    /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}"/>
    /// </summary>
    public class DtoService<TEntity, TEntityKey, TDto, TDtoKey> : IDtoService<TEntity, TEntityKey, TDto, TDtoKey>
        where TEntity : class
        where TDto : IHasUniqueIdentifier<TDtoKey>
    {
        /// <summary>
        /// Service class used to encode/decode key values.
        /// </summary>
        protected IKeyEncoder<TDtoKey, TDtoKey> KeyEncoder { get; }

        /// <summary>
        /// Mapper class for transforming from one object type to another.
        /// </summary>
        protected IMapper Mapper { get; }

        /// <summary>
        /// Service class associated with the domain model object.
        /// </summary>
        protected IObjectService<TEntity, TEntityKey> ObjectService { get; }

        /// <summary>
        /// Create an instance of this service.
        /// </summary>
        /// <param name="mapper">Object mapper.</param>
        /// <param name="objectService">Service class for managing object operations.</param>
        /// <param name="keyEncoder">Service for encoding/decoding an object's unique identifier.</param>
        public DtoService(
            IMapper mapper,
            IObjectService<TEntity, TEntityKey> objectService,
            IKeyEncoder<TDtoKey, TDtoKey> keyEncoder = null)
        {
            Mapper = mapper;
            ObjectService = objectService;
            KeyEncoder = keyEncoder;
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Count(Expression{Func{Dto, bool}})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual int Count(Expression<Func<TDto, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.Count(Expression{Func{Model, bool}})"/>
        /// </summary>
        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            return ObjectService.Count(filter);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.CountAsync(Expression{Func{Dto, bool}}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual Task<int> CountAsync(
            Expression<Func<TDto, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.CountAsync(Expression{Func{Model, bool}}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            return await ObjectService.CountAsync(filter, cancellationToken);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Create(Dto)"/>
        /// </summary>
        public virtual TDto Create(TDto item)
        {
            try
            {
                var model = Mapper.Map<TEntity>(item);
                model = ObjectService.Create(model);
                var created = Mapper.Map<TDto>(model);

                if (KeyEncoder != null)
                {
                    created.Id = KeyEncoder.Encode(created.Id);
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(TDto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Create(IEnumerable{Dto})"/>
        /// </summary>
        public virtual IEnumerable<TDto> Create(IEnumerable<TDto> items)
        {
            try
            {
                var models = Mapper.Map<IEnumerable<TEntity>>(items);
                models = ObjectService.Create(models);
                List<TDto> created = Mapper.Map<IEnumerable<TDto>>(models).ToList();

                if (KeyEncoder != null)
                {
                    created.ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating objects of type {typeof(TDto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.CreateAsync(Dto, CancellationToken)"/>
        /// </summary>
        public virtual async Task<TDto> CreateAsync(TDto item, CancellationToken cancellationToken = default)
        {
            try
            {
                var model = Mapper.Map<TEntity>(item);
                model = await ObjectService.CreateAsync(model, cancellationToken);
                var created = Mapper.Map<TDto>(model);

                if (KeyEncoder != null)
                {
                    created.Id = KeyEncoder.Encode(created.Id);
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(TDto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.CreateAsync(IEnumerable{Dto}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> CreateAsync(
            IEnumerable<TDto> items,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var models = Mapper.Map<IEnumerable<TEntity>>(items);
                models = await ObjectService.CreateAsync(models, cancellationToken);
                List<TDto> created = Mapper.Map<IEnumerable<TDto>>(models).ToList();

                if (KeyEncoder != null)
                {
                    created.ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating objects of type {typeof(TDto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Delete(Dto)"/>
        /// </summary>
        public virtual void Delete(TDto item)
        {
            var model = Mapper.Map<TEntity>(item);
            ObjectService.Delete(model);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.DeleteAsync(Dto, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(TDto item, CancellationToken cancellationToken = default)
        {
            var model = Mapper.Map<TEntity>(item);
            await ObjectService.DeleteAsync(model, cancellationToken);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Exists(DtoPk)"/>
        /// </summary>
        public virtual bool Exists(TDtoKey id)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                var modelId = Mapper.Map<TEntityKey>(id);

                return ObjectService.Exists(modelId);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error determining whether an object of type {typeof(TDto).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.ExistsAsync(DtoPk, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(TDtoKey id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                var modelId = Mapper.Map<TEntityKey>(id);

                return await ObjectService.ExistsAsync(modelId, cancellationToken);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error determining whether an object of type {typeof(TDto).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Retrieve(DtoPk, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="includes">Not supported.</param>
        public virtual TDto Retrieve(TDtoKey id, params Expression<Func<TDto, object>>[] includes)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                TDto item = default;
                var modelId = Mapper.Map<TEntityKey>(id);
                TEntity model = ObjectService.Retrieve(modelId);

                if (model != null)
                {
                    item = Mapper.Map<TDto>(model);

                    if (KeyEncoder != null)
                    {
                        item.Id = KeyEncoder.Encode(item.Id);
                    }
                }

                return item;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(TDto).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Retrieve(Expression{Func{Dto, bool}}, PagingContext, Func{IQueryable{Dto}, IOrderedQueryable{Dto}}, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual IEnumerable<TDto> Retrieve(
            Expression<Func<TDto, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TDto>, IOrderedQueryable<TDto>> sortCondition = null,
            params Expression<Func<TDto, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.Retrieve(Expression{Func{Model, bool}}, PagingContext, Func{IQueryable{Model}, IOrderedQueryable{Model}}, Expression{Func{Model, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<TDto> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var items = new List<TDto>();
                IEnumerable<TEntity> models = ObjectService.Retrieve(filter, pagingContext, sortCondition, includes);

                if (models?.Count() > 0)
                {
                    items = Mapper.Map<List<TDto>>(models);

                    if (KeyEncoder != null)
                    {
                        items.ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                    }
                }

                return items;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(TDto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.RetrieveAsync(DtoPk, CancellationToken, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        public virtual async Task<TDto> RetrieveAsync(
            TDtoKey id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TDto, object>>[] includes)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                TDto item = default;
                var modelId = Mapper.Map<TEntityKey>(id);
                TEntity model = await ObjectService.RetrieveAsync(modelId, cancellationToken);

                if (model != null)
                {
                    item = Mapper.Map<TDto>(model);

                    if (KeyEncoder != null)
                    {
                        item.Id = KeyEncoder.Encode(item.Id);
                    }
                }

                return item;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(TDto).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.RetrieveAsync(Expression{Func{Dto, bool}}, PagingContext, Func{IQueryable{Dto}, IOrderedQueryable{Dto}}, CancellationToken, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual Task<IEnumerable<TDto>> RetrieveAsync(
            Expression<Func<TDto, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TDto>, IOrderedQueryable<TDto>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TDto, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.Retrieve(Expression{Func{Model, bool}}, PagingContext, Func{IQueryable{Model}, IOrderedQueryable{Model}}, Expression{Func{Model, object}}[])"/>
        /// </summary>
        public virtual async Task<IEnumerable<TDto>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            try
            {
                var items = new List<TDto>();
                IEnumerable<TEntity> models = await ObjectService.RetrieveAsync(
                    filter,
                    pagingContext,
                    sortCondition,
                    cancellationToken,
                    includes);

                if (models?.Count() > 0)
                {
                    items = Mapper.Map<List<TDto>>(models);

                    if (KeyEncoder != null)
                    {
                        items.ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                    }
                }

                return items;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(TDto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Update(Dto)"/>
        /// </summary>
        public virtual void Update(TDto item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                if (KeyEncoder != null)
                {
                    item.Id = KeyEncoder.Decode(item.Id);
                }

                var modelId = Mapper.Map<TEntityKey>(item.Id);
                TEntity originalModel = ObjectService.Retrieve(modelId);

                if (originalModel == null)
                {
                    throw new NotFoundException($"Error updating an object of type {typeof(TDto).Name} as identifier of {item.Id} not found.");
                }

                Mapper.Map(item, originalModel);
                ObjectService.Update(originalModel);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(TDto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.UpdateAsync(Dto, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(TDto item, CancellationToken cancellationToken = default)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            try
            {
                if (KeyEncoder != null)
                {
                    item.Id = KeyEncoder.Decode(item.Id);
                }

                var modelId = Mapper.Map<TEntityKey>(item.Id);
                TEntity originalModel = await ObjectService.RetrieveAsync(modelId, cancellationToken);

                if (originalModel == null)
                {
                    throw new NotFoundException($"Error updating an object of type {typeof(TDto).Name} as identifier of {item.Id} not found.");
                }

                Mapper.Map(item, originalModel);
                await ObjectService.UpdateAsync(originalModel, cancellationToken);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(TDto).Name}.", e);
            }
        }
    }
}