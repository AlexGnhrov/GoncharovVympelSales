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
    
    public partial class Adress
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Adress()
        {
            this.DepartamentCompany = new HashSet<DepartamentCompany>();
            this.Passport = new HashSet<Passport>();
        }
    
        public int Adress1 { get; set; }
        public int RegionID { get; set; }
        public int CityID { get; set; }
        public int StreetID { get; set; }
        public int Home { get; set; }
        public string Building { get; set; }
        public Nullable<int> Appartament { get; set; }
    
        public virtual City City { get; set; }
        public virtual Region Region { get; set; }
        public virtual Street Street { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DepartamentCompany> DepartamentCompany { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Passport> Passport { get; set; }
    }
}
