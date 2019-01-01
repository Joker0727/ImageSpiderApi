//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ImageSpiderApi.EF
{
    using System;
    using System.Collections.Generic;
    
    public partial class CatalogTable
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CatalogTable()
        {
            this.Classifications = new HashSet<Classification>();
            this.CollectionTables = new HashSet<CollectionTable>();
            this.ImageTables = new HashSet<ImageTable>();
            this.Browses = new HashSet<Browse>();
        }
    
        public int Id { get; set; }
        public string WebSiteUrl { get; set; }
        public string CatalogUrl { get; set; }
        public string Describe { get; set; }
        public Nullable<int> TotalImages { get; set; }
        public Nullable<bool> IsDownLoad { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Classification> Classifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CollectionTable> CollectionTables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageTable> ImageTables { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Browse> Browses { get; set; }
    }
}
