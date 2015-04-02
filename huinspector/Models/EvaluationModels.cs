using huinspector.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace huinspector.Models
{
    public class EvaluationIndexModel
    {
        public IList<Evaluation> EvaluationsList { get; set; }
    }

    public class EvaluationAddModel
    {
        public IList<ExamSubscription> ExamSubscriptions {get; set;}
        public Exam ExamRequested { get; set; }
    }

    public class EvaluationInsertModel
    {
        public int Result { get; set; }
        public int Userid { get; set; } 
        public int ExamId { get; set; }
        [DataType(DataType.Upload)]
        public HttpPostedFileBase Document { get; set; }
        public string [] CheckBoxResult { get; set; }
    }
}