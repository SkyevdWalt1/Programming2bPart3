namespace Part_2.Models {
    public class UserLoggedInModel {
        public User User {
            get; set;
        }

        public List<Claim> Claims {
            get; set;
        }

        public UserLoggedInModel() {
        }

        public UserLoggedInModel(User user, List<Claim> claims) {
            this.User = user;
            this.Claims = claims;
        }
    }
}
