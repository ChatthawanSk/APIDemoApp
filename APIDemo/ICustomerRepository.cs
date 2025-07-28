using System.Data;

public interface ICustomerRepository
{
    DataTable GetCustomerInfo(string id);
    DataTable GetCustomerInfo(string id,DateOnly startDate,DateOnly endDate);
   
}