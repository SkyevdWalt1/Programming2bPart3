namespace Part_2.Models {
    public class Claim {
        public int ID {
            get; set;
        }
        
        public string Title {
            get; set;
        }

        public int HourlyRate {
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

        public string Status {
            get; set;
        }

        public int Total => this.Hours * this.HourlyRate;

        public Claim() {
        }

        public Claim(int iD, string title, int hourlyRate, int hours, DateTime startDate, DateTime endDate, string status) {
            this.ID = iD;
            this.Title = title;
            this.HourlyRate = hourlyRate;
            this.Hours = hours;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Status = status;
        }
    }
}
