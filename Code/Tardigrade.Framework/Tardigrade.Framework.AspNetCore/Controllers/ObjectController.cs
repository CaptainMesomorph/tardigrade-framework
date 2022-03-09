using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tardigrade.Framework.AspNetCore.Extensions;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Domain;
using Tardigrade.Framework.Models.Persistence;
using Tardigrade.Framework.Services;

namespace Tardigrade.Framework.AspNetCore.Controllers;

/// <summary>
/// REST API Controller based on a specific object type.
/// TODO Reflect new exceptions raised from service methods, e.g. AlreadyExistsException.
/// TODO Add status code 409 Conflict to the Post method once implemented in service layer.
/// </summary>
/// <typeparam name="TEntity">Object type associated with the API Controller.</typeparam>
/// <typeparam name="TKey">Unique identifier type for the object type.</typeparam>
[ApiController]
[ApiConventionType(typeof(DefaultApiConventions))]
[Route("api/[controller]")]
public abstract class ObjectController<TEntity, TKey> : ControllerBase where TEntity : IHasUniqueIdentifier<TKey>
{
    /// <summary>
    /// Object service associated with the Controller.
    /// </summary>
    protected IObjectService<TEntity, TKey> Service { get; }

    /// <summary>
    /// Create an instance of this API Controller.
    /// </summary>
    /// <param name="service">Service associated with the object type.</param>
    /// <exception cref="ArgumentNullException">service is null.</exception>
    protected ObjectController(IObjectService<TEntity, TKey> service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    /// <summary>
    /// DELETE: api/[controller]/{id}
    /// 200 OK
    /// 400 Bad Request
    /// 404 Not Found
    /// </summary>
    /// <param name="id">Unique identifier of the object to delete.</param>
    /// <returns>Result of the delete action.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete(TKey id)
    {
        ActionResult result;

        try
        {
            TEntity model = await Service.RetrieveAsync(id);

            if (model == null)
            {
                result = NotFound();
            }
            else
            {
                await Service.DeleteAsync(model);
                result = Ok();
            }
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }

        return result;
    }

    /// <summary>
    /// GET: api/[controller]
    /// 200 OK
    /// 204 No Content
    /// 400 Bad Request
    /// </summary>
    /// <returns>Collection of objects.</returns>
    [HttpGet]
    [ProducesDefaultResponseType]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public virtual async Task<ActionResult<IEnumerable<TEntity>>> Get(
        uint? pageSize = null,
        uint? pageIndex = 0,
        string sortBy = null)
    {
        PagingContext pagingContext = null;
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> sortCondition = null;

        if (pageSize.HasValue)
        {
            pagingContext = new PagingContext { PageIndex = pageIndex ?? 0, PageSize = pageSize.Value };

            if (string.IsNullOrWhiteSpace(sortBy))
            {
                sortCondition = (q => q.OrderBy(o => o.Id));
            }
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            sortCondition = (q => q.OrderBy(sortBy));
        }

        ActionResult<IEnumerable<TEntity>> result;

        try
        {
            IEnumerable<TEntity> retrieved = await Service.RetrieveAsync(pagingContext: pagingContext, sortCondition: sortCondition);
            List<TEntity> models = retrieved.ToList();

            if (!models.Any())
            {
                result = NoContent();
            }
            else
            {
                result = models.ToList();
            }
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }

        return result;
    }

    /// <summary>
    /// GET: api/[controller]/{id}
    /// 200 OK
    /// 404 Not Found
    /// </summary>
    /// <param name="id">Unique identifier of the object to retrieve.</param>
    /// <returns>Object with a matching unique identifier.</returns>
    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TEntity>> Get(TKey id)
    {
        ActionResult<TEntity> result;

        try
        {
            TEntity model = await Service.RetrieveAsync(id);

            if (model == null)
            {
                result = NotFound();
            }
            else
            {
                result = model;
            }
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }

        return result;
    }

    /// <summary>
    /// POST: api/[controller]
    /// 201 Created
    /// 400 Bad Request
    /// </summary>
    /// <param name="model">Object to create.</param>
    /// <returns>Object created (including allocated unique identifier).</returns>
    [HttpPost]
    public virtual async Task<ActionResult<TEntity>> Post(TEntity model)
    {
        if (model == null)
        {
            return this.BadRequest(message: "Object to create was not provided.");
        }

        ActionResult result;

        try
        {
            TEntity createdObj = await Service.CreateAsync(model);
            result = CreatedAtAction(nameof(Get), new { id = createdObj.Id }, createdObj);
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }
        catch (ValidationException e)
        {
            result = this.BadRequest(message: e.GetBaseException().Message);
        }

        return result;
    }

    //
    /// <summary>
    /// PUT: api/[controller]/{id}
    /// 204 No Content
    /// 400 Bad Request
    /// 404 Not Found
    /// </summary>
    /// <param name="id">Unique identifier of the object to update.</param>
    /// <param name="model">Object to update.</param>
    /// <returns>Result of the update action.</returns>
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Put(TKey id, TEntity model)
    {
        if (model == null)
        {
            return this.BadRequest(message: "Object to update was not provided.");
        }
        else if (!id.Equals(model.Id))
        {
            return this.BadRequest(message: "Unique identifier provided does not match that of the object.");
        }

        IActionResult result;

        try
        {
            await Service.UpdateAsync(model);
            result = NoContent();
        }
        catch (NotFoundException)
        {
            result = NotFound();
        }
        catch (ServiceException e)
        {
            result = this.StatusCode(StatusCodes.Status500InternalServerError, message: e.GetBaseException().Message);
        }
        catch (ValidationException e)
        {
            result = this.BadRequest(message: e.GetBaseException().Message);
        }

        return result;
    }
}