using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PracticalTest.Models
{
    public class UsersModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "NRIC is required")]
        [RegularExpression(@"^[STFG]\d{7}[A-Z]$", ErrorMessage = "NRIC format is invalid.")]
        public string NRIC { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Birthday is required")]
        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        public int Age { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AvailableDate { get; set; }

        public List<int> Subjects { get; set; }
        public List<SubjectsModel> SelectedSubjectList { get; set; }

        public int SubjectCount { get; set; }
    }

    public class UserSubjectModel {
        public int UserId { get; set; }

        public int SubjectId { get; set; }

        public string SubjectName { get; set; }
    }
}