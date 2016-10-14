using UAI.Case.EFProvider;
using System;
using System.Linq;
using System.Linq.Expressions;
using UAI.Case.Domain;
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

       // private readonly ISessionFactory _sessionFactory;
        IHttpContextAccessor _context;
        IUnitOfWork _unitOfWork;
        public Repository(IHttpContextAccessor context, IUnitOfWork unitOfWork)
        {
            
            _unitOfWork= unitOfWork;
            _context = context;


        }
        
        public void Delete(T entity)
        {


            //TODO: pasar esto a un NH event listener y ver como se puede poner un defautl where deleted=0
            entity.FechaEliminacion = DateTime.Now;
            _unitOfWork.Set<T>().Remove(entity);
            
        }



        public T Get(object id)
        {
            var o = _unitOfWork.Set<T>().Where(p=>p.Id.Equals(id)).FirstOrDefault();
            return o;
        }

       
        public IQueryable<T> GetAll()
        {
            return _unitOfWork.Set<T>();
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
                        usuario = _unitOfWork.Set<Usuario>().Where(p=>p.Id.Equals(_authenticatedData.UsuarioId)).FirstOrDefault();
                        
                        asignable.Usuario = usuario;
                    }

                    }

            _unitOfWork.Set<T>().Add(entity);
            _unitOfWork.Commit();
            return entity;
        }


    }
}
