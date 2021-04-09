using System;

namespace UploadService.Model {
    public class Ingredient {
        public bool deleted { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime createdAt { get; set; }
        public string version { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

}
