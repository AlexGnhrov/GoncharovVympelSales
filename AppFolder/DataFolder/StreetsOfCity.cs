//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GoncharovCarPartsAS.AppFolder.DataFolder
{
    using System;
    using System.Collections.Generic;
    
    public partial class StreetsOfCity
    {
        public int StreetsOfCityID { get; set; }
        public int CityID { get; set; }
        public int StreetID { get; set; }
    
        public virtual City City { get; set; }
        public virtual Street Street { get; set; }
    }
}