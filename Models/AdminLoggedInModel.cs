namespace Part_2.Models {
    public class AdminLoggedInModel {
        public User User {
            get; set;
        }

        public List<AdminClaims> Claims {
            get; set;
        }

        public AdminLoggedInModel() {
        }

        public AdminLoggedInModel(User user, List<AdminClaims> claims) {
            this.User = user;
            this.Claims = claims;
        }
    }
}
