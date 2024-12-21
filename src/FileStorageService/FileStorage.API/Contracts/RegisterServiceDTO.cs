namespace FileStorage.API.Contracts;

public record RegisterServiceRequest(string ServiceName);
public record RegisterServiceResponse(int Id , string ServiceName);


