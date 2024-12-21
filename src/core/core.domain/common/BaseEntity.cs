namespace common.defination
{

    public abstract class BaseEntity
    {

        public bool? IsDelete { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime ModifyDate { get; set; }
        public string? ModifyBy { get; set; }
    }
}
