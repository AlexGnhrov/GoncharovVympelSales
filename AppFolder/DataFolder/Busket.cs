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
    
    public partial class Busket
    {
        public int ClientBuscketID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public Nullable<int> StaffID { get; set; }
        public int ProductID { get; set; }
        public int Amount { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual Product Product { get; set; }
        public virtual Staff Staff { get; set; }
    }
}
