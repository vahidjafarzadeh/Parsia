using System;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.AccessGroup
{
    public class AccessGroupFacade : IBaseFacade<AccessGroupDto>
    {
        private static readonly AccessGroupFacade Facade = new AccessGroupFacade();
        private static readonly AccessGroupCopier Copier = new AccessGroupCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private AccessGroupFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, AccessGroupDto dto)
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

        public static AccessGroupFacade GetInstance()
        {
            return Facade;
        }
    }
}