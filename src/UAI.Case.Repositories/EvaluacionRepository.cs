using Microsoft.AspNetCore.Http;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UAI.Case.Domain.CASE;

namespace UAI.Case.Repositories
{
  
        public interface IEvaluacionRepository : IRepository<Evaluacion>
        {
            IList<Evaluacion> GetEvaluacionesConRespuestas(Guid ModeloId);
        }
        public class EvaluacionRepository : Repository<Evaluacion>, IEvaluacionRepository
        {
            public EvaluacionRepository(ISessionFactory sessionFactory, IHttpContextAccessor context) : base(sessionFactory, context) { }

            public IList<Evaluacion> GetEvaluacionesConRespuestas(Guid ModeloId)
            {

                try
                {

                //TODO: Pendiente
                    String qry2 = "FROM Evaluacion e JOIN RespuestaEvaluacion re ON e.ModeloId== WHER E  e.ModeloId= :id";



                    //String strQry = "FROM GrupoTodo as todo WHERE todo.Done='false' AND todo.Grupo.Id  IN (SELECT Grupo.Id From AlumnoCursoGrupo ac WHERE ac.Alumno.Id = :id)";
                    IQuery qry = Session.CreateQuery(qry2);

                    qry.SetParameter("id",ModeloId);

                IList<Evaluacion> res = qry.List<Evaluacion>();
                    return res;
                }
                catch (Exception e)
                {

                    return null;
                }


            }
        }
    
}
