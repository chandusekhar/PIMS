using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pims.Dal.Entities;
using Pims.Dal.Entities.Models;
using Pims.Dal.Helpers.Extensions;

namespace Pims.Dal.Services.Admin
{
    /// <summary>
    /// UserService class, provides a service layer to administrate users within the datasource.
    /// </summary>
    public class UserService : BaseService<User>, IUserService
    {
        #region Variables
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new instance of a UserService, and initializes it with the specified arguments.
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="user"></param>
        /// <param name="logger"></param>
        public UserService(PimsContext dbContext, ClaimsPrincipal user, ILogger<UserService> logger) : base(dbContext, user, logger) { }
        #endregion

        #region Methods
        /// <summary>
        /// Get a page of users from the datasource.
        /// The filter will allow queries to search for anything that starts with the following properties; DisplayName, FirstName, LastName, Email, Agencies.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public Paged<User> GetNoTracking(int page = 1, int quantity = 10)
        {
            return GetNoTracking(new UserFilter(page, quantity));
        }

        /// <summary>
        /// Get a page of users from the datasource.
        /// The filter will allow queries to search for the following property values; DisplayName, FirstName, LastName, Email, Agencies.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public Paged<User> GetNoTracking(UserFilter filter = null)
        {
            this.User.ThrowIfNotAuthorized("system-administrator");

            if (filter.Page < 1) filter.Page = 1;
            if (filter.Quantity < 1) filter.Quantity = 1;
            if (filter.Quantity > 50) filter.Quantity = 50;

            var query = this.Context.Users.AsNoTracking();
            if (filter != null)
            {
                if (!string.IsNullOrWhiteSpace(filter.DisplayName))
                    query = query.Where(u => EF.Functions.Like(u.DisplayName, $"{filter.DisplayName}"));
                if (!string.IsNullOrWhiteSpace(filter.FirstName))
                    query = query.Where(u => EF.Functions.Like(u.FirstName, $"{filter.FirstName}"));
                if (!string.IsNullOrWhiteSpace(filter.LastName))
                    query = query.Where(u => EF.Functions.Like(u.LastName, $"{filter.LastName}"));
                if (!string.IsNullOrWhiteSpace(filter.Email))
                    query = query.Where(u => EF.Functions.Like(u.Email, $"{filter.Email}"));
                if (filter.Agencies?.Any() == true)
                    query = query.Where(u => u.Agencies.Any(a => filter.Agencies.Contains(a.AgencyId)));
                if (filter.Sort?.Any() == true)
                    query = query.OrderByProperty(filter.Sort);
            }
            var users = query.Skip((filter.Page - 1) * filter.Quantity).Take(filter.Quantity);
            return new Paged<User>(users.ToArray(), filter.Page, filter.Quantity, query.Count());
        }

        /// <summary>
        /// Get the user with the specified 'id'.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetNoTracking(Guid id)
        {
            this.User.ThrowIfNotAuthorized("system-administrator");

            return this.Context.Users.AsNoTracking().FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Updates the specified user in the datasource.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override User Update(User entity)
        {
            entity.ThrowIfNull(nameof(entity));

            var user = this.Context.Users.Find(entity.Id);
            if (user == null) throw new KeyNotFoundException();

            this.Context.Entry(user).CurrentValues.SetValues(entity);
            return base.Update(user);
        }

        /// <summary>
        /// Remove the specified user from the datasource.
        /// </summary>
        /// <param name="entity"></param>
        public override void Remove(User entity)
        {
            entity.ThrowIfNull(nameof(entity));

            var user = this.Context.Users.Find(entity.Id);
            if (user == null) throw new KeyNotFoundException();

            this.Context.Entry(user).CurrentValues.SetValues(entity);
            base.Remove(user);
        }

        /// <summary>
        /// Get all the access requests that users have submitted to the system
        /// </summary>
        /// <param name="page"></param>
        /// <param name="quantity"></param>
        /// <param name="sort"></param>
        /// <param name="isGranted"></param>
        public Paged<AccessRequest> GetAccessRequestsNoTracking(int page = 1, int quantity = 10, string sort = null, bool? isGranted = null)
        {
            var query = this.Context.AccessRequests
                .Include(p => p.Agencies)
                .ThenInclude(p => p.Agency)
                .Include(p => p.Roles)
                .ThenInclude(p => p.Role)
                .Include(p => p.User)
                .AsNoTracking();

            if (isGranted.HasValue)
            {
                query = query.Where(request => request.IsGranted == isGranted);
            }
            var accessRequests = query.Skip((page - 1) * quantity).Take(quantity);
            return new Paged<AccessRequest>(accessRequests, page, quantity, query.Count());
        }
        #endregion
    }
}