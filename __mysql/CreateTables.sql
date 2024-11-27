create database part_2;
use part_2;

create table users(
	id int auto_increment not null primary key,
    username varchar(50) not null unique,
    password varchar(255) not null,
    is_admin boolean not null
);

create table role(
	id int auto_increment not null primary key,
    role varchar(20) not null
);

create table user_role(
	id int auto_increment not null primary key,
    user_id int not null,
    role_id int not null,
    foreign key (user_id) references users(id),
    foreign key (role_id) references role(id)
);

create table pending_users(
	id int auto_increment not null primary key,
    username varchar(50) not null,
    password varchar(255) not null
);

create table claims (
	id int auto_increment not null primary key,
    title varchar(255) not null,
    hourly_rate int not null,
    hours int not null,
    start_date date not null,
    end_date date not null,
    status varchar(20) not null
);

create table documents  (
	id int auto_increment not null primary key,
    claim_id int not null,
    filename varchar(255) not null,
    document longblob not null,
    file_type varchar(50) not null,
    foreign key (claim_id) references claims(id)
);

create table notes(
	id int auto_increment not null primary key,
    claim_id int not null,
    notes varchar(255) not null,
    foreign key (claim_id) references claims(id)
);

create table user_claim(
	id int auto_increment not null primary key,
    user_id int not null,
    claim_id int not null,
    foreign key (user_id) references users(id),
    foreign key (claim_id) references claims(id)
);