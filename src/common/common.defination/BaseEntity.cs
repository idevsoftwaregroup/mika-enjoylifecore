namespace common.defination
{

    public abstract class BaseEntity
    {

        public bool? IsDelete { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ModifyDate { get; set; }
        public string? ModifyBy { get; set; }
    }
}
