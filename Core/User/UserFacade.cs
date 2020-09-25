using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.User
{
    public class UserFacade:IBaseFacade<UserDto>
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private static readonly UserFacade Facade = new UserFacade();
        private static readonly UserCopier Copier = new UserCopier();
        private UserFacade()
        {
        }
        public static UserFacade GetInstance()
        {
            return Facade;
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, UserDto dto)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult<object> Delete(BusinessParam bp)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            throw new System.NotImplementedException();
        }
    }
}