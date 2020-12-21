
drop table if exists users;
drop table if exists sessions;
drop table if exists registration_requests;
drop table if exists reset_password_requests;
drop table if exists outgoing_email_queue;



create table users
(
us_id serial primary key,
us_username varchar(20) not null,
us_email_address varchar(64) not null,
us_password varchar(48) not null default '',
us_is_admin boolean default false,
us_is_active boolean default true,
us_created_date timestamptz default CURRENT_TIMESTAMP not null
);

CREATE UNIQUE INDEX us_username_index ON users (us_username);
CREATE UNIQUE INDEX us_email_address_index ON users (us_email_address);

create table sessions
(
	se_id varchar(40) not null,
	se_timestamp timestamptz default CURRENT_TIMESTAMP not null,
	se_user int not null
);

CREATE UNIQUE INDEX se_id_index ON sessions (se_id);



create table registration_requests
(
rr_id serial primary key,
rr_guid varchar(36) not null,
rr_created_date timestamptz default CURRENT_TIMESTAMP,
rr_email_address varchar(64) not null,
rr_username varchar(20) not null,
rr_password varchar(48) not null default '',
rr_is_invitation bool not null default false,
rr_organization int not null default 0
);

create unique index rr_guid_index on registration_requests (rr_guid);

create table reset_password_requests
(
rp_guid varchar(36) not null,
rp_created_date timestamptz default CURRENT_TIMESTAMP,
rp_email_address varchar(64) not null,
rp_user_id int not null
);

create unique index rp_guid_index on reset_password_requests (rp_guid);

create table outgoing_email_queue 
(
oq_id serial primary key,
oq_date_created timestamptz default CURRENT_TIMESTAMP,
oq_email_type varchar(10) not null, /* post, registration, forgot password */
oq_post_id int null, /* if related to post - get the attachments from it, don't store twice */
oq_sending_attempt_count int not null default 0,
oq_last_sending_attempt_date timestamptz null,
oq_last_exception text not null default '',
oq_email_to text not null,
oq_email_subject text not null,
oq_email_body text not null
);

create index oq_id_index on outgoing_email_queue (oq_date_created);

/* built in user, for when the system adds issues and posts */
insert into users (us_username, us_email_address, us_is_active) 
values('system', 'dummy', false);

/* you start of with this user, but you can add other admins 
and deactiviate this */
insert into users (us_username, us_email_address, us_is_admin) 
values('admin', 'admin@example.com', true);

