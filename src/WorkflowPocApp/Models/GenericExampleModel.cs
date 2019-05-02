using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowPocApp.Models
{
    public class GenericExampleModel
    {
        public int Id { get; set; }

        public string Status { get; set; }

        public List<GenericExampleTaskModel> Tasks { get; set; } = new List<GenericExampleTaskModel>();
    }
}
