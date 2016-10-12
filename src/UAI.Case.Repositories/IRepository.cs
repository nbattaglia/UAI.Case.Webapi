using System;
using System.Linq;
using System.Linq.Expressions;
using UAI.Case.NHibernateProvider;

namespace UAI.Case.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Provides a handle to application wide DB activities such as committing any pending changes,
        /// beginning a transaction, rolling back a transaction, etc.
        /// </summary>
        IDbContext DbContext { get; }

        /// <summary>
        /// Returns null if a row is not found matching the provided Id.
        /// </summary>
        T Get(object id);
        T Get(object id, params Expression<Func<T, Object>>[] fetchPaths);

        /// <summary>
        /// Returns a NH Proxy, doesnt hit database, throws ex if obj not found
        /// </summary>
        T Load(object id);
        object Load(Type type, object id);
        Z Load<Z>(object id);

        IQueryable<T> GetAll();
        IQueryable<T> GetAll(params Expression<Func<T, Object>>[] fetchPaths);

        /// <summary>
        /// For entities with automatically generated Ids, such as identity or Hi/Lo, SaveOrUpdate may 
        /// be called when saving or updating an entity.  If you require separate Save and Update
        /// methods, you'll need to extend the base repository interface when using NHibernate.
        /// 
        /// Updating also allows you to commit changes to a detached object.  More info may be found at:
        /// http://www.hibernate.org/hib_docs/nhibernate/html_single/#manipulatingdata-updating-detached
        /// </summary>
        T SaveOrUpdate(T entity);

        /// <summary>
        /// I'll let you guess what this does.
        /// </summary>
        /// <remarks>The SharpLite.NHibernateProvider.Repository commits the deletion immediately; 
        /// see that class for details.</remarks>
        void Delete(T entity);
        void Evict(T entity);
    }
}