using System;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.Menu
{
    public class MenuFacade : IBaseFacade<MenuDto>
    {
        private static readonly MenuFacade Facade = new MenuFacade();
        private static readonly MenuCopier Copier = new MenuCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private MenuFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, MenuDto dto)
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

        public static MenuFacade GetInstance()
        {
            return Facade;
        }
    }
}