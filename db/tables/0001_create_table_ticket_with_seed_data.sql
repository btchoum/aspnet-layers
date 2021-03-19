drop table if exists dbo.tickets
go
create table dbo.tickets 
(
	Id int not null identity(1, 1),
	Title nvarchar(1000) not null,
	SubmitterName nvarchar(100) not null,
	SubmitterEmail nvarchar(200) not null,
	Status varchar(30) not null, 
	CreatedAt datetime not null default getutcdate(),
	constraint pk_tickets primary key(id)
)
go

insert into tickets(Title, SubmitterName, SubmitterEmail, CreatedAt, Status)
values('first ticket', 'John Doe', 'john@example.com', '2021-03-17', 'Active')
	, ('second ticket', 'Jane Doe', 'jane@example.com', '2021-03-18', 'Pending')
