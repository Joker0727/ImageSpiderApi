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
    
    public partial class AccessRecord
    {
        public int Id { get; set; }
        public string OpenId { get; set; }
        public string NickName { get; set; }
        public Nullable<System.DateTime> AccessTime { get; set; }
        public Nullable<System.DateTime> ExitTime { get; set; }
        public Nullable<double> StayTime { get; set; }
        public Nullable<int> AccountId { get; set; }
    
        public virtual Account Account { get; set; }
    }
}
