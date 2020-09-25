﻿using System;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.ComboVal
{
    public class ComboValFacade : IBaseFacade<ComboValDto>
    {
        private static readonly ComboValFacade Facade = new ComboValFacade();
        private static readonly ComboValCopier Copier = new ComboValCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private ComboValFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, ComboValDto dto)
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

        public static ComboValFacade GetInstance()
        {
            return Facade;
        }
    }
}