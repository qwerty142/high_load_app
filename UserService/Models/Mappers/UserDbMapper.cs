using Riok.Mapperly.Abstractions;
using UserService.Entities;

namespace UserService.Models.Mappers;

[Mapper]
public partial class UserDbMapper
{
    public partial UserDataModel ToUserDataModel(IUser user);
    public partial User ToUser(IUser userDataModel);
}