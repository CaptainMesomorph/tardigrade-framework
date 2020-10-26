using Audit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;
using Tardigrade.Framework.Services.Users;

namespace Tardigrade.Framework.AuditNET.Decorators
{
    /// <summary>
    /// Decorator class for auditing of the object service interface.
    /// </summary>
    /// <typeparam name="TEntity">Object type associated with the service operations.</typeparam>
    /// <typeparam name="TKey">Unique identifier type for the object type.</typeparam>
    public class ObjectServiceAuditDecorator<TEntity, TKey> : IObjectService<TEntity, TKey> where TEntity : IHasUniqueIdentifier<TKey>
    {
        private readonly IObjectService<TEntity, TKey> decoratedService;
        private readonly IReadOnlyRepository<TEntity, TKey> readOnlyRepository;
        private readonly IUserContext userContext;

        /// <summary>
        /// Create an instance of this Decorator.
        /// </summary>
        /// <param name="decoratedService">Object service that is being decorated.</param>
        /// <param name="readOnlyRepository">Read-only repository for data retrieval.</param>
        /// <param name="userContext">Contextual information on the current user.</param>
        /// <exception cref="ArgumentNullException">A parameter is null.</exception>
        public ObjectServiceAuditDecorator(
            IObjectService<TEntity, TKey> decoratedService,
            IReadOnlyRepository<TEntity, TKey> readOnlyRepository,
            IUserContext userContext)
        {
            this.decoratedService = decoratedService ?? throw new ArgumentNullException(nameof(decoratedService));
            this.readOnlyRepository =
                readOnlyRepository ?? throw new ArgumentNullException(nameof(readOnlyRepository));
            this.userContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            int count = decoratedService.Count(filter);

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{typeof(TEntity).Name}:Count",
                    ExtraFields = new { Count = count },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (var auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
                }
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }

            return count;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.CountAsync(Expression{Func{T, bool}}, CancellationToken)"/>
        /// </summary>
        public async Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            int count = await decoratedService.CountAsync(filter, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{typeof(TEntity).Name}:Count",
                    ExtraFields = new { Count = count },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }

