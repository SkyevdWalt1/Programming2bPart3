namespace Part_2.Models {
    public class AdminClaims {
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

        public string Sender {
            get; set;
        }

        public int Total => this.HourlyRate * this.Hours;

        public AdminClaims() {
        }

        public AdminClaims(int iD, string title, int hourlyRate, int hours, DateTime startDate, DateTime endDate, string status, string sender) {
            this.ID = iD;
            this.Title = title;
            this.HourlyRate = hourlyRate;
            this.Hours = hours;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Status = status;
            this.Sender = sender;
        }
    }
}
