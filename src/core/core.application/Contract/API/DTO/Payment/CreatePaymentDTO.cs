namespace core.application.Contract.API.DTO.Payment;

public class CreatePaymentDTO
{
    public List<long> expenses { get; set; }
    public int createBy { get; set; }

}
