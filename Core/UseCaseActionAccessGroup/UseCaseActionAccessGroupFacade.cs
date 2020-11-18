using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Parsia.Core.Elastic;

namespace Parsia.Core.UseCaseActionAccessGroup
{
    [ClassDetails(Clazz = "UseCaseActionAccessGroup", Facade = "UseCaseActionAccessGroupFacade")]
    public class UseCaseActionAccessGroupFacade
    {
        private static readonly UseCaseActionAccessGroupFacade Facade = new UseCaseActionAccessGroupFacade();
        private static readonly UseCaseActionAccessGroupCopier Copier = new UseCaseActionAccessGroupCopier();
        private static readonly ClassDetails[] ClassDetails = (ClassDetails[])typeof(UseCaseActionAccessGroupFacade).GetCustomAttributes(typeof(ClassDetails), true);

        public static UseCaseActionAccessGroupFacade GetInstance()
        {
            return Facade;
        }
        private UseCaseActionAccessGroupFacade()
        {
        }

        public ServiceResult<object> SaveList(BusinessParam bp, List<UseCaseActionAccessGroupDto> lstDto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {

                foreach (var dto in lstDto)
                {
                    DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup caseActionAccessGroup;
                    if (dto.EntityId == 0)
                        using (var unitOfWork = new UnitOfWork())
                        {
                            caseActionAccessGroup = Copier.GetEntity(dto, bp, true);
                            if (caseActionAccessGroup.UseCaseAction != -1)
                            {
                                unitOfWork.UseCaseActionAccessGroup.Insert(caseActionAccessGroup);
                                unitOfWork.UseCaseActionAccessGroup.Save();
                            }
                           
                        }
                    else
                        using (var unitOfWork = new UnitOfWork())
                        {
                            caseActionAccessGroup = Copier.GetEntity(dto, bp, false);
                            unitOfWork.UseCaseActionAccessGroup.Update(caseActionAccessGroup);
                            unitOfWork.UseCaseActionAccessGroup.Save();
                        }
                    Elastic<UseCaseActionAccessGroupDto, DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup>.SaveToElastic(caseActionAccessGroup, "UseCaseActionAccessGroup", bp);
                }
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }
        public ServiceResult<object> DeletedList(BusinessParam bp, long accessGroupId)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var useCaseActionAccessGroups = unitOfWork.UseCaseActionAccessGroup.Get(p => p.AccessGroup == accessGroupId).ToList();
                    foreach (var item in useCaseActionAccessGroups)
                    {
                        unitOfWork.UseCaseActionAccessGroup.Delete(item);
                        Elastic<UseCaseActionAccessGroupDto, DataLayer.Model.Core.UseCaseActionAccessGroup.UseCaseActionAccessGroup>.SaveToElastic(item, "UseCaseActionAccessGroup", bp);
                    }
                    unitOfWork.UseCaseActionAccessGroup.Save();
                }
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

    }
}
