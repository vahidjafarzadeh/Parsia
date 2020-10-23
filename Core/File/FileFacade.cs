using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Base;
using DataLayer.Context;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Parsia.Core.File
{
    public class FileFacade : IBaseFacade<FileDto>
    {
        private static readonly FileFacade Facade = new FileFacade();
        private static readonly FileCopier Copier = new FileCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        private readonly ParsiContext _context = new ParsiContext();

        private FileFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            try
            {
                var queryString = "select * from (SELECT * FROM [CO].[File]) e" +
                       QueryUtil.GetWhereClause(bp.Clause, QueryUtil.GetConstraintForNativeQuery(bp)) +
                    QueryUtil.GetOrderByClause(bp.Clause);
                var files = _context.File.FromSqlRaw(queryString).ToList();
                var lstData = files.Select(file => Copier.GetDto(file)).ToList();
                return lstData.Count <= 0 ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "NO_DATA") : new ServiceResult<object>(lstData, lstData.Count);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.GridView", bp.UserInfo);
            }
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

        public ServiceResult<object> GetAllExtension(BusinessParam bp)
        {
            try
            {
                var extensionList = _unitOfWork.File.Get().Select(p=>p.Extension).Distinct().ToList();
                return extensionList.Count <= 0 ? new ServiceResult<object>(Enumerator.ErrorCode.NotFound, "پسوندی برای نمایش در قسمت فیلترها یافت نشد یافت نشد") : new ServiceResult<object>(extensionList, extensionList.Count);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "FileFacade.GetAllExtension", bp.UserInfo);
            }
        }
        public static FileFacade GetInstance()
        {
            return Facade;
        }
    }
}