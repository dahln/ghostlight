namespace template.Server.Utility
{
    static public class ToSharedModels
    {
        static public Shared.CustomerSlim ToSharedCustomerSlim(this Entities.Customer model)
        {
            var customer = new Shared.CustomerSlim()
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone
            };

            return customer;
        }

        static public Shared.Customer ToSharedCustomer(this Entities.Customer model)
        {
            var customer = new Shared.Customer()
            {
                Id = model.Id,
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                City = model.City,
                State = model.State,
                Postal = model.Postal,
                Notes = model.Notes,
                BirthDate = model.BirthDate
            };

            return customer;
        }
    }
}
