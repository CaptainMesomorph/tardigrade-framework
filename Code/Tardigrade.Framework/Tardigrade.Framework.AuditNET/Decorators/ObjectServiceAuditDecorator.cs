using Audit.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Persistence;
using Tardigrade.Framework.Services;
using Tardigrade.Framework.Services.Users;

// ReSharper disable AccessToModifiedClosure

namespace Tardigrade.Framework.AuditNET.Decorators
{
    /// <summary>
    /// Decorator class for auditing of the object service interface.
    /// </summary>
    /// <typeparam name="TEntity">Object type associated with the service operations.</typeparam>
    /// <typeparam name="TKey">Unique identifier type for the object type.</typeparam>
    public class ObjectServiceAuditDecorator<TEntity, TKey>
        : IObjectService<TEntity, TKey> where TEntity : IHasUniqueIdentifier<TKey>
    {
        /// <summary>
        /// Fully qualified name (including namespace) of the entity associated with this decorator.
        /// </summary>
        protected static readonly string EntityFullName = typeof(TEntity).FullName;

        /// <summary>
        /// Name of the entity associated with this decorator.
        /// </summary>
        protected static readonly string EntityName = typeof(TEntity).Name;

        /// <summary>
        /// Object service that is being decorated.
        /// </summary>
        protected readonly IObjectService<TEntity, TKey> DecoratedService;

        /// <summary>
        /// Logging service.
        /// </summary>
        protected readonly ILogger<ObjectServiceAuditDecorator<TEntity, TKey>> Logger;

        /// <summary>
        /// Read-only repository for data retrieval. Used by the Update operations to retrieve the entity prior to any
        /// changes for the purposes of auditing.
        /// </summary>
        protected readonly IReadOnlyRepository<TEntity, TKey> ReadOnlyRepository;

        /// <summary>
        /// Contextual information on the current user.
        /// </summary>
        protected readonly IUserContext UserContext;

        /// <summary>
        /// Create an instance of this Decorator.
        /// </summary>
        /// <param name="decoratedService">Object service that is being decorated.</param>
        /// <param name="readOnlyRepository">Read-only repository for data retrieval.</param>
        /// <param name="userContext">Contextual information on the current user.</param>
        /// <param name="logger">Logging service (optional).</param>
        /// <exception cref="ArgumentNullException">A mandatory parameter is null.</exception>
        public ObjectServiceAuditDecorator(
            IObjectService<TEntity, TKey> decoratedService,
            IReadOnlyRepository<TEntity, TKey> readOnlyRepository,
            IUserContext userContext,
            ILogger<ObjectServiceAuditDecorator<TEntity, TKey>> logger = null)
        {
            DecoratedService = decoratedService ?? throw new ArgumentNullException(nameof(decoratedService));
            ReadOnlyRepository = readOnlyRepository ?? throw new ArgumentNullException(nameof(readOnlyRepository));
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            Logger = logger;
        }

        /// <inheritdoc />
        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            int count = DecoratedService.Count(filter);

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{EntityName}:Count",
                    ExtraFields = new { Count = count },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (var auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{EntityFullName}";
                }
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for Count of type {Entity}.", EntityName);
            }

