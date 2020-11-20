using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Parsia.Core.Elastic;
using Parsia.Core.RoleAccessGroup;

namespace Parsia.Core.roleAccessGroup
{
    [ClassDetails(Clazz = "RoleAccessGroup", Facade = "RoleAccessGroupFacade")]
    public class RoleAccessGroupFacade
    {
        private static readonly RoleAccessGroupFacade Facade = new RoleAccessGroupFacade();
        private static readonly RoleAccessGroupCopier Copier = new RoleAccessGroupCopier();

        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(RoleAccessGroupFacade).GetCustomAttributes(typeof(ClassDetails), true);

        private RoleAccessGroupFacade()
        {
        }

        public static RoleAccessGroupFacade GetInstance()
        {
            return Facade;
        }

        public ServiceResult<object> SaveList(BusinessParam bp, List<RoleAccessGroupDto> lstDto)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                foreach (var dto in lstDto)
                {
                    DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup accessGroup;
                    if (dto.EntityId == 0)
                        using (var unitOfWork = new UnitOfWork())
                        {
                            accessGroup = Copier.GetEntity(dto, bp, true);
                            unitOfWork.RoleAccessGroup.Insert(accessGroup);
                            unitOfWork.RoleAccessGroup.Save();
                        }
                    else
                        using (var unitOfWork = new UnitOfWork())
                        {
                            accessGroup = Copier.GetEntity(dto, bp, false);
                            unitOfWork.RoleAccessGroup.Update(accessGroup);
                            unitOfWork.RoleAccessGroup.Save();
                        }

                    Elastic<RoleAccessGroupDto, DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup>.SaveToElastic(
                        accessGroup, ClassDetails[0].Clazz, bp);
                }

                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, ClassDetails[0].Facade + methodName, bp.UserInfo);
            }
        }

        public ServiceResult<object> DeletedList(BusinessParam bp, long roleId)
        {
            var methodName = $".{new StackTrace().GetFrame(1).GetMethod().Name}";
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var accessGroups = unitOfWork.RoleAccessGroup.Get(p => p.Role == roleId).ToList();
                    foreach (var item in accessGroups)
                    {
                        unitOfWork.RoleAccessGroup.Delete(item);
                        Elastic<RoleAccessGroupDto, DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup>.SaveToElastic(
                            item, ClassDetails[0].Clazz, bp);
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