            return count;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Create(IEnumerable{T})"/>
        /// </summary>
        public IEnumerable<TEntity> Create(IEnumerable<TEntity> items)
        {
            IEnumerable<TEntity> created = default;
            var isServiceException = false;

            try
            {
                using (var auditScope = AuditScope.Create($"{typeof(TEntity).Name}:Create+", () => created))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{items.GetType()}";

                    try
                    {
                        created = decoratedService.Create(items);
                    }
                    catch
                    {
                        isServiceException = true;
                        throw;
                    }
                }
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Create(T)"/>
        /// </summary>
        public TEntity Create(TEntity item)
        {
            TEntity created = default;
            var isServiceException = false;

            try
            {
                using (var auditScope = AuditScope.Create($"{typeof(TEntity).Name}:Create", () => created))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";

                    try
                    {
                        created = decoratedService.Create(item);
                    }
                    catch
                    {
                        isServiceException = true;
                        throw;
                    }
                }
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.CreateAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public async Task<IEnumerable<TEntity>> CreateAsync(
            IEnumerable<TEntity> items,
            CancellationToken cancellationToken = new CancellationToken())
        {
            IEnumerable<TEntity> created = default;
            AuditScope auditScope = null;
            var isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(TEntity).Name}:Create+", () => created);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{items.GetType()}";

                try
                {
                    created = await decoratedService.CreateAsync(items, cancellationToken);
                }
                catch
                {
                    isServiceException = true;
                    throw;
                }
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        public async Task<TEntity> CreateAsync(TEntity item, CancellationToken cancellationToken = new CancellationToken())
        {
            TEntity created = default;
            AuditScope auditScope = null;
            var isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(TEntity).Name}:Create", () => created);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";

                try
                {
                    created = await decoratedService.CreateAsync(item, cancellationToken);
                }
                catch
                {
                    isServiceException = true;
                    throw;
                }
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Delete(T)"/>
        /// </summary>
        public void Delete(TEntity item)
        {
            decoratedService.Delete(item);

            try
            {
                using (var auditScope = AuditScope.Create($"{typeof(TEntity).Name}:Delete", () => item))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
                    item = default;
                }
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public async Task DeleteAsync(TEntity item, CancellationToken cancellationToken = new CancellationToken())
        {
            await decoratedService.DeleteAsync(item, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(TEntity).Name}:Delete", () => item);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
                item = default;
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Exists(PK)"/>
        /// </summary>
        public bool Exists(TKey id)
        {
            bool exists = decoratedService.Exists(id);

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{typeof(TEntity).Name}:Exists",
                    ExtraFields = new { Id = id, Exists = exists },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (var auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
                }
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }

            return exists;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.ExistsAsync(PK, CancellationToken)"/>
        /// </summary>
        public async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = new CancellationToken())
        {
            bool exists = await decoratedService.ExistsAsync(id, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{typeof(TEntity).Name}:Exists",
                    ExtraFields = new { Id = id, Exists = exists },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }

            return exists;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Retrieve(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, Expression{Func{T, object}}[])"/>
        /// </summary>
        public IEnumerable<TEntity> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            List<TEntity> retrieved = decoratedService.Retrieve(filter, pagingContext, sortCondition, includes).ToList();

            try
            {
                // Due to size constraints, only audit the number of objects retrieved rather than the objects themselves.
                var options = new AuditScopeOptions
                {
                    EventType = $"{typeof(TEntity).Name}:Retrieve+",
                    ExtraFields = new { Count = retrieved.Count() },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (var auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
                }
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }

            return retrieved;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// </summary>
        public TEntity Retrieve(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity retrieved = default;
            var isServiceException = false;

            try
            {
                using (var auditScope = AuditScope.Create($"{typeof(TEntity).Name}:Retrieve", () => retrieved))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";

                    try
                    {
                        retrieved = decoratedService.Retrieve(id, includes);
                    }
                    catch
                    {
                        isServiceException = true;
                        throw;
                    }
                }
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }

            return retrieved;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.RetrieveAsync(Expression{Func{T, bool}}, PagingContext, Func{IQueryable{T}, IOrderedQueryable{T}}, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public async Task<IEnumerable<TEntity>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = new CancellationToken(),
            params Expression<Func<TEntity, object>>[] includes)
        {
            IEnumerable<TEntity> retrieved = await decoratedService.RetrieveAsync(
                filter,
                pagingContext,
                sortCondition,
                cancellationToken,
                includes);
            AuditScope auditScope = null;

            try
            {
                // Due to size constraints, only audit the number of objects retrieved rather than the objects themselves.
                var options = new AuditScopeOptions
                {
                    EventType = $"{typeof(TEntity).Name}:Retrieve+",
                    ExtraFields = new { Count = retrieved.Count() },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";
            }
            catch
            {
                // TODO: Ignore and log the exception raised by the auditing framework.
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }

            return retrieved;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.RetrieveAsync(PK, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public async Task<TEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = new CancellationToken(),
            params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity retrieved = default;
            AuditScope auditScope = null;
            var isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(TEntity).Name}:Retrieve", () => retrieved);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";

                try
                {
                    retrieved = await decoratedService.RetrieveAsync(id, cancellationToken, includes);
                }
                catch
                {
                    isServiceException = true;
                    throw;
                }
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }

            return retrieved;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Update(T)"/>
        /// </summary>
        public void Update(TEntity item)
        {
            TEntity original = readOnlyRepository.Retrieve(item.Id);
            var isServiceException = false;

            try
            {
                using (var auditScope = AuditScope.Create($"{typeof(TEntity).Name}:Update", () => original))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";

                    try
                    {
                        decoratedService.Update(item);
                    }
                    catch
                    {
                        isServiceException = true;
                        throw;
                    }

                    // Assign the updated item back to the original. This allows Audit.NET to determine what was changed.
                    original = item;
                }
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = new CancellationToken())
        {
            AuditScope auditScope = null;
            TEntity original = await readOnlyRepository.RetrieveAsync(item.Id, cancellationToken);
            var isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(TEntity).Name}:Update", () => original);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(TEntity).FullName}";

                try
                {
                    await decoratedService.UpdateAsync(item, cancellationToken);
                }
                catch
                {
                    isServiceException = true;
                    throw;
                }

                // Assign the updated item back to the original. This allows Audit.NET to determine what was changed.
                // ReSharper disable once RedundantAssignment
                original = item;
            }
            catch
            {
                if (isServiceException)
                {
                    throw;
                }
                else
                {
                    // TODO: Ignore and log the exception raised by the auditing framework.
                }
            }
            finally
            {
                if (auditScope != null)
                {
                    await auditScope.DisposeAsync();
                }
            }
        }
    }
}