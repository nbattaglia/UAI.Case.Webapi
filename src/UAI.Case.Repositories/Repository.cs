
using System;
using System.Linq;
using System.Linq.Expressions;
using UAI.Case.Domain;
using UAI.Case.NHibernateProvider;
using NHibernate;
using NHibernate.Linq;
using Microsoft.AspNetCore.Http;

using UAI.Case.Domain.Common;
using UAI.Case.Domain.Interfaces;

using System.Text;
using UAI.Case.Security;

namespace UAI.Case.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {


        private IAuthenticatedData _authenticatedData = null;
        public IAuthenticatedData AuthenticatedData
        {
            get
            {
                return _authenticatedData;
            }
            set
            {
                _authenticatedData = value;
            }
        }

        private readonly ISessionFactory _sessionFactory;
        IHttpContextAccessor _context;
        public Repository(ISessionFactory sessionFactory, IHttpContextAccessor context)
        {
            if (sessionFactory == null) throw new ArgumentNullException("sessionFactory may not be null");
            _sessionFactory = sessionFactory;
            _context = context;


        }




        protected virtual NHibernate.ISession Session
        {
            get
            {
                //TODO: cambiar esto por WebSessionContext


                return (NHibernate.ISession)_context.HttpContext.Items["NHibernateCurrentSessionFactory"];


            }
        }

        public IDbContext DbContext
        {
            get
            {
                return new DbContext(_sessionFactory);
            }
        }

        public void Delete(T entity)
        {


            //TODO: pasar esto a un NH event listener y ver como se puede poner un defautl where deleted=0
            entity.FechaEliminacion = DateTime.Now;
            Session.Delete(entity);
            Session.Flush();
        }


        public void Evict(T entity)
        {
            Session.Evict(entity);
            
        }

        public T Get(object id)
        {
            
            T o = Session.Get<T>(id);
            Session.Evict(o);
            return o;
        }

        public T Get(object id, params Expression<Func<T, object>>[] fetchPaths)
        {
            
            var query = Session.QueryOver<T>().Where(_ => _.Id==Guid.Parse(id.ToString()));
            foreach (var fetchPath in fetchPaths)
                query = query.Fetch(fetchPath).Eager;
            //query.CacheMode(CacheMode.Refresh);
            return query.SingleOrDefault();
        }

        public IQueryable<T> GetAll()
        {
            return Session.Query<T>();//.Where(t=>!(t.FechaEliminacion.Equals(null)));
        }

        //esto lo metemos aca y despues lo metemos en troo lado
        public static MemberExpression GetMemberExpression(Expression expression)
        {
            if (expression is MemberExpression)
            {
                return (MemberExpression)expression;
            }
            else if (expression is LambdaExpression)
            {
                var lambdaExpression = expression as LambdaExpression;
                if (lambdaExpression.Body is MemberExpression)
                {
                    return (MemberExpression)lambdaExpression.Body;
                }
                else if (lambdaExpression.Body is UnaryExpression)
                {
                    return ((MemberExpression)((UnaryExpression)lambdaExpression.Body).Operand);
                }
            }
            return null;
        }

        public static string GetPropertyPath(Expression expr)
        {
            var path = new StringBuilder();
            MemberExpression memberExpression = GetMemberExpression(expr);
            do
            {
                if (path.Length > 0)
                {
                    path.Insert(0, ".");
                }
                path.Insert(0, memberExpression.Member.Name);
                memberExpression = GetMemberExpression(memberExpression.Expression);
            }
            while (memberExpression != null);
            return path.ToString();
        }

        public static object GetPropertyValue(object obj, string propertyPath)
        {
            object propertyValue = null;
            if (propertyPath.IndexOf(".") < 0)
            {
                var objType = obj.GetType();
                propertyValue = objType.GetProperty(propertyPath).GetValue(obj, null);
                return propertyValue;
            }
            var properties = propertyPath.Split('.').ToList();
            var midPropertyValue = obj;
            while (properties.Count > 0)
            {
                var propertyName = properties.First();
                properties.Remove(propertyName);
                propertyValue = GetPropertyValue(midPropertyValue, propertyName);
                midPropertyValue = propertyValue;
            }
            return propertyValue;
        }


        public IQueryable<T> GetAll(params Expression<Func<T, object>>[] fetchPaths)
        {
           

            var queryable = Session.Query<T>();

            foreach (var fetchPath in fetchPaths)
            {
                queryable.Fetch(fetchPath);

                var fetchPathString = GetPropertyPath(fetchPath);

                if (!fetchPathString.Contains("."))
                    queryable = queryable.Fetch(fetchPath);
                else
                {

                    
                    int counter = 0;

                    Type tAnt=null;
                    foreach (var item in fetchPathString.Split('.'))
                    {
                        if(counter == 0)
                        {
                            var expParam = Expression.Parameter(typeof(T), "_");
                            var expField = Expression.Property(expParam, item);

                            var finalExp = Expression.Lambda<Func<T, object>>(expField, new ParameterExpression[] { expParam });

                            queryable = queryable.Fetch(finalExp);
                            tAnt = finalExp.Body.Type;

                        }
                            
                        else
                        {
                            var expParam = Expression.Parameter(tAnt, "_");
                            var expField = Expression.Property(expParam, item);

                            object[] parameters = new object[2];
                            parameters[0] = expField;
                            parameters[1] = new ParameterExpression[] { expParam };

                            Type[] typeArgs = { tAnt, typeof(object) };

                            var finalExp = (Expression<Func<object, object>>)
                                typeof(Expression)
                                .GetMethods().Where(_ => _.Name == "Lambda" && _.IsGenericMethod)
                                .First()
                                .MakeGenericMethod(typeof(Func<>).MakeGenericType(typeArgs))
                                .Invoke(null, parameters);
                            
                            //var finalExp = Expression.Lambda<Func<tAnt, object>>(expField, new ParameterExpression[] { expParam });

                            queryable = (queryable as INhFetchRequest<T, object>).ThenFetch(finalExp);
                            tAnt = finalExp.Body.Type;
                        }

                        counter++;
                    }

                }
            }

            
            return queryable;

        }

        public T Load(object id)
        {
            return Session.Load<T>(id);
        }

        public object Load(Type type, object id)
        {
            return Session.Load(type, id);
        }

        public Z Load<Z>(object id)
        {
            return Session.Load<Z>(id);
        }




        public T SaveOrUpdate(T entity)
        {

         
                if (entity.IsTransient())
            {

                entity.FechaCreacion = DateTime.Now;
            }

                if (entity is IAsignable && _authenticatedData!=null)
               
                    if (_authenticatedData.UsuarioId != null)
                    {
                                        
                    Usuario usuario = null;
                    
                    var asignable = ((IAsignable)entity);

                    if (asignable.Usuario == null)
                    {
                        usuario = Session.Get<Usuario>(_authenticatedData.UsuarioId);
                        
                        asignable.Usuario = usuario;
                    }

                    }


           
            Session.SaveOrUpdate(entity);
            



            return entity;
        }


    }
}
