using System;

namespace UploadService.Model {

    public class Ingredient {
        public string name { get; set; }
        public string id { get; set; }
        public string version { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public bool deleted { get; set; }
    }


}
