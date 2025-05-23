namespace ItbisDgii.Application.DTOs
{
    public class ContribuyenteDto
    {
        public Guid Id { get; set; }
        public string RncCedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Estatus { get; set; } = string.Empty;
        public decimal TotalITBIS { get; set; }
    }
}