            return count;
        }

        /// <inheritdoc />
        public virtual async Task<int> CountAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default)
        {
            int count = await DecoratedService.CountAsync(filter, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{EntityName}:Count",
                    ExtraFields = new { Count = count },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{EntityFullName}";
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for CountAsync of type {Entity}.", EntityName);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }

            return count;
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> Create(IEnumerable<TEntity> items)
        {
            IEnumerable<TEntity> created = default;
            var isServiceException = false;

            try
            {
                using (var auditScope = AuditScope.Create($"{EntityName}:Create+", () => created))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{items.GetType()}";

                    try
                    {
                        created = DecoratedService.Create(items);
                    }
                    catch
                    {
                        isServiceException = true;
                        auditScope.Discard();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for Create+ of type {Entity}.", EntityName);
            }

            return created;
        }

        /// <inheritdoc />
        public virtual TEntity Create(TEntity item)
        {
            TEntity created = default;
            var isServiceException = false;

            try
            {
                using (var auditScope = AuditScope.Create($"{EntityName}:Create", () => created))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{EntityFullName}";

                    try
                    {
                        created = DecoratedService.Create(item);
                    }
                    catch
                    {
                        isServiceException = true;
                        auditScope.Discard();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for Create of type {Entity}.", EntityName);
            }

            return created;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> CreateAsync(
            IEnumerable<TEntity> items,
            CancellationToken cancellationToken = default)
        {
            AuditScope auditScope = null;
            IEnumerable<TEntity> created = default;
            var isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{EntityName}:Create+", () => created);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{items.GetType()}";

                try
                {
                    created = await DecoratedService.CreateAsync(items, cancellationToken);
                }
                catch
                {
                    isServiceException = true;
                    auditScope.Discard();
                    throw;
                }
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for CreateAsync+ of type {Entity}.", EntityName);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }

            return created;
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> CreateAsync(
            TEntity item,
            CancellationToken cancellationToken = default)
        {
            TEntity created = default;
            AuditScope auditScope = null;
            var isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{EntityName}:Create", () => created);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{EntityFullName}";

                try
                {
                    created = await DecoratedService.CreateAsync(item, cancellationToken);
                }
                catch
                {
                    isServiceException = true;
                    auditScope.Discard();
                    throw;
                }
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for CreateAsync of type {Entity}.", EntityName);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }

            return created;
        }

        /// <inheritdoc />
        public virtual void Delete(TEntity item)
        {
            TKey id = item.Id;
            DecoratedService.Delete(item);

            try
            {
                using (var auditScope = AuditScope.Create($"{EntityName}:Delete", () => item))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{EntityFullName}";

                    // Set the deleted item to null. This prevents Audit.NET from replicating the audit details of the
                    // "Old" object into "New".
                    item = default;
                }
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for Delete of type {Entity} with ID {Id}.", EntityName, id);
            }
        }

        /// <inheritdoc />
        public virtual async Task DeleteAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            AuditScope auditScope = null;
            TKey id = item.Id;
            await DecoratedService.DeleteAsync(item, cancellationToken);

            try
            {
                auditScope = await AuditScope.CreateAsync($"{EntityName}:Delete", () => item);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{EntityFullName}";

                // Set the deleted item to null. This prevents Audit.NET from replicating the audit details of the
                // "Old" object into "New".
                // ReSharper disable once RedundantAssignment
                item = default;
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for DeleteAsync of type {Entity} with ID {Id}.", EntityName, id);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }
        }

        /// <inheritdoc />
        public virtual bool Exists(TKey id)
        {
            bool exists = DecoratedService.Exists(id);

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{EntityName}:Exists",
                    ExtraFields = new { Id = id, Exists = exists },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (var auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{EntityFullName}";
                }
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for Exists of type {Entity} with ID {Id}.", EntityName, id);
            }

            return exists;
        }

        /// <inheritdoc />
        public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken = default)
        {
            bool exists = await DecoratedService.ExistsAsync(id, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                var options = new AuditScopeOptions
                {
                    EventType = $"{EntityName}:Exists",
                    ExtraFields = new { Id = id, Exists = exists },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{EntityFullName}";
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for ExistsAsync of type {Entity} with ID {Id}.", EntityName, id);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }

            return exists;
        }

        /// <inheritdoc />
        public virtual IEnumerable<TEntity> Retrieve(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IEnumerable<TEntity> retrieved =
                DecoratedService.Retrieve(filter, pagingContext, sortCondition, includes).ToList();

            try
            {
                // Due to size constraints, only audit the number of objects retrieved rather than the objects themselves.
                var options = new AuditScopeOptions
                {
                    EventType = $"{EntityName}:Retrieve+",
                    ExtraFields = new { Count = retrieved.Count() },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                using (var auditScope = AuditScope.Create(options))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{EntityFullName}";
                }
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for Retrieve+ of type {Entity}.", EntityName);
            }

            return retrieved;
        }

        /// <inheritdoc />
        public virtual TEntity Retrieve(TKey id, params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity retrieved = default;
            var isServiceException = false;

            try
            {
                using (var auditScope = AuditScope.Create($"{EntityName}:Retrieve", () => retrieved))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{EntityFullName}";

                    try
                    {
                        retrieved = DecoratedService.Retrieve(id, includes);
                    }
                    catch
                    {
                        isServiceException = true;
                        auditScope.Discard();
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for Retrieve of type {Entity} with ID {Id}.", EntityName, id);
            }

            return retrieved;
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> RetrieveAsync(
            Expression<Func<TEntity, bool>> filter = null,
            PagingContext pagingContext = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IEnumerable<TEntity> entities = await DecoratedService.RetrieveAsync(
                filter,
                pagingContext,
                sortCondition,
                cancellationToken,
                includes);
            IEnumerable<TEntity> retrieved = entities.ToList();
            AuditScope auditScope = null;

            try
            {
                // Due to size constraints, only audit the number of objects retrieved rather than the objects
                // themselves.
                var options = new AuditScopeOptions
                {
                    EventType = $"{EntityName}:Retrieve+",
                    ExtraFields = new { Count = retrieved.Count() },
                    AuditEvent = new AuditEvent { Target = new AuditTarget() }
                };

                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{EntityFullName}";
            }
            catch (Exception e)
            {
                Logger.Warning(e, "Auditing failed for RetrieveAsync+ of type {Entity}.", EntityName);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }

            return retrieved;
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> RetrieveAsync(
            TKey id,
            CancellationToken cancellationToken = default,
            params Expression<Func<TEntity, object>>[] includes)
        {
            TEntity retrieved = default;
            AuditScope auditScope = null;
            var isServiceException = false;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{EntityName}:Retrieve", () => retrieved);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{EntityFullName}";

                try
                {
                    retrieved = await DecoratedService.RetrieveAsync(id, cancellationToken, includes);
                }
                catch
                {
                    isServiceException = true;
                    auditScope.Discard();
                    throw;
                }
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for RetrieveAsync of type {Entity} with ID {Id}.", EntityName, id);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }

            return retrieved;
        }

        /// <inheritdoc />
        public virtual void Update(TEntity item)
        {
            var isServiceException = false;
            TKey id = item.Id;
            TEntity original = ReadOnlyRepository.Retrieve(item.Id);

            try
            {
                using (var auditScope = AuditScope.Create($"{EntityName}:Update", () => original))
                {
                    auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                    auditScope.Event.Target.Type = $"{EntityFullName}";

                    try
                    {
                        DecoratedService.Update(item);
                    }
                    catch
                    {
                        isServiceException = true;
                        auditScope.Discard();
                        throw;
                    }

                    // Assign the updated item back to the original. This allows Audit.NET to determine what was
                    // changed.
                    original = item;
                }
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for Update of type {Entity} with ID {Id}.", EntityName, id);
            }
        }

        /// <inheritdoc />
        public virtual async Task UpdateAsync(TEntity item, CancellationToken cancellationToken = default)
        {
            AuditScope auditScope = null;
            var isServiceException = false;
            TKey id = item.Id;
            TEntity original = await ReadOnlyRepository.RetrieveAsync(id, cancellationToken);

            try
            {
                auditScope = await AuditScope.CreateAsync($"{EntityName}:Update", () => original);
                auditScope.Event.Environment.UserName = UserContext.CurrentUser;
                auditScope.Event.Target.Type = $"{EntityFullName}";

                try
                {
                    await DecoratedService.UpdateAsync(item, cancellationToken);
                }
                catch
                {
                    isServiceException = true;
                    auditScope.Discard();
                    throw;
                }

                // Assign the updated item back to the original. This allows Audit.NET to determine what was changed.
                // ReSharper disable once RedundantAssignment
                original = item;
            }
            catch (Exception e)
            {
                if (isServiceException) throw;

                Logger.Warning(e, "Auditing failed for UpdateAsync of type {Entity} with ID {Id}.", EntityName, id);
            }
            finally
            {
                if (auditScope != null) await auditScope.DisposeAsync();
            }
        }
    }
}