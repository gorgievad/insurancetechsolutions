namespace Claims.Domain.DTO
{
    public class CoverDto
    {
        public CoverDto() { }

        public CoverDto(Cover cover)
        {
            Id = cover.Id;
            StartDate = cover.StartDate;
            EndDate = cover.EndDate;
            Type = cover.Type;
            Premium = cover.Premium;
        }

        public string Id { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType Type { get; set; }
        public decimal Premium { get; set; }
    }
}
