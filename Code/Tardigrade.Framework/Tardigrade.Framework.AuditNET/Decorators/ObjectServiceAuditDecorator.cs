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

namespace Tardigrade.Framework.AuditNET.Decorators
{
    /// <summary>
    /// Decorator class for auditing of the object service interface.
    /// </summary>
    /// <typeparam name="T">Object type associated with the service operations.</typeparam>
    /// <typeparam name="PK">Unique identifier type for the object type.</typeparam>
    public class ObjectServiceAuditDecorator<T, PK> : IObjectService<T, PK> where T : IHasUniqueIdentifier<PK>
    {
        private readonly IObjectService<T, PK> decoratee;
        private readonly IReadOnlyRepository<T, PK> readOnlyRepository;

        /// <summary>
        /// Create an instance of this Decorator.
        /// </summary>
        /// <param name="decoratee">Object service that is being decorated.</param>
        /// <param name="readOnlyRepository">Read-only repository for data retrieval.</param>
        public ObjectServiceAuditDecorator(
            IObjectService<T, PK> decoratee, IReadOnlyRepository<T, PK> readOnlyRepository)
        {
            this.decoratee = decoratee;
            this.readOnlyRepository = readOnlyRepository;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Count(Expression{Func{T, bool}})"/>
        /// </summary>
        public int Count(Expression<Func<T, bool>> filter = null)
        {
            int count = decoratee.Count(filter);

            AuditScopeOptions options = new AuditScopeOptions
            {
                EventType = $"{typeof(T).Name}:Count",
                ExtraFields = new { Count = count },
                AuditEvent = new AuditEvent { Target = new AuditTarget() }
            };

            using (AuditScope auditScope = AuditScope.Create(options))
            {
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
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
            int count = await decoratee.CountAsync(filter, cancellationToken);

            AuditScopeOptions options = new AuditScopeOptions
            {
                EventType = $"{typeof(T).Name}:Count",
                ExtraFields = new { Count = count },
                AuditEvent = new AuditEvent { Target = new AuditTarget() }
            };

            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
            }
            finally
            {
                await auditScope.DisposeAsync();
            }

            return count;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Create(IEnumerable{T})"/>
        /// </summary>
        public IEnumerable<T> Create(IEnumerable<T> objs)
        {
            IEnumerable<T> created = default(IEnumerable<T>);

            using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}+:Create", () => created))
            {
                auditScope.Event.Target.Type = $"{objs.GetType()}";
                created = decoratee.Create(objs);
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Create(T)"/>
        /// </summary>
        public T Create(T obj)
        {
            T created = default(T);

            using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Create", () => created))
            {
                auditScope.Event.Target.Type = $"{obj.GetType()}";
                created = decoratee.Create(obj);
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.CreateAsync(IEnumerable{T}, CancellationToken)"/>
        /// </summary>
        public async Task<IEnumerable<T>> CreateAsync(
            IEnumerable<T> objs,
            CancellationToken cancellationToken = new CancellationToken())
        {
            IEnumerable<T> created = default(IEnumerable<T>);
            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}+:Create", () => created);
                auditScope.Event.Target.Type = $"{objs.GetType()}";
                created = await decoratee.CreateAsync(objs, cancellationToken);
            }
            finally
            {
                await auditScope.DisposeAsync();
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.CreateAsync(T, CancellationToken)"/>
        /// </summary>
        public async Task<T> CreateAsync(T obj, CancellationToken cancellationToken = new CancellationToken())
        {
            T created = default(T);
            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Create", () => created);
                auditScope.Event.Target.Type = $"{obj.GetType()}";
                created = await decoratee.CreateAsync(obj, cancellationToken);
            }
            finally
            {
                await auditScope.DisposeAsync();
            }

            return created;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Delete(T)"/>
        /// </summary>
        public void Delete(T obj)
        {
            decoratee.Delete(obj);

            using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Delete", () => obj))
            {
                auditScope.Event.Target.Type = $"{obj.GetType()}";
                obj = default(T);
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.DeleteAsync(T, CancellationToken)"/>
        /// </summary>
        public async Task DeleteAsync(T obj, CancellationToken cancellationToken = new CancellationToken())
        {
            await decoratee.DeleteAsync(obj, cancellationToken);
            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Delete", () => obj);
                auditScope.Event.Target.Type = $"{obj.GetType()}";
                obj = default(T);
            }
            finally
            {
                await auditScope.DisposeAsync();
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Exists(PK)"/>
        /// </summary>
        public bool Exists(PK id)
        {
            bool exists = decoratee.Exists(id);

            AuditScopeOptions options = new AuditScopeOptions
            {
                EventType = $"{typeof(T).Name}:Exists",
                ExtraFields = new { Id = id, Exists = exists },
                AuditEvent = new AuditEvent { Target = new AuditTarget() }
            };

            using (AuditScope auditScope = AuditScope.Create(options))
            {
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
            }

            return exists;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.ExistsAsync(PK, CancellationToken)"/>
        /// </summary>
        public async Task<bool> ExistsAsync(PK id, CancellationToken cancellationToken = new CancellationToken())
        {
            bool exists = await decoratee.ExistsAsync(id, cancellationToken);

            AuditScopeOptions options = new AuditScopeOptions
            {
                EventType = $"{typeof(T).Name}:Exists",
                ExtraFields = new { Id = id, Exists = exists },
                AuditEvent = new AuditEvent { Target = new AuditTarget() }
            };

            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
            }
            finally
            {
                await auditScope.DisposeAsync();
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
            IEnumerable<T> retrieved = decoratee.Retrieve(filter, pagingContext, sortCondition, includes);

            // Due to size constraints, only audit the number of objects retrieved rather than the objects themselves.
            AuditScopeOptions options = new AuditScopeOptions
            {
                EventType = $"{typeof(T).Name}+:Retrieve",
                ExtraFields = new { Count = retrieved.Count() },
                AuditEvent = new AuditEvent { Target = new AuditTarget() }
            };

            using (AuditScope auditScope = AuditScope.Create(options))
            {
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
            }

            return retrieved;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Retrieve(PK, Expression{Func{T, object}}[])"/>
        /// </summary>
        public T Retrieve(PK id, params Expression<Func<T, object>>[] includes)
        {
            T retrieved = default(T);

            using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Retrieve", () => retrieved))
            {
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
                retrieved = decoratee.Retrieve(id, includes);
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
            IEnumerable<T> retrieved = await decoratee.RetrieveAsync(
                filter,
                pagingContext,
                sortCondition,
                cancellationToken,
                includes);

            // Due to size constraints, only audit the number of objects retrieved rather than the objects themselves.
            AuditScopeOptions options = new AuditScopeOptions
            {
                EventType = $"{typeof(T).Name}+:Retrieve",
                ExtraFields = new { Count = retrieved.Count() },
                AuditEvent = new AuditEvent { Target = new AuditTarget() }
            };

            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync(options);
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
            }
            finally
            {
                await auditScope.DisposeAsync();
            }

            return retrieved;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.RetrieveAsync(PK, CancellationToken, Expression{Func{T, object}}[])"/>
        /// </summary>
        public async Task<T> RetrieveAsync(
            PK id,
            CancellationToken cancellationToken = new CancellationToken(),
            params Expression<Func<T, object>>[] includes)
        {
            T retrieved = default(T);
            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Retrieve", () => retrieved);
                auditScope.Event.Target.Type = $"{typeof(T).FullName}";
                retrieved = await decoratee.RetrieveAsync(id, cancellationToken, includes);
            }
            finally
            {
                await auditScope.DisposeAsync();
            }

            return retrieved;
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.Update(T)"/>
        /// </summary>
        public void Update(T obj)
        {
            T original = readOnlyRepository.Retrieve(obj.Id);

            using (AuditScope auditScope = AuditScope.Create($"{typeof(T).Name}:Update", () => original))
            {
                auditScope.Event.Target.Type = $"{obj.GetType()}";
                decoratee.Update(obj);
                original = obj;
            }
        }

        /// <summary>
        /// <see cref="IObjectService{T, PK}.UpdateAsync(T, CancellationToken)"/>
        /// </summary>
        public async Task UpdateAsync(T obj, CancellationToken cancellationToken = new CancellationToken())
        {
            AuditScope auditScope = null;

            try
            {
                auditScope = await AuditScope.CreateAsync($"{typeof(T).Name}:Update", () => obj);
                auditScope.Event.Target.Type = $"{obj.GetType()}";
                await decoratee.UpdateAsync(obj);
            }
            finally
            {
                await auditScope.DisposeAsync();
            }
        }
    }
}