using System;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.User
{
    public class UserFacade : IBaseFacade<UserDto>
    {
        private static readonly UserFacade Facade = new UserFacade();
        private static readonly UserCopier Copier = new UserCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private UserFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, UserDto dto)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> ShowRow(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Delete(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> AutocompleteView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public static UserFacade GetInstance()
        {
            return Facade;
        }
    }
}