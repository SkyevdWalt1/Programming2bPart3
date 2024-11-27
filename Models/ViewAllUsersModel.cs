namespace Part_2.Models {
    public class ViewAllUsersModel {
        public List<User> ActiveUsers {
            get; set; 
        }

        public List<PendingUsers> PendingUsers {
            get; set;
        }
    }
}
