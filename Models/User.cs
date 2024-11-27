namespace Part_2.Models {
    public class User {
        public int ID {
            get; set;
        }

        public string Username {
            get; set;
        }

        public bool IsAdmin {
            get; set;
        }

        public string Role {
            get => this.IsAdmin ? "Admin" : "User";
        }

        public User() {
        }

        public User(int id, string username, bool isAdmin) {
            this.ID = id;
            this.Username = username;
            this.IsAdmin = isAdmin;
        }
    }
}
