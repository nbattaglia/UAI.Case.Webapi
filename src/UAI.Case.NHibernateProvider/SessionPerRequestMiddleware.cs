using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using NHibernate;
using System.Web.Http;
using NHibernate.Context;

namespace UAI.Case.NHibernateProvider
{
    public class SessionPerRequestMiddleware
    {
        private const string CURRENT_SESSION_CONTEXT_KEY = "NHibernateCurrentSessionFactory";
        
        RequestDelegate _next;
        private readonly ISessionFactory _sessionFactory;
        

        public SessionPerRequestMiddleware(RequestDelegate next,ISessionFactory sessionFactory)
        {
            _next = next;
            _sessionFactory = sessionFactory;
        }



        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers["Cache-Control"]= "no-cache";

            
            

            try
            {
                
                 var session = _sessionFactory.OpenSession();

                //TODO: cambiar esto por WebSessionContext
                context.Items[CURRENT_SESSION_CONTEXT_KEY] = session;
                session.BeginTransaction();


                await _next(context);
                
                session = (NHibernate.ISession)context.Items[CURRENT_SESSION_CONTEXT_KEY];
                var transaction = session.Transaction;


                    transaction.Commit();
                EndSession(context);

            }

            catch (Exception e)
            {
                var session= (NHibernate.ISession)context.Items[CURRENT_SESSION_CONTEXT_KEY];
                context.Response.StatusCode = 500;
                var transaction = session.Transaction;
                
                transaction.Rollback();
                
                
                session = (NHibernate.ISession)context.Items[CURRENT_SESSION_CONTEXT_KEY];

                EndSession(context);
    
                throw e;
                
                
            }

            
        }

        static void EndSession(HttpContext context)
        {
            var session = (NHibernate.ISession)context.Items[CURRENT_SESSION_CONTEXT_KEY];
            
            //TODO: cambiar esto por WebSessionContext unbind
            context.Items[CURRENT_SESSION_CONTEXT_KEY] = null;
            session.Close();
        }
        

        private static NHibernate.ISession BeginSession(ISessionFactory sessionFactory)
        {

            var session = sessionFactory.OpenSession();
            session.BeginTransaction();
            return session;
        }

     
      
      


    }
}
