namespace Api.Definitions
{
    public class ApiParameter
    {
        public int PageNumber { get; set; } = 1;
        public int? PageSize { get; set; }
        public string Fields { get; set; }
        public string Expand { get; set; }
    }
}
