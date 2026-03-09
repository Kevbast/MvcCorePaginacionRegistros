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


    -- PROCEDURE PARA VISTA DE LOS EMP DEL DEPT

CREATE procedure SP_GRUPO_EMPLEADOS_DEPARTAMENTO
(@posicion int, @departamento int, @registros int out)
as
	select @registros= count(EMP_NO) from emp
	where DEPT_NO = @departamento
	SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO 
	from 
	(select cast(row_number() over (order by apellido) as int)
	as posicion,EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
	FROM EMP
	where DEPT_NO= @departamento) query
	where (query.POSICION = @posicion )
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

        public async Task<Departamento> FindDepartamento(int idDepartamento)
        {
            return await this.context.Departamentos.Where(z => z.IdDepartamento == idDepartamento).FirstOrDefaultAsync();
        }
        public async Task<int> EmpleadosCountAsync(int idDepartamento)
        {
            return await this.context.Empleados.Where(z => z.IdDepartamento == idDepartamento).CountAsync();
        }
        public async Task<ModelEmpleadosOficio> GetEmpleadosDepartamentoAsync(int idDepartamento, int posicion)
        {
            string sql = "sp_grupo_empleados_departamento @posicion,@departamento, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamDept = new SqlParameter("@departamento", idDepartamento);
            SqlParameter pamReg = new SqlParameter("@registros", 0);
            pamReg.DbType = DbType.Int32;
            pamReg.Direction = ParameterDirection.Output;
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamDept, pamReg);

            List<Empleado> empleados = await consulta.ToListAsync();
            //HASTA QUE NO HEMOS EXTRAIDO LOS DATOS (Empleados)
            // NO SE LIBERAN LOS PARAMETROS DE SALIDA
            int registros = (int)pamReg.Value;
            return new ModelEmpleadosOficio
            {
                Empleados = empleados,
                NumeroRegistros = registros
            };
        }
        public async Task<ModelEmpleadosOficio> GetEmpleadosDepartamentoEFAsync(int iddept, int posicion)
        {
            int numeroRegistros = await this.context.Empleados
                .Where(x => x.IdDepartamento == iddept)
                .CountAsync();

            List<Empleado> empleados = await this.context.Empleados
                .Where(x => x.IdDepartamento == iddept)
                .OrderBy(x => x.IdEmpleado)
                .Skip(posicion - 1)
                .Take(1)
                .ToListAsync();

            ModelEmpleadosOficio model = new ModelEmpleadosOficio
            {
                NumeroRegistros = numeroRegistros,
                Empleados = empleados
            };

            return model;
        }

        #region STOREDPROCEDUREEXTRA
        /*
         //PARA PLANTILLA
        SELECT * FROM PLANTILLA

CREATE procedure SP_GRUPO_HOSPITALES_PLANTILLA
(@posicion int, @idhospital int, @registros int out)
as
	select @registros= count(EMPLEADO_NO) from PLANTILLA
	where HOSPITAL_COD = @idhospital;

	SELECT EMPLEADO_NO, APELLIDO, FUNCION, SALARIO,HOSPITAL_COD 
	from 
	(select cast(row_number() over (order by APELLIDO) as int)
	as posicion,EMPLEADO_NO, APELLIDO, FUNCION, SALARIO, HOSPITAL_COD
	FROM PLANTILLA
	where HOSPITAL_COD= @idhospital) query
	where (query.POSICION = @posicion )
go

         */

        #endregion
        public async Task<List<Hospital>> GetAllHospitalesAsync()
        {

            return await this.context.Hospitales.ToListAsync();
        }

        public async Task<Hospital> FindHospital(int idHospital)
        {
            return await this.context.Hospitales.Where(z => z.IdHospital == idHospital).FirstOrDefaultAsync();
        }
        public async Task<int> EmpleadosPlantillaCountAsync(int idHospital)
        {
            return await this.context.EmpleadosPlantilla.Where(z => z.IdHospital == idHospital).CountAsync();
        }
        public async Task<ModelPlantillaHospital> GetEmpleadosPlantillaHospitalAsync(int idHospital, int posicion)
        {
            string sql = "SP_GRUPO_HOSPITALES_PLANTILLA @posicion,@idhospital, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamHospi = new SqlParameter("@idhospital", idHospital);
            SqlParameter pamReg = new SqlParameter("@registros", 0);
            pamReg.DbType = DbType.Int32;
            pamReg.Direction = ParameterDirection.Output;

            var consulta = this.context.EmpleadosPlantilla.FromSqlRaw(sql, pamPosicion, pamHospi, pamReg);

            List<Plantilla> empleadosPlantilla = await consulta.ToListAsync();
            //HASTA QUE NO HEMOS EXTRAIDO LOS DATOS (Empleados)
            // NO SE LIBERAN LOS PARAMETROS DE SALIDA
            int registros = (int)pamReg.Value;
            return new ModelPlantillaHospital
            {
                EmpleadosPlantilla = empleadosPlantilla,
                NumeroRegistros = registros
            };
        }






    }
}
