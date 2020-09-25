using System;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.File
{
    public class FileFacade : IBaseFacade<FileDto>
    {
        private static readonly FileFacade Facade = new FileFacade();
        private static readonly FileCopier Copier = new FileCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private FileFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, FileDto dto)
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

        public static FileFacade GetInstance()
        {
            return Facade;
        }
    }
}