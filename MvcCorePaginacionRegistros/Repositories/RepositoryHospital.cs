using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

namespace MvcCorePaginacionRegistros.Repositories
{
    #region storedview
    /*
     CREATE VIEW V_DEPARTAMENTOS_INDIVIDUAL
as
	select ROW_NUMBER() over (order by DEPT_NO) as POSICION,DEPT_NO,DNOMBRE,LOC FROM DEPT
go

select *  FROM V_DEPARTAMENTOS_INDIVIDUAL WHERE POSICION=1

    alter VIEW V_DEPARTAMENTOS_INDIVIDUAL
as
	select cast(ROW_NUMBER() over (order by DEPT_NO)as int )as POSICION,DEPT_NO,DNOMBRE,LOC FROM DEPT
go
     */
    #endregion
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }
        //PARA PODER REALIZAR LA PAGINACIÓN NECESITAREMOS LA VISTA PARA EL FILTER

        public async Task<int> GetNumerosRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamentos.CountAsync();
        }
        public async Task<VistaDepartamento>GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento departamento = 
                await this.context.VistaDepartamentos.Where(z => z.Posicion == posicion).FirstOrDefaultAsync();
            return departamento;
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamento(int posicion)
        {
            //select *  FROM V_DEPARTAMENTOS_INDIVIDUAL WHERE POSICION>= 1 and POSICION<(1+2)
            var consulta = from datos in this.context.VistaDepartamentos
                           where datos.Posicion >= posicion && datos.Posicion < (posicion + 2)
                           select datos;
            
            return await consulta.ToListAsync();
        }




    }
}
