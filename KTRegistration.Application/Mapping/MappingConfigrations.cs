namespace KTRegistration.Core.Mapping;
public class MappingConfigrations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        //ApplicationUser
        config.NewConfig<RegisterRequest, ApplicationUser>()
           .Map(dest => dest.Id, src => src.ICNumber)
           .Map(dest => dest.Email, src => src.Email.ToLower())
           .Map(dest => dest.UserName, src => src.ICNumber);
    }
}
