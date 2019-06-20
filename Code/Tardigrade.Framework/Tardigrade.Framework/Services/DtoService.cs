﻿using AutoMapper;
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
    public class DtoService<Model, ModelPk, Dto, DtoPk>
        : IObjectService<Dto, DtoPk>, IDtoService<Model, ModelPk, Dto, DtoPk>
        where Model : class
        where Dto : IHasUniqueIdentifier<DtoPk>
    {
        /// <summary>
        /// Service class used to encode/decode key values.
        /// </summary>
        protected IKeyEncoder<DtoPk, DtoPk> KeyEncoder { get; private set; }

        /// <summary>
        /// Mapper class for transforming from one object type to another.
        /// </summary>
        protected IMapper Mapper { get; private set; }

        /// <summary>
        /// Service class associated with the domain model object.
        /// </summary>
        protected IObjectService<Model, ModelPk> ObjectService { get; private set; }

        /// <summary>
        /// Create an instance of this service.
        /// </summary>
        /// <param name="mapper">Object mapper.</param>
        /// <param name="objectService">Service class for managing object operations.</param>
        /// <param name="keyEncoder">Service for encoding/decoding an object's unique identifier.</param>
        public DtoService(
            IMapper mapper,
            IObjectService<Model, ModelPk> objectService,
            IKeyEncoder<DtoPk, DtoPk> keyEncoder = null)
        {
            Mapper = mapper;
            ObjectService = objectService;
            KeyEncoder = keyEncoder;
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Count(Expression{Func{Dto, bool}})"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual int Count(Expression<Func<Dto, bool>> filter = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.Count(Expression{Func{Model, bool}})"/>
        /// </summary>
        public virtual int Count(Expression<Func<Model, bool>> filter = null)
        {
            return ObjectService.Count(filter);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.CountAsync(Expression{Func{Dto, bool}}, CancellationToken)"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual Task<int> CountAsync(
            Expression<Func<Dto, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.CountAsync(Expression{Func{Model, bool}}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<int> CountAsync(
            Expression<Func<Model, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            return await ObjectService.CountAsync(filter, cancellationToken);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Create(Dto)"/>
        /// </summary>
        public virtual Dto Create(Dto dto)
        {
            try
            {
                Model model = Mapper.Map<Model>(dto);
                model = ObjectService.Create(model);
                Dto created = Mapper.Map<Dto>(model);

                if (KeyEncoder != null)
                {
                    created.Id = KeyEncoder.Encode(created.Id);
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(Dto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Create(IEnumerable{Dto})"/>
        /// </summary>
        public virtual IEnumerable<Dto> Create(IEnumerable<Dto> dtos)
        {
            try
            {
                IEnumerable<Model> models = Mapper.Map<IEnumerable<Model>>(dtos);
                models = ObjectService.Create(models);
                IEnumerable<Dto> created = Mapper.Map<IEnumerable<Dto>>(models);

                if (KeyEncoder != null)
                {
                    created.ToList().ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating objects of type {typeof(Dto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.CreateAsync(Dto, CancellationToken)"/>
        /// </summary>
        public virtual async Task<Dto> CreateAsync(Dto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                Model model = Mapper.Map<Model>(dto);
                model = await ObjectService.CreateAsync(model);
                Dto created = Mapper.Map<Dto>(model);

                if (KeyEncoder != null)
                {
                    created.Id = KeyEncoder.Encode(created.Id);
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating an object of type {typeof(Dto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.CreateAsync(IEnumerable{Dto}, CancellationToken)"/>
        /// </summary>
        public virtual async Task<IEnumerable<Dto>> CreateAsync(
            IEnumerable<Dto> dtos,
            CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<Model> models = Mapper.Map<IEnumerable<Model>>(dtos);
                models = await ObjectService.CreateAsync(models);
                IEnumerable<Dto> created = Mapper.Map<IEnumerable<Dto>>(models);

                if (KeyEncoder != null)
                {
                    created.ToList().ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                }

                return created;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error creating objects of type {typeof(Dto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Delete(Dto)"/>
        /// </summary>
        public virtual void Delete(Dto dto)
        {
            Model model = Mapper.Map<Model>(dto);
            ObjectService.Delete(model);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Delete(DtoPk)"/>
        /// </summary>
        public virtual void Delete(DtoPk id)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                ModelPk modelId = Mapper.Map<ModelPk>(id);
                ObjectService.Delete(modelId);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(Dto).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.DeleteAsync(Dto, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(Dto dto, CancellationToken cancellationToken = default)
        {
            Model model = Mapper.Map<Model>(dto);
            await ObjectService.DeleteAsync(model);
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.DeleteAsync(DtoPk, CancellationToken)"/>
        /// </summary>
        public virtual async Task DeleteAsync(DtoPk id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                ModelPk modelId = Mapper.Map<ModelPk>(id);
                await ObjectService.DeleteAsync(modelId);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error deleting an object of type {typeof(Dto).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Exists(DtoPk)"/>
        /// </summary>
        public virtual bool Exists(DtoPk id)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                ModelPk modelId = Mapper.Map<ModelPk>(id);

                return ObjectService.Exists(modelId);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error determing whether an object of type {typeof(Dto).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.ExistsAsync(DtoPk, CancellationToken)"/>
        /// </summary>
        public virtual async Task<bool> ExistsAsync(DtoPk id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                ModelPk modelId = Mapper.Map<ModelPk>(id);

                return await ObjectService.ExistsAsync(modelId);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error determing whether an object of type {typeof(Dto).Name} with unique identifier of {id} exists.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Retrieve(DtoPk, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        /// <param name="id">Unique identifier for the instance.</param>
        /// <param name="includes">Not supported.</param>
        public virtual Dto Retrieve(DtoPk id, params Expression<Func<Dto, object>>[] includes)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                Dto dto = default(Dto);
                ModelPk modelId = Mapper.Map<ModelPk>(id);
                Model model = ObjectService.Retrieve(modelId);

                if (model != null)
                {
                    dto = Mapper.Map<Dto>(model);

                    if (KeyEncoder != null)
                    {
                        dto.Id = KeyEncoder.Encode(dto.Id);
                    }
                }

                return dto;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(Dto).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Retrieve(Expression{Func{Dto, bool}}, PagingContext, Func{IQueryable{Dto}, IOrderedQueryable{Dto}}, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual IEnumerable<Dto> Retrieve(
            Expression<Func<Dto, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<Dto>, IOrderedQueryable<Dto>> sortCondition = null,
            params Expression<Func<Dto, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.Retrieve(Expression{Func{Model, bool}}, PagingContext, Func{IQueryable{Model}, IOrderedQueryable{Model}}, Expression{Func{Model, object}}[])"/>
        /// </summary>
        public virtual IEnumerable<Dto> Retrieve(
            Expression<Func<Model, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<Model>, IOrderedQueryable<Model>> sortCondition = null,
            params Expression<Func<Model, object>>[] includes)
        {
            try
            {
                List<Dto> dtos = new List<Dto>();
                IEnumerable<Model> models = ObjectService.Retrieve(filter, pagingContext, sortCondition, includes);

                if (models?.Count() > 0)
                {
                    dtos = Mapper.Map<List<Dto>>(models);

                    if (KeyEncoder != null)
                    {
                        dtos.ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                    }
                }

                return dtos;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(Dto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.RetrieveAsync(DtoPk, CancellationToken, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        public virtual async Task<Dto> RetrieveAsync(
            DtoPk id,
            CancellationToken cancellationToken = default,
            params Expression<Func<Dto, object>>[] includes)
        {
            try
            {
                if (KeyEncoder != null)
                {
                    id = KeyEncoder.Decode(id);
                }

                Dto dto = default(Dto);
                ModelPk modelId = Mapper.Map<ModelPk>(id);
                Model model = await ObjectService.RetrieveAsync(modelId);

                if (model != null)
                {
                    dto = Mapper.Map<Dto>(model);

                    if (KeyEncoder != null)
                    {
                        dto.Id = KeyEncoder.Encode(dto.Id);
                    }
                }

                return dto;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving an object of type {typeof(Dto).Name} with a unique identifier of {id}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.RetrieveAsync(Expression{Func{Dto, bool}}, PagingContext, Func{IQueryable{Dto}, IOrderedQueryable{Dto}}, CancellationToken, Expression{Func{Dto, object}}[])"/>
        /// </summary>
        /// <exception cref="NotImplementedException">Not supported.</exception>
        public virtual Task<IEnumerable<Dto>> RetrieveAsync(
            Expression<Func<Dto, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<Dto>, IOrderedQueryable<Dto>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<Dto, object>>[] includes)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="IDtoService{Model, ModelPk, Dto, DtoPk}.Retrieve(Expression{Func{Model, bool}}, PagingContext, Func{IQueryable{Model}, IOrderedQueryable{Model}}, Expression{Func{Model, object}}[])"/>
        /// </summary>
        public virtual async Task<IEnumerable<Dto>> RetrieveAsync(
            Expression<Func<Model, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<Model>, IOrderedQueryable<Model>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<Model, object>>[] includes)
        {
            try
            {
                List<Dto> dtos = new List<Dto>();
                IEnumerable<Model> models = await ObjectService.RetrieveAsync(
                    filter,
                    pagingContext,
                    sortCondition,
                    cancellationToken,
                    includes);

                if (models?.Count() > 0)
                {
                    dtos = Mapper.Map<List<Dto>>(models);

                    if (KeyEncoder != null)
                    {
                        dtos.ForEach(d => d.Id = KeyEncoder.Encode(d.Id));
                    }
                }

                return dtos;
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error retrieving objects of type {typeof(Dto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.Update(Dto)"/>
        /// </summary>
        public virtual void Update(Dto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                if (KeyEncoder != null)
                {
                    dto.Id = KeyEncoder.Decode(dto.Id);
                }

                ModelPk modelId = Mapper.Map<ModelPk>(dto.Id);
                Model originalModel = ObjectService.Retrieve(modelId);

                if (originalModel == null)
                {
                    throw new NotFoundException($"Error updating an object of type {typeof(Dto).Name} as identifier of {dto.Id} not found.");
                }

                Mapper.Map(dto, originalModel);
                ObjectService.Update(originalModel);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(Dto).Name}.", e);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{Dto, DtoPk}.UpdateAsync(Dto, CancellationToken)"/>
        /// </summary>
        public virtual async Task UpdateAsync(Dto dto, CancellationToken cancellationToken = default)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            try
            {
                if (KeyEncoder != null)
                {
                    dto.Id = KeyEncoder.Decode(dto.Id);
                }

                ModelPk modelId = Mapper.Map<ModelPk>(dto.Id);
                Model originalModel = await ObjectService.RetrieveAsync(modelId);

                if (originalModel == null)
                {
                    throw new NotFoundException($"Error updating an object of type {typeof(Dto).Name} as identifier of {dto.Id} not found.");
                }

                Mapper.Map(dto, originalModel);
                await ObjectService.UpdateAsync(originalModel);
            }
            catch (EncodingException e)
            {
                throw new ServiceException($"Error updating an object of type {typeof(Dto).Name}.", e);
            }
        }
    }
}