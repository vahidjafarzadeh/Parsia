using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.Tools;
using Datalayer.UnitOfWork;
using Parsia.Core.Elastic;
using Parsia.Core.RoleAccessGroup;

namespace Parsia.Core.roleAccessGroup
{
    public class RoleAccessGroupFacade
    {
        private static readonly RoleAccessGroupFacade Facade = new RoleAccessGroupFacade();
        private static readonly RoleAccessGroupCopier Copier = new RoleAccessGroupCopier();

        public static RoleAccessGroupFacade GetInstance()
        {
            return Facade;
        }
        private RoleAccessGroupFacade()
        {
        }

        public ServiceResult<object> SaveList(BusinessParam bp, List<RoleAccessGroupDto> lstDto)
        {
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
                    Elastic<RoleAccessGroupDto, DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup>.SaveToElastic(accessGroup, "RoleAccessGroup", bp);
                }
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "RoleAccessGroupFacade.Save", bp.UserInfo);
            }
        }
        public ServiceResult<object> DeletedList(BusinessParam bp, long roleId)
        {
            try
            {
                using (var unitOfWork = new UnitOfWork())
                {
                    var accessGroups = unitOfWork.RoleAccessGroup.Get(p => p.Role == roleId).ToList();
                    foreach (var item in accessGroups)
                    {
                        unitOfWork.RoleAccessGroup.Delete(item);
                        Elastic<RoleAccessGroupDto, DataLayer.Model.Core.RoleAccessGroup.RoleAccessGroup>.SaveToElastic(item, "RoleAccessGroup", bp);
                    }
                    unitOfWork.UseCaseActionAccessGroup.Save();
                }
                return new ServiceResult<object>(true, 1);
            }
            catch (Exception e)
            {
                return ExceptionUtil.ExceptionHandler(e, "RoleAccessGroupFacade.DeletedList", bp.UserInfo);
            }
        }

    }
}
