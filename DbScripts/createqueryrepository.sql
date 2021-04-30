create schema GQuery

create table GQuery.QueryItem(
	Id int identity(1,1) not null,
	Name [nvarchar](256) not null,
	Label [nvarchar](256) not null,
	Description [nvarchar](2048) null,
	Parent [nvarchar](256) null,
	Pos [integer] not null default 0,
	Sql [nvarchar](2048) not null,
	constraint [pk_sysadm_tsy_queryitem] primary key clustered (id asc),
	constraint [uc_sysadm_tsy_queryitem_name] unique(Name),
	constraint [uc_sysadm_tsy_queryitem_parent_pos] unique(Name, Parent, Pos),
	foreign key (Parent) REFERENCES GQuery.QueryItem(Name)
)

