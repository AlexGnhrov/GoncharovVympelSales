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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.ProductGroup = new HashSet<ProductGroup>();
        }
    
        public int OrderID { get; set; }
        public string UQnum { get; set; }
        public Nullable<int> StaffID { get; set; }
        public Nullable<int> ClientID { get; set; }
        public decimal Price { get; set; }
        public Nullable<System.DateTime> OrderStart { get; set; }
        public System.DateTime OrderIssue { get; set; }
        public Nullable<System.DateTime> ShellLife { get; set; }
        public System.DateTime DeliveryDate { get; set; }
        public int StatusOrderID { get; set; }
        public int TypeOfIssueD { get; set; }
        public Nullable<int> DepartamentCompanyID { get; set; }
        public string DeliveryAdress { get; set; }
        public string CancelDescription { get; set; }
        public int PaymentTypetID { get; set; }
    
        public virtual Client Client { get; set; }
        public virtual DepartamentCompany DepartamentCompany { get; set; }
        public virtual PaymentType PaymentType { get; set; }
        public virtual Staff Staff { get; set; }
        public virtual StatusOrder StatusOrder { get; set; }
        public virtual TypeOfIssueOrder TypeOfIssueOrder { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProductGroup> ProductGroup { get; set; }
    }
}