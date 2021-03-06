﻿using System;
using System.Collections.Generic;
using DataLayer.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Parsia.Core.Menu
{
    [ApiController]
    [ClassDetails(Clazz = "Menu", Facade = "MenuService")]
    public class MenuService : ControllerBase
    {
        private static readonly ClassDetails[] ClassDetails =
            (ClassDetails[]) typeof(MenuService).GetCustomAttributes(typeof(ClassDetails), true);

        private readonly IMemoryCache _memoryCache;
        private readonly IUserSessionManager _userSessionManager;

        public MenuService(IMemoryCache memoryCache, IUserSessionManager userSessionManager)
        {
            _memoryCache = memoryCache;
            _userSessionManager = userSessionManager;
        }

        [HttpPost]
        [Route("service/menu/gridView")]
        public ServiceResult<object> GridView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "gridView");
            return checkAccess.Done
                ? MenuFacade.GetInstance().GridView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/save")]
        public ServiceResult<object> Save()
        {
            var dtoFromRequest = MenuFacade.GetInstance().GetDtoFromRequest(HttpContext.Request);
            if (!dtoFromRequest.Done)
                return dtoFromRequest;
            var dto = (MenuDto) dtoFromRequest.Result;
            var userInfo = _userSessionManager.GetUserInfo(dto.Ticket, Request);
            var bp = new BusinessParam(userInfo);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz,
                dto.EntityId == 0 ? "insert" : "update");
            return checkAccess.Done ? MenuFacade.GetInstance().Save(bp, dto) : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/showRow")]
        public ServiceResult<object> ShowRow(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "update");
            return checkAccess.Done
                ? MenuFacade.GetInstance().ShowRow(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/delete")]
        public ServiceResult<object> Delete(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "delete");
            return checkAccess.Done
                ? MenuFacade.GetInstance().Delete(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/autocompleteView")]
        public ServiceResult<object> AutocompleteView(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            var bp = new BusinessParam(userInfo, clause);
            var checkAccess = _userSessionManager.CheckAccess(bp, ClassDetails[0].Clazz, "autocomplete");
            return checkAccess.Done
                ? MenuFacade.GetInstance().AutocompleteView(bp)
                : checkAccess;
        }

        [HttpPost]
        [Route("service/menu/getAllMenu")]
        public ServiceResult<object> GetAllMenu(Clause clause)
        {
            var userInfo = _userSessionManager.GetUserInfo(clause.Ticket, Request);
            if (userInfo == null)
                return new ServiceResult<object>(Enumerator.ErrorCode.UserExpired,
                    "زمان استفاده از حساب کاربری به پایان رسیده است");
            var bp = new BusinessParam(userInfo, clause);
            var data = new List<MenuDto>();
            if (_memoryCache.TryGetValue("Menu_" + bp.UserInfo.RoleId, out data))
            {
                return new ServiceResult<object>(data, data.Count);
            }

            var myServiceData = MenuFacade.GetInstance().GetAllMenu(bp);
            if (myServiceData.Done)
            {
                data = (List<MenuDto>) myServiceData.Result;
                _memoryCache.Set("Menu_" + bp.UserInfo.RoleId, data,
                    TimeSpan.FromMinutes(DataLayer.Tools.SystemConfig.MenuCacheTimeMinute));
            }

            return myServiceData;
        }
    }
}