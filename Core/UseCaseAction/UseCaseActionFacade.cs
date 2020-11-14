
using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Parsia.Core.Elastic;

namespace Parsia.Core.UseCaseAction
{
    public class UseCaseActionFacade
    {
        private static readonly UseCaseActionFacade Facade = new UseCaseActionFacade();
        private static readonly UseCaseActionCopier Copier = new UseCaseActionCopier();
        private UseCaseActionFacade()
        {
        }
        public static UseCaseActionFacade GetInstance()
        {
            return Facade;
        }

        public long GetUseCaseAction(UseCaseActionDto dto)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                   return unitOfWork.UseCaseAction.Get(p=>p.Action == dto.Action.EntityId && p.UseCase == dto.UseCase.EntityId).Select(p=>p.EntityId).FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }
        public ServiceResult<object> SaveList(BusinessParam bp, List<UseCaseActionDto> lstDto)
        {
            try
            {

                foreach (var dto in lstDto)
                {
                    DataLayer.Model.Core.UseCaseAction.UseCaseAction useCaseAction;
                    if (dto.EntityId == 0)
                        using (var unitOfWork = new UnitOfWork())
                        {
                            useCaseAction = Copier.GetEntity(dto, bp, true);
                            unitOfWork.UseCaseAction.Insert(useCaseAction);
                            unitOfWork.UseCaseAction.Save();
                        }
                    else
                        using (var unitOfWork = new UnitOfWork())
                        {
                            useCaseAction = Copier.GetEntity(dto, bp, false);
                            unitOfWork.UseCaseAction.Update(useCaseAction);
                            unitOfWork.UseCaseAction.Save();
                        }
                    Elastic<UseCaseActionDto, DataLayer.Model.Core.UseCaseAction.UseCaseAction>.SaveToElastic(useCaseAction, "UseCaseAction", bp);
                }
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UseCaseFacade.Save", bp.UserInfo);
            }
        }

        public ServiceResult<object> DeletedList(BusinessParam bp, long useCaseId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var useCaseActions = unitOfWork.UseCaseAction.Get(p=>p.UseCase == useCaseId).ToList();
                    foreach (var item in useCaseActions)
                    {
                        unitOfWork.UseCaseAction.Delete(item.EntityId);
                    }
                    unitOfWork.UseCaseAction.Save();
                }
                return new ServiceResult<object>(true,1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "UseCaseFacade.Save", bp.UserInfo);
            }
        }
    }
}
