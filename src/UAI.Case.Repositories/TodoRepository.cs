
using Microsoft.AspNetCore.Http;
using NHibernate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAI.Case.Domain.Common;

namespace UAI.Case.Repositories
{
    public interface ITodoRepository : IRepository<Todo>
    {
         long  GetPendientes(Guid IdUsuario);
    }
    public class TodoRepository : Repository<Todo>, ITodoRepository
    {
        public TodoRepository(ISessionFactory sessionFactory, IHttpContextAccessor context) : base(sessionFactory, context) { }

        public long  GetPendientes(Guid IdUsuario)
        {

            try
            {
                String qry2 = "FROM AlumnoCursoGrupo ag, GrupoTodo gt "
            + "WHERE gt.Grupo.Id=ag.Grupo.Id AND ag.Alumno.Id = :id AND gt.Done = :done";



                //String strQry = "FROM GrupoTodo as todo WHERE todo.Done='false' AND todo.Grupo.Id  IN (SELECT Grupo.Id From AlumnoCursoGrupo ac WHERE ac.Alumno.Id = :id)";
                IQuery qry = Session.CreateQuery(qry2);

                qry.SetParameter("id", IdUsuario);
                qry.SetParameter("done", false);
                long res = qry.List().Count;
                return res;
            }
            catch (Exception)
            {

                return -1;
            }

            
        }
    }
}
