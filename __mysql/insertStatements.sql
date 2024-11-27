insert into users(username, password, is_admin) values ("Skye", "Password", true);
insert into users(username, password, is_admin) values ("John", "Password", false);
insert into users(username, password, is_admin) values ("Fred", "Password", true);

select * from users;

insert into role(role) values ("LT");
insert into role(role) values ("AD");
insert into role(role) values ("HR");

insert into user_role(user_id, role_id) values (1, 2);
insert into user_role(user_id, role_id) values (2, 1);
insert into user_role(user_id, role_id) values (3, 3);

INSERT INTO claims (title, hourly_rate, hours, start_date, end_date, status) VALUES
('Lecturing', 150, 5, '2024-01-01', '2024-12-31', 'Active'),
('Marking', 300, 6, '2024-02-01', '2024-12-31', 'Active'),
('Tutoring', 150, 8, '2024-03-01', '2024-12-31', 'Active');

INSERT INTO user_claim (user_id, claim_id) VALUES
(2, 1),
(2, 2),
(2, 3);

select * from pending_users;

select * from user_role;

select * from claims;
select * from user_claim;

select * from notes;
select * from documents;