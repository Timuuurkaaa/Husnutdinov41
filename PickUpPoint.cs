//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Husnutdinov41
{
    using System;
    using System.Collections.Generic;
    
    public partial class PickUpPoint
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PickUpPoint()
        {
            this.Order = new HashSet<Order>();
        }
    
        public int PickUpPointID { get; set; }
        public string PickUpPointIndex { get; set; }
        public string PickUpPointCity { get; set; }
        public string PickUpPointStreet { get; set; }
        public string PickUpPointHome { get; set; }

        public string PickUpPointAddress
        {
            get
            {
                return PickUpPointIndex + " " + PickUpPointCity + " " + PickUpPointStreet + " " + PickUpPointHome;
            }

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
