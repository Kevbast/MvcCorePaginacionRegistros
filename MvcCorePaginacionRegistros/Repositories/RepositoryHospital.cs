using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;

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

//STORED PROCEDURE
CREATE procedure SP_GRUPO_DEPARTAMENTOS 

(@posicion int) 

as 

select DEPT_NO, DNOMBRE, LOC from V_DEPARTAMENTOS_INDIVIDUAL 
where POSICION >= @posicion and POSICION < (@posicion + 2) 

go 

    //VISTA Y PRCEDIMIENTO PAGINACION 3 3 DE EMPLEADOS

    --creamos vista y procedure PARA EMPLEADO

CREATE VIEW V_EMPLEADOS_INDIVIDUAL
as
	select cast(ROW_NUMBER() over (order by APELLIDO)as int ) as POSICION,EMP_NO,APELLIDO,OFICIO,SALARIO,DEPT_NO FROM EMP
go
--procedure
CREATE procedure SP_GRUPO_EMPLEADOS 
(@posicion int) 

as 

select EMP_NO, APELLIDO, OFICIO,SALARIO,DEPT_NO from V_EMPLEADOS_INDIVIDUAL 
where POSICION >= @posicion and POSICION < (@posicion + 3) 

go 

exec SP_GRUPO_EMPLEADOS 1
    
  --STORED PROCEDURE PARA LOS OFICIOS PAGINACION 3 EN 3

--NO SE HACE SELECT * FROM \\select EMP_NO, APELLIDO, OFICIO,SALARIO,DEPT_NO
CREATE  OR ALTER PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO
(@posicion int,@oficio nvarchar(50)) 
as
select EMP_NO, APELLIDO, OFICIO,SALARIO,DEPT_NO from(select cast(ROW_NUMBER() over (order by APELLIDO)as int ) as POSICION,EMP_NO, APELLIDO, OFICIO,SALARIO,DEPT_NO from EMP 
WHERE OFICIO=@oficio) QUERY
WHERE (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 3)) 
go

exec SP_GRUPO_EMPLEADOS_OFICIO 3 ,'VENDEDOR'

    --PROCEDIMIENTO MEJORADO PARA QUE NOS DEVUELVAN LOS REGISTROS
    CREATE  OR ALTER PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO
(@posicion int,@oficio nvarchar(50),@registros int out) 
as
select EMP_NO, APELLIDO, OFICIO,SALARIO,DEPT_NO from(select cast(ROW_NUMBER() over (order by APELLIDO)as int ) as POSICION,EMP_NO, APELLIDO, OFICIO,SALARIO,DEPT_NO from EMP 
WHERE OFICIO=@oficio) QUERY
WHERE (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 3)) 

select  @registros = count(EMP_NO) FROM EMP where OFICIO=@oficio;
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

        public async Task<List<Departamento>>
            GetGrupoDepartamentosAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPARTAMENTOS @posicion";

            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        //EMPLEADOS PAGINACIÓN

        public async Task<int> GetEmpleadosCountAsync()
        {//Num empleados
            return await this.context.Empleados.CountAsync();
        }


        public async Task<List<Empleado>>
            GetGrupoEmpleadosAsync(int posicion)
        {//lo mismo que en dept
            string sql = "SP_GRUPO_EMPLEADOS @posicion";

            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados.Where(z => z.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>>
            GetGrupoEmpleadosOficiosAsync(int posicion,string oficio)
        {//lo mismo que en dept
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio";

            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);

            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio);//2 PARAMS
            return await consulta.ToListAsync();
        }

        public async Task<ModelEmpleadosOficio>
            GetGrupoEmpleadosOficiosOUTAsync(int posicion, string oficio)
        {//lo mismo que en dept
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio,@registros out";

            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);

            pamRegistros.Direction = ParameterDirection.Output;
            pamRegistros.DbType = DbType.Int32;//lo pasamos a int

            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio,pamRegistros);//3 PARAMS
            List<Empleado> empleados = await consulta.ToListAsync();
            //HASTA QUE NO HEMOS EXTRAIDO LOS DATOS
            //NO SE LIBERAN LOS PARAMS DE SALIDA!!
            int registros = (int) pamRegistros.Value;
            
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };

        }

        //A PARTE PAR LA PRACTICA

        public async Task<List<Departamento>> GetAllDepartamentosAsync()
        {

            return await this.context.Departamentos.ToListAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosPorDeptAsync(int iddept)
        {

            return await this.context.Empleados.Where(z => z.IdDepartamento == iddept).ToListAsync() ;
        }

        public async Task<int> GetNumerosRegistrosVistaEmpleadosAsync()
        {
            return await this.context.VistaEmpleadosIndividual.CountAsync();
        }

        public async Task<ModelEmpleadoDeptRegistro> GetGrupoVistaEmpleado(int posicion,int deptno)
        {
            string sql = "SP_EMPLEADOS_DEPARTAMENTO_REGISTRO @IDDEPARTAMENTO, @POSICION, @REGISTROS OUT";

            SqlParameter pamPosicion = new SqlParameter("@IDDEPARTAMENTO", deptno);
            SqlParameter pamOficio = new SqlParameter("@POSICION", posicion);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);

            pamRegistros.Direction = ParameterDirection.Output;
            pamRegistros.DbType = DbType.Int32;//lo pasamos a int

            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio, pamRegistros);//3 PARAMS

            List<Empleado> empleados = await consulta.ToListAsync();
            //HASTA QUE NO HEMOS EXTRAIDO LOS DATOS
            //NO SE LIBERAN LOS PARAMS DE SALIDA!!
            int registros = (int) pamRegistros.Value;

            return new ModelEmpleadoDeptRegistro
            {
                Empleado = empleados.FirstOrDefault(),
                Registros = registros
            };
        }











    }
}
