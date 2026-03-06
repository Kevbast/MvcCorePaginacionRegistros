using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCorePaginacionRegistros.Models
{
    
    [Table("V_DEPARTAMENTOS_INDIVIDUAL")]
    public class VistaDepartamento
    {
        [Key]
        [Column("DEPT_NO")]
        public int IdDepartamento { get; set; }
        [Column("DNOMBRE")]
        public string Nombre { get; set; }
        [Column("LOC")]
        public string Localidad { get; set; }
        [Column("POSICION")]//COMO ES UN CAMPO CALCULADO DEVUELVE INT64,O LO CAMBIAMOS X LONG O LO CASTEAMOS EN LA VISTA
        public int Posicion { get; set; }
    }
}
