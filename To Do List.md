# tardigrade-framework
Framework for supporting coding best practices.


## To do list

- Update Tardigrade.Framework.EntityFrameworkCore.Repository<TEntity, TKey>.UpdateAsync(TEntity, CancellationToken) to cater for DbUpdateConcurrencyException.
- Update Tardigrade.Framework.Persistence.IReadOnlyRepository<TEntity, in TKey> to include Exists methods that use a filter condition, i.e. Exists(Expression<Func<TEntity, bool>>) and CountAsync(Expression<Func<TEntity, bool>>, CancellationToken).
- For security reasons, handle all unexpected exceptions raised from the repository to return RepositoryException. This will help prevent table names being exposed if details of the base exception are not automaticaly referenced by calling code.
- Create a new Tardigrade.Framework.IdentityAspNet project. Move any identity-related classes to this project from the AspNet project.
