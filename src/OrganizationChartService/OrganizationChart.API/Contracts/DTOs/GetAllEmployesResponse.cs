namespace OrganizationChart.API.Contracts.DTOs
{
    public class GetAllEmployesResponse
    {
        public string id { get; set; }
        public string title { get; set; }
        public List<User> users { get; set; }
    }

    public class User
    {
        public string name { get; set; }
        public string personnelNumber { get; set; }
        public string role { get; set; }
        public string imgUrl { get; set; }
        public string PersonalCode { get; set; }
        public string ActivityLocation { get; set; }
    }

}
