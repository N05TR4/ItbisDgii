using ItbisDgii.Domain.Common;
using ItbisDgii.Domain.Exceptions;

namespace ItbisDgii.Domain.Entities
{
    public class ComprobanteFiscal : BaseEntity
    {
        public Guid ContribuyenteId { get; set; }
        public string RncCedula { get; set; }
        public string NCF { get; set; }
        public decimal Monto { get; set; }
        public decimal Itbis18 { get; set; }

        
        public Contribuyente? Contribuyente { get; set; }

        // Constructor for EF Core
        public ComprobanteFiscal() { }

        public ComprobanteFiscal(string rncCedula, string ncf, decimal monto)
        {
            if (string.IsNullOrWhiteSpace(rncCedula))
                throw new DomainException("RNC/Cédula no puede ser vacío");

            if (string.IsNullOrWhiteSpace(ncf))
                throw new DomainException("NCF no puede ser vacío");

            if (monto <= 0)
                throw new DomainException("Monto debe ser mayor que cero");

            RncCedula = rncCedula;
            NCF = ncf;
            Monto = monto;
            Itbis18 = CalcularITBIS(monto);
        }

        private static decimal CalcularITBIS(decimal monto)
        {
            // ITBIS es el 18% del monto
            return Math.Round(monto * 0.18m, 2);
        }
    }
}
