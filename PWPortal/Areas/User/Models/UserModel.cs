using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;

namespace PWPortal.Areas.User.Models
{
    public class UserModel
    {
        public int UserId { get; set; }            

        public string FullName { get; set; }

        public string Email { get; set; }

        public string Group { get; set; }

        public bool Status { get; set; }

        public string LastLogin { get; set; }      
    }

    public class AddUserModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address")]     
        public string Email { get; set; }
        [RegularExpression(@"^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}", ErrorMessage = "Please enter a valid phone number")]     
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Group")]
        public string selectedGroupId { get; set; }
     
        public List<ItemsList> groupList { get; set; }       
    }

    public class EditUserModel
    {
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email address")]
        public string Email { get; set; }
        [RegularExpression(@"^\+?\d{0,2}\-?\d{4,5}\-?\d{5,6}", ErrorMessage = "Please enter a valid phone number")]
        public string Phone { get; set; }
        [Required]
        [Display(Name = "Group")]
        public string selectedGroupId { get; set; }       
        [Required]
        [Display(Name = "Status")]
        public string SeletedStatusId { get; set; }

        public List<ItemsList> groupList { get; set; }

        public List<ItemsList> statusList { get; set; }     
    }


    public class ItemsList
    {
        public string Value { get; set; }
        public string Text { get; set; }
    }

    
}