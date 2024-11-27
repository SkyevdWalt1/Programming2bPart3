namespace Part_2.Models {
    public class SubmittedDocs {
        public List<Blob> Blobs {
            get; set; 
        }

        public List<string> fileNames {
            get; set;
        }

        public List<string> fileTypes {
            get; set;
        }

        public string Notes {
            get; set;
        }
    }
}
