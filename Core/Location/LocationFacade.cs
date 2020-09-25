using System;
using DataLayer.Base;
using DataLayer.Tools;
using Datalayer.UnitOfWork;

namespace Parsia.Core.Location
{
    public class LocationFacade : IBaseFacade<LocationDto>
    {
        private static readonly LocationFacade Facade = new LocationFacade();
        private static readonly LocationCopier Copier = new LocationCopier();
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        private LocationFacade()
        {
        }

        public ServiceResult<object> GridView(BusinessParam bp)
        {
            throw new NotImplementedException();
        }

        public ServiceResult<object> Save(BusinessParam bp, LocationDto dto)
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

        public static LocationFacade GetInstance()
        {
            return Facade;
        }
    }
}