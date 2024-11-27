using Microsoft.AspNetCore.Mvc.TagHelpers;
using MySql.Data.MySqlClient;
using Part_2.Models;

namespace Part_2.Services {
    public class Db {
        public readonly string constr;

        public Db() {
            this.constr = "server=localhost;uid=root;pwd=Alph@47C;database=part_2";
        }

        public bool LoginUser(string username, string password) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                // Prepare the SQL command to get the user by username
                string query = "SELECT password FROM users WHERE username = @username";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            string storedPassword = reader.GetString(0);
                            return storedPassword == password;
                        }
                    }
                }
            }

            return false;
        }

        public User GetUser(string username) {
            User user = null;

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "SELECT id, username, is_admin FROM users WHERE username = @username";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@username", username);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            // User found
                            user = new User {
                                ID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                IsAdmin = reader.GetBoolean(2)
                            };
                        }
                    }
                }
            }
            return user;
        }

        public User GetUserById(int id) {
            User user = null;

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                // Query to select user by ID
                string query = "SELECT id, username, is_admin FROM users WHERE id = @id";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@id", id);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            // User found
                            user = new User {
                                ID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                IsAdmin = reader.GetBoolean(2)
                            };
                        }
                    }
                }
            }
            return user;
        }


        public bool UsernameExists(string username) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "SELECT COUNT(*) FROM pending_users WHERE username = @username";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@username", username);

                    long count = (long)command.ExecuteScalar();

                    return count > 0;
                }
            }
        }

        public void SignUpUser(string username, string password) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "INSERT INTO pending_users (username, password) VALUES (@username, @password)";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Claim GetClaim(int claimID) {
            var claim = new Claim();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();
                string query = @"SELECT * from claims where id = @id";

                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@id", claimID);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            claim = new Claim {
                                ID = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                HourlyRate = reader.GetInt32("hourly_rate"),
                                Hours = reader.GetInt32("hours"),
                                StartDate = reader.GetDateTime("start_date"),
                                EndDate = reader.GetDateTime("end_date"),
                                Status = reader.GetString("status")
                            };
                        }
                    }
                }
            }

            return claim;
        }

        public List<Claim> GetClaims(int userId) {
            var claims = new List<Claim>();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();
                string query = @"SELECT c.id, c.title, c.hourly_rate, c.hours, c.start_date, c.end_date, c.status
                                 FROM claims c
                                 JOIN user_claim uc ON c.id = uc.claim_id
                                 WHERE uc.user_id = @UserId";

                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@UserId", userId);

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var claim = new Claim {
                                ID = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                HourlyRate = reader.GetInt32("hourly_rate"),
                                Hours = reader.GetInt32("hours"),
                                StartDate = reader.GetDateTime("start_date"),
                                EndDate = reader.GetDateTime("end_date"),
                                Status = reader.GetString("status")
                            };
                            claims.Add(claim);
                        }
                    }
                }
            }

            return claims;
        }

        public List<AdminClaims> GetAdminClaims() {
            var adminClaims = new List<AdminClaims>();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();
                string query = @"
                    SELECT c.id, c.title, c.hourly_rate, c.hours, c.start_date, c.end_date, c.status, u.username 
                    FROM claims c 
                    JOIN user_claim uc ON c.id = uc.claim_id 
                    JOIN users u ON uc.user_id = u.id";

                using (var command = new MySqlCommand(query, connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var claim = new AdminClaims {
                                ID = reader.GetInt32("id"),
                                Title = reader.GetString("title"),
                                HourlyRate = reader.GetInt32("hourly_rate"),
                                Hours = reader.GetInt32("hours"),
                                StartDate = reader.GetDateTime("start_date"),
                                EndDate = reader.GetDateTime("end_date"),
                                Status = reader.GetString("status"),
                                Sender = reader.GetString("username")
                            };
                            adminClaims.Add(claim);
                        }
                    }
                }
            }

            return adminClaims;
        }

        public void SubmitClaim(int userId, string title, int rate, int hours, DateTime startDate, DateTime endDate) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                // Start a transaction to ensure both inserts succeed
                using (var transaction = connection.BeginTransaction()) {
                    try {
                        // Step 1: Insert into the claims table
                        string claimQuery = "INSERT INTO claims (title, hourly_rate, hours, start_date, end_date, status) VALUES (@title, @rate, @hours, @startDate, @endDate, 'Active')";
                        using (var claimCommand = new MySqlCommand(claimQuery, connection, transaction)) {
                            claimCommand.Parameters.AddWithValue("@title", title);
                            claimCommand.Parameters.AddWithValue("@rate", rate);
                            claimCommand.Parameters.AddWithValue("@hours", hours);
                            claimCommand.Parameters.AddWithValue("@startDate", startDate);
                            claimCommand.Parameters.AddWithValue("@endDate", endDate);
                            claimCommand.ExecuteNonQuery();
                        }

                        // Step 2: Get the last inserted claim ID
                        string getClaimIdQuery = "SELECT LAST_INSERT_ID()";
                        int claimId;
                        using (var idCommand = new MySqlCommand(getClaimIdQuery, connection, transaction)) {
                            claimId = Convert.ToInt32(idCommand.ExecuteScalar());
                        }

                        // Step 3: Insert into the user_claim table
                        string userClaimQuery = "INSERT INTO user_claim (user_id, claim_id) VALUES (@userId, @claimId)";
                        using (var userClaimCommand = new MySqlCommand(userClaimQuery, connection, transaction)) {
                            userClaimCommand.Parameters.AddWithValue("@userId", userId);
                            userClaimCommand.Parameters.AddWithValue("@claimId", claimId);
                            userClaimCommand.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    } catch {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public List<Claim> GetPaidClaims(int userId) {
            List<Claim> paidClaims = new();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = @"
                    SELECT c.id, c.title, c.hourly_rate, c.hours, c.start_date, c.end_date, c.status
                    FROM claims c
                    INNER JOIN user_claim uc ON c.id = uc.claim_id
                    WHERE uc.user_id = @userId AND c.status = 'paid'";

                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@userId", userId);

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            Claim claim = new() {
                                ID = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                HourlyRate = reader.GetInt32(2),
                                Hours = reader.GetInt32(3),
                                StartDate = reader.GetDateTime(4),
                                EndDate = reader.GetDateTime(5),
                                Status = reader.GetString(6)
                            };
                            paidClaims.Add(claim);
                        }
                    }
                }
            }

            return paidClaims;
        }

        public Dictionary<string, List<Claim>> GetAllUsersWithClaims() {
            Dictionary<string, List<Claim>> userClaimsDict = new();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = @"
                    SELECT u.username, c.id, c.title, c.hourly_rate, c.hours, c.start_date, c.end_date, c.status
                    FROM users u
                    INNER JOIN user_claim uc ON u.id = uc.user_id
                    INNER JOIN claims c ON c.id = uc.claim_id";

                using (var command = new MySqlCommand(query, connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            string username = reader.GetString(0);

                            Claim claim = new() {
                                ID = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                HourlyRate = reader.GetInt32(3),
                                Hours = reader.GetInt32(4),
                                StartDate = reader.GetDateTime(5),
                                EndDate = reader.GetDateTime(6),
                                Status = reader.GetString(7)
                            };

                            if (!userClaimsDict.ContainsKey(username)) {
                                userClaimsDict[username] = new List<Claim>();
                            }

                            userClaimsDict[username].Add(claim);
                        }
                    }
                }
            }

            return userClaimsDict;
        }

        public ViewAllUsersModel GetAllUsers() {
            ViewAllUsersModel allUsers = new();
            allUsers.ActiveUsers = new List<User>();
            allUsers.PendingUsers = new List<PendingUsers>();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string activeUsersQuery = "SELECT id, username, is_admin FROM users";
                using (var activeCommand = new MySqlCommand(activeUsersQuery, connection)) {
                    using (var reader = activeCommand.ExecuteReader()) {
                        while (reader.Read()) {
                            User activeUser = new() {
                                ID = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                IsAdmin = reader.GetBoolean(2)
                            };
                            allUsers.ActiveUsers.Add(activeUser);
                        }
                    }
                }

                string pendingUsersQuery = "SELECT id, username FROM pending_users";
                using (var pendingCommand = new MySqlCommand(pendingUsersQuery, connection)) {
                    using (var reader = pendingCommand.ExecuteReader()) {
                        while (reader.Read()) {
                            PendingUsers pendingUser = new() {
                                ID = reader.GetInt32(0),
                                Username = reader.GetString(1)
                            };
                            allUsers.PendingUsers.Add(pendingUser);
                        }
                    }
                }
            }

            return allUsers;
        }

        public Dictionary<string, List<Claim>> GetPaidClaimsByUser() {
            Dictionary<string, List<Claim>> userClaimsMap = new Dictionary<string, List<Claim>>();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = @"
                    SELECT u.username, c.id, c.title, c.hourly_rate, c.hours, c.start_date, c.end_date, c.status
                    FROM users u
                    JOIN user_claim uc ON u.id = uc.user_id
                    JOIN claims c ON uc.claim_id = c.id
                    WHERE c.status = 'paid'";

                using (var command = new MySqlCommand(query, connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            string username = reader.GetString(0);

                            Claim claim = new Claim {
                                ID = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                HourlyRate = reader.GetInt32(3),
                                Hours = reader.GetInt32(4),
                                StartDate = reader.GetDateTime(5),
                                EndDate = reader.GetDateTime(6),
                                Status = reader.GetString(7)
                            };

                            if (!userClaimsMap.ContainsKey(username)) {
                                userClaimsMap[username] = new List<Claim>();
                            }

                            userClaimsMap[username].Add(claim);
                        }
                    }
                }
            }

            return userClaimsMap;
        }

        public Dictionary<string, List<Claim>> GetSubmittedClaimsByUser() {
            Dictionary<string, List<Claim>> userClaimsMap = new Dictionary<string, List<Claim>>();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = @"
                    SELECT u.username, c.id, c.title, c.hourly_rate, c.hours, c.start_date, c.end_date, c.status
                    FROM users u
                    JOIN user_claim uc ON u.id = uc.user_id
                    JOIN claims c ON uc.claim_id = c.id
                    WHERE c.status = 'Unpaid'";

                using (var command = new MySqlCommand(query, connection)) {
                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            // Extracting user info and claim info
                            string username = reader.GetString(0);

                            Claim claim = new Claim {
                                ID = reader.GetInt32(1),
                                Title = reader.GetString(2),
                                HourlyRate = reader.GetInt32(3),
                                Hours = reader.GetInt32(4),
                                StartDate = reader.GetDateTime(5),
                                EndDate = reader.GetDateTime(6),
                                Status = reader.GetString(7)
                            };

                            if (!userClaimsMap.ContainsKey(username)) {
                                userClaimsMap[username] = new List<Claim>();
                            }

                            userClaimsMap[username].Add(claim);
                        }
                    }
                }
            }

            return userClaimsMap;
        }

        public void ConfirmPendingUser(int pendingId) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string getPendingUserQuery = "SELECT username, password FROM pending_users WHERE id = @id";
                string username = "", password = "";
                using (var command = new MySqlCommand(getPendingUserQuery, connection)) {
                    command.Parameters.AddWithValue("@id", pendingId);
                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            username = reader.GetString(0);
                            password = reader.GetString(1);
                        }
                    }
                }

                string insertUserQuery = "INSERT INTO users (username, password, is_admin) VALUES (@username, @password, 0)";
                using (var command = new MySqlCommand(insertUserQuery, connection)) {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.ExecuteNonQuery();
                }

                string deletePendingUserQuery = "DELETE FROM pending_users WHERE id = @id";
                using (var command = new MySqlCommand(deletePendingUserQuery, connection)) {
                    command.Parameters.AddWithValue("@id", pendingId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RejectPendingUser(int pendingId) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string deletePendingUserQuery = "DELETE FROM pending_users WHERE id = @id";
                using (var command = new MySqlCommand(deletePendingUserQuery, connection)) {
                    command.Parameters.AddWithValue("@id", pendingId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void MarkClaimAsPaid(int claimId) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "UPDATE claims SET status = 'Paid' WHERE id = @claimId";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@claimId", claimId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void MarkClaimAsRejected(int claimId) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "UPDATE claims SET status = 'Rejected' WHERE id = @claimId";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@claimId", claimId);
                    command.ExecuteNonQuery();
                }
            }
        }


        public void MarkClaimAsUnpaid(int claimId) {
            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "UPDATE claims SET status = 'Unpaid' WHERE id = @claimId";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@claimId", claimId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SetDocuments(int claimId, SubmittedDocs docs) {
            if (docs.Blobs.Count != 0) {
                using (var connection = new MySqlConnection(constr)) {
                    connection.Open();

                    for (int i = 0; i < docs.Blobs.Count; i++) {
                        string query = "INSERT INTO documents (claim_id, filename, document, file_type) VALUES (@claimID, @filename, @blob, @file_type)";
                        using (var command = new MySqlCommand(query, connection)) {
                            command.Parameters.AddWithValue("@claimID", claimId);
                            command.Parameters.AddWithValue("@filename", docs.fileNames[i]);
                            command.Parameters.AddWithValue("@blob", docs.Blobs[i].Data);
                            command.Parameters.AddWithValue("@file_type", docs.fileTypes[i]);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void SetNotes(int claimId, string notes) {
            if (!string.IsNullOrEmpty(notes)) {
                using (var connection = new MySqlConnection(constr)) {
                    connection.Open();
                    string query = "INSERT INTO notes (claim_id, notes) VALUES (@claimID, @notes)";
                    using (var command = new MySqlCommand(query, connection)) {
                        command.Parameters.AddWithValue("@claimID", claimId);
                        command.Parameters.AddWithValue("@notes", notes);
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public List<Document> GetDocumentsByClaimId(int claimId) {
            var documents = new List<Document>();

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "SELECT * FROM documents WHERE claim_id = @claimId";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@claimId", claimId);

                    using (var reader = command.ExecuteReader()) {
                        while (reader.Read()) {
                            var document = new Document {
                                FileName = reader.GetString("filename"),
                                Data = (byte[])reader["document"],
                                FileType = reader.GetString("file_type")
                            };

                            documents.Add(document);
                        }
                    }
                }
            }

            return documents;
        }

        public string GetNotesByClaimId(int claimId) {
            string notes = string.Empty;

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "SELECT notes FROM notes WHERE claim_id = @claimId LIMIT 1";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@claimId", claimId);

                    var result = command.ExecuteScalar();
                    if (result != null) {
                        notes = result.ToString();
                    }
                }
            }

            return notes;
        }


        public Document GetDocumentByFileName(int claimId, string fileName) {
            Document document = null;

            using (var connection = new MySqlConnection(constr)) {
                connection.Open();

                string query = "SELECT * FROM documents WHERE claim_id = @claimId AND filename = @fileName LIMIT 1";
                using (var command = new MySqlCommand(query, connection)) {
                    command.Parameters.AddWithValue("@claimId", claimId);
                    command.Parameters.AddWithValue("@fileName", fileName);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            document = new Document {
                                FileName = reader.GetString("filename"),
                                Data = (byte[])reader["document"],
                                FileType = reader.GetString("file_type")
                            };
                        }
                    }
                }
            }

            return document;
        }

        public string GetRole(int user_id) {
            using (var con = new MySqlConnection(constr)) {
                con.Open();
                string query;
                int role_id;
                query = "SELECT user_id from user_role where user_id = @uid";
                using (var command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@uid", user_id);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            role_id = reader.GetInt32("user_id");
                        } else {
                            return string.Empty;
                        }
                    }
                }

                string role = string.Empty;
                query = "SELECT role FROM role where id = @rid";
                using (var command = new MySqlCommand(query, con)) {
                    command.Parameters.AddWithValue("@rid", role_id);

                    using (var reader = command.ExecuteReader()) {
                        if (reader.Read()) {
                            role = reader.GetString("role");
                        } else {
                            return role;
                        }
                    }
                }

                return role;
            }
        }
    }
}