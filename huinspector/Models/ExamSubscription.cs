//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace huinspector.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ExamSubscription
    {
        public int UserId { get; set; }
        public int ExamId { get; set; }
        public int Id { get; set; }
        public Nullable<decimal> Result { get; set; }
        public Nullable<int> ExamClassroomId { get; set; }
        public int Week { get; set; }
        public Nullable<bool> IsNA { get; set; }
    
        public virtual Exam Exam { get; set; }
        public virtual User User { get; set; }
    }
}