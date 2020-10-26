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
    /// <typeparam name="T">Object type associated with the service operations.</typeparam>
    /// <typeparam name="Pk">Unique identifier type for the object type.</typeparam>
    public class ObjectServiceAuditDecorator<T, Pk> : IObjectService<T, Pk> where T : IHasUniqueIdentifier<Pk>
    {
        private readonly IObjectService<T, Pk> decoratedService;
        private readonly IReadOnlyRepository<T, Pk> readOnlyRepository;
        private readonly IUserContext userContext;

        /// <summary>
        /// Create an instance of this Decorator.
        /// </summary>
        /// <param name="decoratedService">Object service that is being decorated.</param>
        /// <param name="readOnlyRepository">Read-only repository for data retrieval.</param>
        /// <param name="userContext">Contextual information on the current user.</param>
        /// <exception cref="ArgumentNullException">A parameter is null.</exception>
        public ObjectServiceAuditDecorator(
            IObjectService<T, Pk> decoratedService,
            IReadOnlyRepository<T, Pk> readOnlyRepository,
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
        public int Count(Expression<Func<T, bool>> filter = null)
        {
            int count = decoratedService.Count(filter);

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{typeof(T).Name}:Count",
                    ExtraFields = new { Count = count },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (AuditScope auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
            Expression<Func<T, bool>> filter = null,
            CancellationToken cancellationToken = new CancellationToken())
        {
            int count = await decoratedService.CountAsync(filter, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                AuditScopeOptions options = new AuditScopeOptions
                {
                    EventType = $"{typeof(T).Name}:Count",
                    ExtraFields = new { Count = count },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
        public IEnumerable<T> Create(IEnumerable<T> items)
        {
            IEnumerable<T> created = default;
            bool isServiceException = false;

            try
            {
                using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Create+", () => created))
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
        public T Create(T item)
        {
            T created = default;
            bool isServiceException = false;

            try
            {
                using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Create", () => created))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(T).FullName}";

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
        public async Task<IEnumerable<T>> CreateAsync(
            IEnumerable<T> items,
            CancellationToken cancellationToken = new CancellationToken())
        {
            IEnumerable<T> created = default;
            AuditScope auditScope = null;
            bool isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Create+", () => created);
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
        public async Task<T> CreateAsync(T item, CancellationToken cancellationToken = new CancellationToken())
        {
            T created = default;
            AuditScope auditScope = null;
            bool isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Create", () => created);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";

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
        public void Delete(T item)
        {
            decoratedService.Delete(item);

            try
            {
                using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Delete", () => item))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
        public async Task DeleteAsync(T item, CancellationToken cancellationToken = new CancellationToken())
        {
            await decoratedService.DeleteAsync(item, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Delete", () => item);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
        public bool Exists(Pk id)
        {
            bool exists = decoratedService.Exists(id);

            try
            {
                AuditScopeOptions options = new AuditScopeOptions
                {
                    EventType = $"{typeof(T).Name}:Exists",
                    ExtraFields = new { Id = id, Exists = exists },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (AuditScope auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
        public async Task<bool> ExistsAsync(Pk id, CancellationToken cancellationToken = new CancellationToken())
        {
            bool exists = await decoratedService.ExistsAsync(id, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                AuditScopeOptions options = new AuditScopeOptions
                {
                    EventType = $"{typeof(T).Name}:Exists",
                    ExtraFields = new { Id = id, Exists = exists },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
        public IEnumerable<T> Retrieve(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            params Expression<Func<T, object>>[] includes)
        {
            List<T> retrieved = decoratedService.Retrieve(filter, pagingContext, sortCondition, includes).ToList();

            try
            {
                // Due to size constraints, only audit the number of objects retrieved rather than the objects themselves.
                AuditScopeOptions options = new AuditScopeOptions
                {
                    EventType = $"{typeof(T).Name}:Retrieve+",
                    ExtraFields = new { Count = retrieved.Count() },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (AuditScope auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
        public T Retrieve(Pk id, params Expression<Func<T, object>>[] includes)
        {
            T retrieved = default;
            bool isServiceException = false;

            try
            {
                using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Retrieve", () => retrieved))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(T).FullName}";

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
        public async Task<IEnumerable<T>> RetrieveAsync(
            Expression<Func<T, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> sortCondition = null,
            CancellationToken cancellationToken = new CancellationToken(),
            params Expression<Func<T, object>>[] includes)
        {
            IEnumerable<T> retrieved = await decoratedService.RetrieveAsync(
                filter,
                pagingContext,
                sortCondition,
                cancellationToken,
                includes);
            AuditScope auditScope = null;

            try
            {
                // Due to size constraints, only audit the number of objects retrieved rather than the objects themselves.
                AuditScopeOptions options = new AuditScopeOptions
                {
                    EventType = $"{typeof(T).Name}:Retrieve+",
                    ExtraFields = new { Count = retrieved.Count() },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
        public async Task<T> RetrieveAsync(
            Pk id,
            CancellationToken cancellationToken = new CancellationToken(),
            params Expression<Func<T, object>>[] includes)
        {
            T retrieved = default;
            AuditScope auditScope = null;
            bool isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Retrieve", () => retrieved);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";

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
        public void Update(T item)
        {
            T original = readOnlyRepository.Retrieve(item.Id);
            bool isServiceException = false;

            try
            {
                using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Update", () => original))
                {
                    auditScope.Event.Environment.UserName = userContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{typeof(T).FullName}";

                    try
                    {
                        decoratedService.Update(item);
                    }
                    catch
                    {
                        isServiceException = true;
                        throw;
                    }

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
        public async Task UpdateAsync(T item, CancellationToken cancellationToken = new CancellationToken())
        {
            AuditScope auditScope = null;
            T original = await readOnlyRepository.RetrieveAsync(item.Id);
            bool isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Update", () => original);
                auditScope.Event.Environment.UserName = userContext.CurrentUser;
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";

                try
                {
                    await decoratedService.UpdateAsync(item, cancellationToken);
                }
                catch
                {
                    isServiceException = true;
                    throw;
                }

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