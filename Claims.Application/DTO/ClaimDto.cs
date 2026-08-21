using Claims.Domain;

namespace Claims.Application.DTO
{
    public class ClaimDto
    {
        public ClaimDto() { }

        public ClaimDto(Claim claim)
        {
            Id = claim.Id;
            CoverId = claim.CoverId;
            Created = claim.Created;
            Name = claim.Name;
            Type = claim.Type;
            DamageCost = claim.DamageCost;
        }

        public string Id { get; set; } = string.Empty;
        public string CoverId { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public string Name { get; set; } = string.Empty;
        public ClaimType Type { get; set; }
        public decimal DamageCost { get; set; }
    }
}
