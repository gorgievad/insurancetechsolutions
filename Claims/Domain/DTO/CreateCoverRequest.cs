namespace Claims.Domain.DTO
{
    public class CreateCoverRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType Type { get; set; }
    }
}
