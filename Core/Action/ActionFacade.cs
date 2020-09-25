using System;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.Action
{
    public class ActionFacade : IBaseFacade<ActionDto>
    {
        private static readonly ActionFacade Facade = new ActionFacade();
        private static readonly ActionCopier Copier = new ActionCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private ActionFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, ActionDto dto)
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

        public static ActionFacade GetInstance()
        {
            return Facade;
        }
    }
}