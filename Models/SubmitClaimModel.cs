namespace Part_2.Models {
    public class SubmitClaimModel {
        public string Title {
            get; set;
        }
        public int Rate {
            get; set;
        }

        public int Hours {
            get; set;
        }

        public DateTime StartDate {
            get; set;
        }
        public DateTime EndDate {
            get; set;
        }
    }
}