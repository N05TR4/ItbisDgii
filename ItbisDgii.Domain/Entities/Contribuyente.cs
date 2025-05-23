using ItbisDgii.Domain.Common;
using ItbisDgii.Domain.Enums;
using ItbisDgii.Domain.Exceptions;

namespace ItbisDgii.Domain.Entities
{
    public class Contribuyente : BaseEntity
    {
        public string RncCedula { get; set; }
        public string Nombre { get; set; }
        public TipoContribuyente Tipo { get; set; }
        public EstatusContribuyente Estatus { get; set; }

        
        public ICollection<ComprobanteFiscal> ComprobantesFiscales { get; set; } = new List<ComprobanteFiscal>();

        // Constructor for EF Core
        public Contribuyente() { }

        public Contribuyente(string rncCedula, string nombre, TipoContribuyente tipo, EstatusContribuyente estatus)
        {
            if (string.IsNullOrWhiteSpace(rncCedula))
                throw new DomainException("RNC/Cédula no puede ser vacío");

            if (tipo == TipoContribuyente.PersonaFisica && rncCedula.Length != 11)
                throw new DomainException("RNC/Cédula para persona física debe tener 11 dígitos");

            if (tipo == TipoContribuyente.PersonaJuridica && rncCedula.Length != 9)
                throw new DomainException("RNC para persona jurídica debe tener 9 dígitos");

            if (string.IsNullOrWhiteSpace(nombre))
                throw new DomainException("Nombre no puede ser vacío");

            RncCedula = rncCedula;
            Nombre = nombre;
            Tipo = tipo;
            Estatus = estatus;
        }

        public void ActualizarEstatus(EstatusContribuyente nuevoEstatus)
        {
            Estatus = nuevoEstatus;
        }

        public decimal CalcularTotalITBIS()
        {
            return ComprobantesFiscales.Sum(c => c.Itbis18);
        }
    }
}
