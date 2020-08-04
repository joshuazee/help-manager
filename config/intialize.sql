IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[activity_news]') AND type in (N'U'))
DROP TABLE [dbo].[activity_news]
GO
CREATE TABLE [dbo].[activity_news](
	[news_id] [int] IDENTITY(1,1) NOT NULL,
	[news_title] [varchar](200) NULL,
	[news_content] [varchar](max) NULL,
	[news_html] [varchar](max) NULL,
	[news_user] [int] NULL,
	[news_dep] [int] NULL,
	[news_type] [varchar](20) NULL,
	[news_photo] [varchar](max) NULL,
	[news_order] [int] NULL,
	[news_top] [int] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_activity_news] PRIMARY KEY CLUSTERED 
(
	[news_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[chatting_records]') AND type in (N'U'))
DROP TABLE [dbo].[chatting_records]
GO
CREATE TABLE [dbo].[chatting_records](
	[chat_id] [int] IDENTITY(1,1) NOT NULL,
	[chat_startTime] [datetime] NULL,
	[chat_endTime] [datetime] NULL,
	[chat_content] [varchar](max) NULL,
	[chat_user1] [int] NOT NULL,
	[chat_user2] [int] NOT NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_chatting_records] PRIMARY KEY CLUSTERED 
(
	[chat_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[home_page_photo]') AND type in (N'U'))
DROP TABLE [dbo].[home_page_photo]
GO
CREATE TABLE [dbo].[home_page_photo](
	[photo_id] [int] IDENTITY(1,1) NOT NULL,
	[photo_title] [varchar](100) NULL,
	[photo_url] [varchar](100) NULL,
	[photo_order] [int] NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_dep]') AND type in (N'U'))
DROP TABLE [dbo].[sys_dep]
GO
CREATE TABLE [dbo].[sys_dep](
	[dep_id] [int] IDENTITY(1,1) NOT NULL,
	[dep_code] [varchar](50) NULL,
	[dep_name] [varchar](100) NULL,
	[dep_parent] [int] NULL,
	[dep_level] [int] NULL,
	[dep_alias] [varchar](100) NULL,
	[dep_order] [int] NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_dep] PRIMARY KEY CLUSTERED 
(
	[dep_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_dic]') AND type in (N'U'))
DROP TABLE [dbo].[sys_dic]
GO
CREATE TABLE [dbo].[sys_dic](
	[dic_id] [int] IDENTITY(1,1) NOT NULL,
	[table_name] [varchar](100) NULL,
	[field_name] [varchar](100) NULL,
	[field_value] [int] NULL,
	[field_alias] [varchar](100) NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_dic] PRIMARY KEY CLUSTERED 
(
	[dic_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_error_log]') AND type in (N'U'))
DROP TABLE [dbo].[sys_error_log]
GO
CREATE TABLE [dbo].[sys_error_log](
	[error_id] [int] IDENTITY(1,1) NOT NULL,
	[error_code] [varchar](20) NULL,
	[error_message] [varchar](200) NULL,
	[error_content] [varchar](max) NULL,
	[error_stack] [varchar](max) NULL,
	[error_time] [datetime] NULL,
 CONSTRAINT [PK_sys_error_log] PRIMARY KEY CLUSTERED 
(
	[error_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_menu]') AND type in (N'U'))
DROP TABLE [dbo].[sys_menu]
GO
CREATE TABLE [dbo].[sys_menu](
	[menu_id] [int] IDENTITY(1,1) NOT NULL,
	[menu_name] [varchar](100) NULL,
	[menu_code] [varchar](50) NULL,
	[menu_url] [varchar](200) NULL,
	[menu_icon] [varchar](200) NULL,
	[menu_type] [varchar](10) NULL,
	[menu_parent] [int] NULL,
	[menu_system] [varchar](20) NULL,
	[menu_order] [int] NULL,
	[menu_config] [varchar](max) NULL,
	[menu_path] [varchar](100) NULL,
	[menu_title] [varchar](50) NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_menu] PRIMARY KEY CLUSTERED 
(
	[menu_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_operation_log]') AND type in (N'U'))
DROP TABLE [dbo].[sys_operation_log]
GO
CREATE TABLE [dbo].[sys_operation_log](
	[operation_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[operation_name] [varchar](100) NULL,
	[operation_content] [varchar](max) NULL,
	[opertaion_time] [datetime] NULL,
 CONSTRAINT [PK_sys_operation_log] PRIMARY KEY CLUSTERED 
(
	[operation_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_role]') AND type in (N'U'))
DROP TABLE [dbo].[sys_role]
GO
CREATE TABLE [dbo].[sys_role](
	[role_id] [int] IDENTITY(1,1) NOT NULL,
	[role_code] [varchar](100) NULL,
	[role_name] [varchar](200) NULL,
	[role_level] [int] NULL,
	[role_order] [int] NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_role] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_role_menu_relationship]') AND type in (N'U'))
DROP TABLE [dbo].[sys_role_menu_relationship]
GO
CREATE TABLE [dbo].[sys_role_menu_relationship](
	[role_id] [int] NOT NULL,
	[menu_id] [int] NOT NULL,
	[rm_order] [int] NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_role_menu_relationship] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC,
	[menu_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_user]') AND type in (N'U'))
DROP TABLE [dbo].[sys_user]
GO
CREATE TABLE [dbo].[sys_user](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[user_name] [varchar](100) NULL,
	[user_identity] [varchar](50) NULL,
	[user_mobile] [varchar](20) NULL,
	[user_password] [varchar](50) NULL,
	[user_parent] [int] NULL,
	[user_age] [int] NULL,
	[user_sex] [varchar](10) NULL,
	[user_photo] [varchar](100) NULL,
	[user_pin] [varchar](200) NULL,
	[user_pin_update_time] [datetime] NULL,
	[user_auth_code] [varchar](100) NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_user] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_user_dep_relationship]') AND type in (N'U'))
DROP TABLE [dbo].[sys_user_dep_relationship]
GO
CREATE TABLE [dbo].[sys_user_dep_relationship](
	[user_id] [int] NOT NULL,
	[dep_id] [int] NOT NULL,
	[ud_order] [int] NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_user_dep_relationship] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[dep_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_user_login]') AND type in (N'U'))
DROP TABLE [dbo].[sys_user_login]
GO
CREATE TABLE [dbo].[sys_user_login](
	[user_id] [int] NOT NULL,
	[login_token] [varchar](200) NULL,
	[client_id] [varchar](50) NULL,
	[creat_time] [datetime] NULL,
 CONSTRAINT [PK_sys_user_login] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_user_role_relationship]') AND type in (N'U'))
DROP TABLE [dbo].[sys_user_role_relationship]
GO
CREATE TABLE [dbo].[sys_user_role_relationship](
	[user_id] [int] NOT NULL,
	[role_id] [int] NOT NULL,
	[ur_order] [int] NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_user_role_relationship] PRIMARY KEY CLUSTERED 
(
	[user_id] ASC,
	[role_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[urinalysis_records]') AND type in (N'U'))
DROP TABLE [dbo].[urinalysis_records]
GO
CREATE TABLE [dbo].[urinalysis_records](
	[urinalysis_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id1] [int] NULL,
	[user_id2] [int] NULL,
	[urinalysis_result] [varchar](20) NULL,
	[urinalysis_remark] [varchar](max) NULL,
	[urinalysis_photo] [varchar](max) NULL,
	[urinalysis_state] [varchar](20) NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_urinalysis_records] PRIMARY KEY CLUSTERED 
(
	[urinalysis_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[user_sign]') AND type in (N'U'))
DROP TABLE [dbo].[user_sign]
GO
CREATE TABLE [dbo].[user_sign](
	[sign_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id1] [int] NULL,
	[sign_location1] [varchar](200) NULL,
	[sign_address1] [varchar](200) NULL,
	[sign_time1] [datetime] NULL,
	[sign_photo] [varchar](200) NULL,
	[sign_type] [varchar](50) NULL,
	[sign_state] [varchar](50) NULL,
	[sign_remark] [varchar](500) NULL,
	[user_id2] [int] NULL,
	[sign_location2] [varchar](200) NULL,
	[sign_address2] [varchar](200) NULL,
	[sign_time2] [datetime] NULL,
	[sign_appointment] [varchar](200) NULL,
	[sign_appointment_time] [datetime] NULL,
	[sign_distance] [float] NULL,
	[create_time] [datetime] NULL,
	[update_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_user_signIn] PRIMARY KEY CLUSTERED 
(
	[sign_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[user_trace_point]') AND type in (N'U'))
DROP TABLE [dbo].[user_trace_point]
GO
CREATE TABLE [dbo].[user_trace_point](
	[point_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NULL,
	[point_x] [float] NULL,
	[point_y] [float] NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_user_trace_point] PRIMARY KEY CLUSTERED 
(
	[point_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[video_records]    Script Date: 02/13/2019 17:19:42 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[video_records]') AND type in (N'U'))
DROP TABLE [dbo].[video_records]
GO
CREATE TABLE [dbo].[video_records](
	[video_id] [int] IDENTITY(1,1) NOT NULL,
	[video_name] [varchar](100) NULL,
	[video_uploader] [varchar](100) NULL,
	[video_dep] [int] NULL,
	[video_url] [varchar](max) NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[app_version_records]') AND type in (N'U'))
DROP TABLE [dbo].[app_version_records]
GO
CREATE TABLE [dbo].[app_version_records](
	[version_id] [int] IDENTITY(1,1) NOT NULL,
	[version_code] [varchar](100) NULL,
	[version_type] [varchar](20) NULL,
	[version_description] [varchar](max) NULL,
	[create_time] [datetime] NULL
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[electronic_fence]') AND type in (N'U'))
DROP TABLE [dbo].[electronic_fence]
GO
CREATE TABLE [dbo].[electronic_fence](
	[fence_id] [int] IDENTITY(1,1) NOT NULL,
	[fence_name] [varchar](100) NULL,
	[fence_type] [varchar](20) NULL,
	[fence_extent] [varchar](max) NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_electronic_fence] PRIMARY KEY CLUSTERED 
(
	[fence_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dep_fence_relationship]') AND type in (N'U'))
DROP TABLE [dbo].[dep_fence_relationship]
GO
CREATE TABLE [dbo].[dep_fence_relationship](
	[dep_id] [int] NOT NULL,
	[fence_id] [int] NOT NULL,
	[create_time] [datetime] NULL,
 CONSTRAINT [PK_dep_fence_relationship] PRIMARY KEY CLUSTERED 
(
	[dep_id] ASC,
	[fence_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sys_role_dep_relationship]') AND type in (N'U'))
DROP TABLE [dbo].[sys_role_dep_relationship]
GO
CREATE TABLE [dbo].[sys_role_dep_relationship](
	[role_id] [int] NOT NULL,
	[dep_id] [int] NOT NULL,
	[create_time] [datetime] NULL,
	[delete_mark] [int] NULL,
 CONSTRAINT [PK_sys_role_dep_relationship] PRIMARY KEY CLUSTERED 
(
	[role_id] ASC,
	[dep_id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserRoleRelationship]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateUserRoleRelationship]
GO
CREATE PROCEDURE UpdateUserRoleRelationship
@userid int, @roleid int
AS
BEGIN
	delete from sys_user_role_relationship where user_id = @userid
	
	insert into sys_user_role_relationship(user_id, role_id, create_time, delete_mark)
	values(@userid, @roleid, GETDATE(), 0)
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserDepRelationship]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateUserDepRelationship]
GO
CREATE PROCEDURE UpdateUserDepRelationship
@userid int, @depid int
AS
BEGIN
	delete from sys_user_dep_relationship where user_id = @userid
	
	insert into sys_user_dep_relationship(user_id, dep_id, create_time, delete_mark)
	values(@userid, @depid, GETDATE(), 0)
END
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TranslateUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[TranslateUser]
GO
CREATE PROCEDURE [dbo].[TranslateUser] @userid int, @depid int, @adminid int
AS
BEGIN
	set nocount on;
	declare @message varchar(500) = '';
	begin try
		begin tran
		 delete from sys_user_dep_relationship where user_id = @userid
		 insert into sys_user_dep_relationship(user_id, dep_id, create_time, delete_mark) values(@userid, @depid, GETDATE(), 0)
		 if @adminid != 0
		 begin
			update sys_user set user_parent = @adminid where user_id = @userid
		end
		commit tran
	end try
	begin catch
		select ERROR_LINE() as 错误行数, ERROR_MESSAGE() as 错误消息
		if @@TRANCOUNT > 0
		begin
			rollback tran;
		end
		set @message = @@ERROR;
	end catch
	select @message
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteRole]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteRole]
GO
CREATE proc [dbo].[DeleteRole]
	@role_id int
as
begin
	declare @count int;
	declare @message varchar(500) = '';
	select @count = count(*) from sys_user_role_relationship where delete_mark = 0 and role_id = @role_id
	
	if(@count = 0)
	begin
		begin try
			begin tran
				update sys_role_menu_relationship set delete_mark = 1 where delete_mark = 0 and role_id = @role_id;

				update sys_role set delete_mark = 1 where delete_mark = 0 and role_id = @role_id;
			commit tran
		end try
		begin catch
			if @@TRANCOUNT > 0
			begin
				rollback tran
			end
		end catch
	end
	else
	begin
		set @message = '该角色下还有关联用户，无法删除';
	end
	
	select @message;
end
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteUser]
GO
CREATE proc [dbo].[DeleteUser]
	@user_id int
as
begin
	declare @message varchar(500) = '';
	declare @count int = 0;
	select @count = COUNT(*) from sys_user where user_parent = @user_id and delete_mark = 0;
	if @count > 0
	begin
		set @message = '该用户还有下属用户存在，无法删除';
	end
	else
	begin
		begin try
			begin tran
				delete from sys_user_role_relationship where delete_mark = 0 and user_id = @user_id;
				
				delete from sys_user_dep_relationship where delete_mark = 0 and user_id = @user_id;
				
				update sys_user set delete_mark = 1 where delete_mark = 0 and user_id = @user_id;
			commit tran
		end try
		begin catch
			if @@TRANCOUNT > 0
			begin
				rollback tran;
			end
			set @message = @@ERROR;
		end catch
	end
	select @message;
end
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteDep]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteDep]
GO
CREATE proc [dbo].[DeleteDep]
	@dep_id int
as
begin
	declare @count int = 0;
	declare @message varchar(500) = '';
	begin try
		select @count = COUNT(*) from sys_user_dep_relationship where delete_mark = 0 and dep_id = @dep_id;
		if @count = 0
		begin
			select @count = COUNT(*) from sys_dep where delete_mark = 0 and dep_parent = @dep_id;
			if @count = 0
			begin
				begin tran
					update sys_dep set delete_mark = 1 where delete_mark = 0 and dep_id = @dep_id;
				commit tran
			end
			else
			begin
				set @message = '该部门下还有' + CONVERT(varchar(100), @count) + '个子部门，无法删除';
			end
		end
		else
		begin
			set @message = '该部门下还有' + CONVERT(varchar(100), @count) + '个用户，无法删除';
		end
	end try
	begin catch
		select ERROR_LINE() as 错误行数, ERROR_MESSAGE() as 错误消息
		if @@TRANCOUNT > 0
		begin
			rollback tran;
		end
		set @message = @@ERROR;
	end catch
	select @message;
end
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ValidUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[ValidUser]
GO
CREATE proc [dbo].[ValidUser]
	@mobile varchar(20),@pin varchar(100),@ilevel int
as
begin
	declare @count int;
	declare @pin_temp varchar(100);
	declare @message varchar(500) = '';
	declare @id int = 0;
	declare @level int = 0;
	select @count = COUNT(*) from sys_user where delete_mark = 0 and user_mobile = @mobile
	
	if(@count = 1)
	begin
		select @level = r.role_level from sys_user u,sys_role r,sys_user_role_relationship t 
		where u.user_id = t.user_id and t.role_id = r.role_id and u.delete_mark = 0 and user_mobile = @mobile
		
		if(@level = @ilevel)
		begin
			select @pin_temp = user_pin from sys_user where delete_mark = 0 and user_mobile = @mobile
		
			if(@pin_temp = '' or @pin_temp is null)
			begin
				update sys_user set user_pin = @pin where user_mobile = @mobile and delete_mark = 0;
			end
			else if(@pin_temp != @pin)
			begin
				set @message = '换设备登录请点击申请更换设备';
			end
			
			select @id = user_id from sys_user where delete_mark = 0 and user_mobile = @mobile and user_pin = @pin;
		end
		else
		begin
			select @id = USER_ID from sys_user where delete_mark = 0 and user_mobile = @mobile
		end
	end
	else
	begin
		set @message = '不存在该用户，请到相关社区进行注册';
	end
	
	select @message,@id;
end
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QuerySubUserByUserID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QuerySubUserByUserID]
GO
CREATE proc [dbo].[QuerySubUserByUserID]
	@user_id int,@role_level int
as
begin
	declare @dep_id int;
	declare @t table
	( 
		[user_id] [int] NOT NULL,
		[user_name] [varchar](100) NULL,
		[user_identity] [varchar](50) NULL,
		[user_mobile] [varchar](20) NULL,
		[user_password] [varchar](50) NULL,
		[user_parent] [int] NULL,
		[user_age] [int] NULL,
		[user_sex] [varchar](10) NULL,
		[user_photo] [varchar](100) NULL,
		[user_pin] [varchar](200) NULL,
		[user_pin_update_time] [datetime] NULL,
		[user_auth_code] [varchar](100) NULL,
		[create_time] [datetime] NULL,
		[delete_mark] [int] NULL
	);
	
	declare @t2 table
	(
		[dep_id] [int] not null
	);
	
	declare @t3 table
	(
		[dep_id] [int] not null,
		[dep_parent] [int] null
	);
	
	if @user_id != 1
	begin
		insert into @t2 select d.dep_id from sys_user u
		left join sys_user_dep_relationship t1 on u.user_id = t1.user_id and t1.delete_mark = 0
		left join sys_dep d on t1.dep_id = d.dep_id and t1.delete_mark = 0 and d.delete_mark = 0
		where u.user_id = @user_id;
		
		declare cur123 cursor for select * FROM @t2;
		open cur123;
		fetch next from cur123 into @dep_id;
		while @@fetch_status = 0
		begin
			insert into @t3 select distinct * from f_GetAllChildrenDepById(@dep_id);
			
			insert into @t select distinct u.* from sys_user u
			left join sys_user_dep_relationship t1 on u.user_id = t1.user_id and t1.delete_mark = 0
			left join sys_dep d on t1.dep_id = d.dep_id and t1.delete_mark = 0 and d.delete_mark = 0
			left join sys_user_role_relationship t2 on u.user_id = t2.user_id and t2.delete_mark = 0
			left join sys_role r on t2.role_id = r.role_id and t2.delete_mark = 0 and r.delete_mark = 0
			where r.role_level = @role_level and d.dep_id in (select dep_id from @t3);
			
			fetch next from cur123 into @dep_id;
		end
		close cur123;
		deallocate cur123;
	end
	else
	begin
		insert into @t select distinct u.* from sys_user u
		left join sys_user_role_relationship t2 on u.user_id = t2.user_id and t2.delete_mark = 0
		left join sys_role r on t2.role_id = r.role_id and r.delete_mark = 0
		where r.role_level = @role_level and u.delete_mark = 0
	end
	
	select distinct * from @t;
end
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreateSegmentTable]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CreateSegmentTable]
GO
CREATE PROCEDURE [dbo].[CreateSegmentTable]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	declare @tableName1 varchar(100) = 'sys_error_log_' + convert(varchar(10), YEAR(GETDATE()));
	declare @tableName2 varchar(200) = 'user_trace_point_' + convert(varchar(10), YEAR(GETDATE()));
	declare @sql varchar(MAX);
	
	if MONTH(GETDATE()) >= 10
	begin
		set @tableName1 += convert(varchar(2), MONTH(GETDATE()));
		set @tableName2 += convert(varchar(2), MONTH(GETDATE()));
	end
	else
	begin
		set @tableName1 += '0' + convert(varchar(1), MONTH(GETDATE()));
		set @tableName2 += '0' + convert(varchar(2), MONTH(GETDATE()));
	end
	
	begin try
	begin tran
	set @sql = 'CREATE TABLE ' + @tableName1 + '(' +
				'[error_id] [int] IDENTITY(1,1) NOT NULL,' +
				'[error_code] [varchar](20) NULL,' +
				'[error_message] [varchar](200) NULL,' +
				'[error_content] [varchar](max) NULL,' +
				'[error_stack] [varchar](max) NULL,' +
				'[error_time] [datetime] NULL,' +
			   'CONSTRAINT [PK_' + @tableName1 + '] PRIMARY KEY CLUSTERED ' +
			   '(' +
				'[error_id] ASC' +
			   ')WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]' +
			   ') ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]';
	exec(@sql);
	set @sql = 'CREATE TABLE ' + @tableName2 + '(' +
				'[point_id] [int] IDENTITY(1,1) NOT NULL,' +
				'[user_id] [int] NULL,' +
				'[point_x] [float] NULL,' +
				'[point_y] [float] NULL,' +
				'[create_time] [datetime] NULL,' +
				'[delete_mark] [int] NULL,' +
			   'CONSTRAINT [PK_' + @tableName2 + '] PRIMARY KEY CLUSTERED ' +
			   '(' +
				'[point_id] ASC' +
			   ')WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]' +
			   ') ON [PRIMARY]';
	exec(@sql);
	commit tran;
	end try
	begin catch
		if @@TRANCOUNT > 0
		begin
			rollback tran
		end
		select ERROR_MESSAGE()
	end catch
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryRegionByDepID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryRegionByDepID]
GO
CREATE PROCEDURE [dbo].[QueryRegionByDepID]
	@dep_id int
AS
BEGIN
	declare @code varchar(50);
	declare @parent int;
	
	select @code = dep_code, @parent = dep_parent from sys_dep where dep_id = @dep_id and delete_mark = 0
	
	while (@code is null or @code = '') and @parent != 0
	begin
		select @code = dep_code, @parent = dep_parent, @dep_id = dep_id from sys_dep where dep_id = @parent and delete_mark = 0
	end
	
	select dep_id, dep_code, dep_name, dep_parent from sys_dep where dep_id = @dep_id and delete_mark = 0
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QuerySubRegionByDepID]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QuerySubRegionByDepID]
GO
CREATE PROCEDURE [QuerySubRegionByDepID]
	@dep_id int
AS
BEGIN
	declare @dep_code varchar(50);
	select @dep_code = dep_code from sys_dep where dep_id = @dep_id and delete_mark = 0
	
	if(@dep_code is not null and @dep_code != '')
	begin
		select dep_name, dep_code, dep_alias from sys_dep where dep_parent = @dep_id and delete_mark = 0 and dep_code is not null and dep_code != ''
	end
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StatSubRegionData]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StatSubRegionData]
GO
CREATE PROCEDURE [dbo].[StatSubRegionData]
	@dep_parent int, @role_level1 int, @role_level2 int, @role_level3 int
AS
BEGIN
	create table #temp
	(
		dep_name varchar(100),
		dep_alias varchar(100),
		children_count int,
		user_count_all int,
		user_count1 int,
		user_count2 int,
		user_count3 int
	)
	declare @dep_id int
	declare @dep_count int
	declare @dep_alias varchar(100)
	declare @dep_name varchar(100)
	declare @user_count int
	declare @user_count1 int
	declare @user_count2 int
	declare @user_count3 int
	
	declare myCur cursor for select dep_id, dep_name, dep_alias from sys_dep where dep_parent = @dep_parent and delete_mark = 0
	
	open myCur
	
	fetch next from myCur
	into @dep_id,@dep_name,@dep_alias
	
	while @@FETCH_STATUS = 0
	begin
		select @dep_count = COUNT(*) from sys_dep where dep_parent = @dep_id and delete_mark = 0
		
		select @user_count = count(*) 
		from sys_dep d, sys_user u, sys_user_dep_relationship t
		where d.dep_id = t.dep_id and u.user_id = t.user_id
		and u.delete_mark = 0 and d.dep_id in (select dep_id from sys_dep where dep_parent = @dep_id and delete_mark = 0)
		
		select @user_count1 = count(*) 
		from sys_dep d, sys_user u, sys_user_dep_relationship t, sys_role r, sys_user_role_relationship t2
		where d.dep_id = t.dep_id and u.user_id = t.user_id and r.role_id = t2.role_id and u.user_id = t2.user_id and r.role_level = @role_level1
		and u.delete_mark = 0 and d.dep_id in (select dep_id from sys_dep where dep_parent = @dep_id and delete_mark = 0)
		
		select @user_count2 = count(*) 
		from sys_dep d, sys_user u, sys_user_dep_relationship t, sys_role r, sys_user_role_relationship t2
		where d.dep_id = t.dep_id and u.user_id = t.user_id and r.role_id = t2.role_id and u.user_id = t2.user_id and r.role_level = @role_level2
		and u.delete_mark = 0 and d.dep_id in (select dep_id from sys_dep where dep_parent = @dep_id and delete_mark = 0)
		
		select @user_count3 = count(*) 
		from sys_dep d, sys_user u, sys_user_dep_relationship t, sys_role r, sys_user_role_relationship t2
		where d.dep_id = t.dep_id and u.user_id = t.user_id and r.role_id = t2.role_id and u.user_id = t2.user_id and r.role_level = @role_level3
		and u.delete_mark = 0 and d.dep_id in (select dep_id from sys_dep where dep_parent = @dep_id and delete_mark = 0)
		
		insert into #temp values(@dep_name,@dep_alias,@dep_count,@user_count,@user_count1,@user_count2,@user_count3)
		
		fetch next from myCur
		into @dep_id,@dep_name,@dep_alias
	end
	close myCur
	deallocate myCur
	
	select * from #temp
END
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[QueryNewsPermission]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[QueryNewsPermission]
GO
create proc QueryNewsPermission
@dep int
as
declare @dParent int
select @dParent = dep_parent from sys_dep where dep_id = @dep;
with t as(
select * from sys_dep where dep_id = @dep
union all
select d.* from t, sys_dep d where t.dep_parent = d.dep_id
)select dep_id from t union select dep_id from sys_dep where dep_parent = @dParent
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateRoleDepRelationship]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateRoleDepRelationship]
GO
CREATE proc [dbo].[UpdateRoleDepRelationship]
@role int, @deps varchar(max)
as
	declare @dep int
	if @deps is not null and @deps != ''
	begin
		declare mycursor cursor for select TextValue from f_StringSplit(@deps, ',')
		delete from sys_role_dep_relationship where role_id = @role
		
		open mycursor
		fetch next from mycursor into @dep
		while @@FETCH_STATUS = 0
		begin
			insert into sys_role_dep_relationship values(@role, @dep, GETDATE(), 0)
			fetch next from mycursor into @dep
		end
		close mycursor
		deallocate mycursor
	end
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StatLevel1DepSignInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StatLevel1DepSignInfo]
GO
CREATE proc [dbo].[StatLevel1DepSignInfo]
@depids varchar(max), @starttime datetime, @endtime datetime
as

create table #dep_tmp
(
	dep_id int,
	dep_name varchar(100),
	dep_alias varchar(100)
)

declare @depid int
declare @depname varchar(100)
declare @depalias varchar(100)
declare @name varchar(100)

insert into #dep_tmp
select dep_id, dep_name, dep_alias
from sys_dep, f_StringSplit(@depids, ',')
where convert(int, TextValue) = dep_id

declare @count int
select @count = COUNT(*) from #dep_tmp

if @count = 1
begin
	create table #user_result_tmp
	(
		user_name varchar(100),
		total int,
		signed int,
		unsigned int,
		checked int,
		unchecked int
	)
	declare @userid int
	declare @username varchar(100)
	
	select @depid = dep_id, @depname = dep_name from #dep_tmp
	declare usercursor cursor local for
	with d as(
	select * from sys_dep where dep_id = @depid
	union all
	select a.* from sys_dep a inner join d b on a.dep_parent = b.dep_id
	)
	select u.user_id,u.user_name from sys_user u, d, sys_user_dep_relationship t
	where u.user_id = t.user_id and t.dep_id = d.dep_id and u.delete_mark = 0
	--select u.user_id,u.user_name from sys_user u, sys_dep d, sys_user_dep_relationship t
	--where u.user_id = t.user_id and t.dep_id = d.dep_id and u.delete_mark = 0 and d.dep_id = @depid

	open usercursor

	fetch next from usercursor into @userid, @username

	while @@FETCH_STATUS = 0
	begin
		insert into #user_result_tmp
		select @username, COUNT(*) total, COUNT(sign_time1) signed,COUNT(*) - COUNT(sign_time1) unsigned, COUNT(sign_time2) checked, COUNT(*) - COUNT(sign_time2) unchecked
		from user_sign 
		where user_id1 = @userid and create_time >= @starttime and create_time <= @endtime
		
		fetch next from usercursor into @userid, @username
	end
	close usercursor
	deallocate usercursor

	select * from #user_result_tmp

	drop table #user_result_tmp
end
else if @count > 1
begin
	create table #dep_result_tmp
	(
		dep_name varchar(100),
		total int,
		signed int,
		unsigned int,
		checked int,
		unchecked int
	)
	declare deptmpcursor cursor local for select * from #dep_tmp
	
	open deptmpcursor
	
	fetch next from deptmpcursor into @depid, @depname, @depalias
	while @@FETCH_STATUS = 0
	begin
		if @depalias is not null and @depalias != ''
		begin
			set @name = @depalias
		end
		else
		begin
			set @name = @depname
		end
		
		;with d as
		(
		select * from sys_dep where dep_id = @depid
		union all
		select a.* from sys_dep a inner join d b on a.dep_parent = b.dep_id
		)
		insert into #dep_result_tmp
		select @name, COUNT(*) total, COUNT(sign_time1) signed,COUNT(*) - COUNT(sign_time1) unsigned, COUNT(sign_time2) checked, COUNT(*) - COUNT(sign_time2) unchecked
		from user_sign
		where user_id1 in (select u.user_id from d, sys_user_dep_relationship t, sys_user u where t.dep_id = d.dep_id and t.user_id = u.user_id and u.delete_mark = 0)
		and create_time >= @starttime and create_time <= @endtime
		
		fetch next from deptmpcursor into @depid, @depname, @depalias
	end
	close deptmpcursor
	deallocate deptmpcursor
	
	select * from #dep_result_tmp
	
	drop table #dep_result_tmp
end
else
begin
	RAISERROR('无效参数', 16, 1)
end
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StatParentDepSignInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[StatParentDepSignInfo]
GO
CREATE proc [dbo].[StatParentDepSignInfo]
@regionCode varchar(100), @starttime datetime, @endtime datetime
as

create table #grandchild_tmp
(
	dep_id int,
	dep_name varchar(100),
	dep_level int
)

create table #stat_tmp
(
	name varchar(100),
	total int,
	signed int,
	unsigned int,
	checked int,
	unchecked int
)

create table #result_tmp
(
	dep_name varchar(100),
	dep_code varchar(100),
	total int,
	signed int,
	unsigned int,
	checked int,
	unchecked int
)

declare @parent_id int
declare @childid int
declare @childname varchar(100)
declare @childalias varchar(100)
declare @childlevel int
declare @childcode varchar(100)
declare @name varchar(100)
declare @childcount int

declare @lowestlevel int = 1
declare @currentsigned int
declare @currrentunchecked int

declare @grandchildrenid varchar(max)

select @parent_id = dep_id from sys_dep where dep_code = @regionCode and delete_mark = 0
declare depcursor cursor local for select dep_id, dep_name, dep_alias, dep_level, dep_code from sys_dep where dep_parent = @parent_id and delete_mark = 0

open depcursor

fetch next from depcursor into @childid, @childname, @childalias, @childlevel, @childcode

while @@FETCH_STATUS = 0
begin
	if @childlevel > 1
	begin
		with temp as
		(
			select dep_id,dep_name,dep_level from sys_dep where dep_parent = @childid
			union all
			select d.dep_id,d.dep_name,d.dep_level from temp inner join sys_dep d
			on temp.dep_id=d.dep_parent
		)
		insert into #grandchild_tmp select dep_id,dep_name,dep_level from temp where dep_level = @lowestlevel order by dep_level
		
		select @childcount = COUNT(*) from #grandchild_tmp
		if @childcount > 0
		begin
			select @grandchildrenid = stuff((select ',' + convert(varchar(10), dep_id) from #grandchild_tmp for xml path('')), 1, 1, '')
			
			insert into #stat_tmp exec StatLevel1DepSignInfo @grandchildrenid,@starttime,@endtime
		end
		else
		begin
			if @childalias is not null and @childalias != ''
			begin
				set @name = @childalias
			end
			else
			begin
				set @name = @childname
			end
			insert into #stat_tmp values(@name, 0,0,0,0,0)
		end
	end
	else
	begin
		insert into #stat_tmp exec StatLevel1DepSignInfo @childid,@starttime,@endtime
	end
	
	if @childalias is not null and @childalias != ''
	begin
		set @name = @childalias
	end
	else
	begin
		set @name = @childname
	end
	insert into #result_tmp select @name, @childcode,SUM(total), SUM(signed), SUM(unsigned), SUM(checked), SUM(unchecked) from #stat_tmp
	
	delete from #stat_tmp
	
	delete from #grandchild_tmp
	
	fetch next from depcursor into @childid, @childname, @childalias, @childlevel, @childcode
end
select * from #result_tmp

drop table #stat_tmp
drop table #result_tmp
drop table #grandchild_tmp
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CountLevel1DepUrinalysis]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CountLevel1DepUrinalysis]
GO
CREATE proc [dbo].[CountLevel1DepUrinalysis]
@depids varchar(max), @starttime datetime, @endtime datetime
as

create table #dep_tmp
(
	dep_id int,
	dep_name varchar(100),
	dep_alias varchar(100)
)

declare @depid int
declare @depname varchar(100)
declare @depalias varchar(100)
declare @name varchar(100)

insert into #dep_tmp
select dep_id, dep_name, dep_alias
from sys_dep, f_StringSplit(@depids, ',')
where convert(int, TextValue) = dep_id

declare @count int
select @count = COUNT(*) from #dep_tmp

if @count = 1
begin
	create table #user_result_tmp
	(
		user_name varchar(100),
		total int,
		checked int,
		unchecked int
	)
	declare @userid int
	declare @username varchar(100)
	
	select @depid = dep_id, @depname = dep_name from #dep_tmp
	
	declare usercursor cursor local for 
	with d as(
	select * from sys_dep where dep_id = @depid
	union all
	select a.* from sys_dep a inner join d b on a.dep_parent = b.dep_id
	)
	select u.user_id,u.user_name from sys_user u, d, sys_user_dep_relationship t
	where u.user_id = t.user_id and t.dep_id = d.dep_id and u.delete_mark = 0
	--select u.user_id,u.user_name from sys_user u, sys_dep d, sys_user_dep_relationship t
	--where u.user_id = t.user_id and t.dep_id = d.dep_id and u.delete_mark = 0 and d.dep_id = @depid

	open usercursor

	fetch next from usercursor into @userid, @username

	while @@FETCH_STATUS = 0
	begin
		insert into #user_result_tmp
		select @username, COUNT(*) total, COUNT(update_time) checked, COUNT(*) - COUNT(update_time) unchecked
		from urinalysis_records 
		where user_id1 = @userid and create_time >= @starttime and create_time <= @endtime
		
		fetch next from usercursor into @userid, @username
	end
	close usercursor
	deallocate usercursor

	select * from #user_result_tmp

	drop table #user_result_tmp
end
else if @count > 1
begin
	create table #dep_result_tmp
	(
		dep_name varchar(100),
		total int,
		checked int,
		unchecked int
	)
	declare deptmpcursor cursor local for select * from #dep_tmp
	
	open deptmpcursor
	
	fetch next from deptmpcursor into @depid, @depname, @depalias
	while @@FETCH_STATUS = 0
	begin
		if @depalias is not null and @depalias != ''
		begin
			set @name = @depalias;
		end
		else
		begin
			set @name = @depname;
		end
		
		;with d as
		(
		select * from sys_dep where dep_id = @depid
		union all
		select a.* from sys_dep a inner join d b on a.dep_parent = b.dep_id
		)
		insert into #dep_result_tmp
		select @name, COUNT(*) total, COUNT(update_time) checked, COUNT(*) - COUNT(update_time) unchecked
		from urinalysis_records
		where user_id1 in (select u.user_id from d, sys_user_dep_relationship t, sys_user u where t.dep_id = d.dep_id and t.user_id = u.user_id and u.delete_mark = 0)
		and create_time >= @starttime and create_time <= @endtime
		
		fetch next from deptmpcursor into @depid, @depname, @depalias
	end
	close deptmpcursor
	deallocate deptmpcursor
	
	select * from #dep_result_tmp
	
	drop table #dep_result_tmp
end
else
begin
	RAISERROR('无效参数', 16, 1)
end
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CountParentDepUrinalysis]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CountParentDepUrinalysis]
GO
CREATE proc [dbo].[CountParentDepUrinalysis]
@regionCode varchar(100), @starttime datetime, @endtime datetime
as

create table #grandchild_tmp
(
	dep_id int,
	dep_name varchar(100),
	dep_level int
)

create table #stat_tmp
(
	name varchar(100),
	total int,
	checked int,
	unchecked int
)

create table #result_tmp
(
	dep_name varchar(100),
	code varchar(100),
	total int,
	checked int,
	unchecked int
)

declare @parent_id int
declare @childid int
declare @childname varchar(100)
declare @childalias varchar(100)
declare @childlevel int
declare @childcode varchar(100)
declare @name varchar(100)
declare @childcount int

declare @lowestlevel int = 1
declare @currentsigned int
declare @currrentunchecked int

declare @grandchildrenid varchar(max)

select @parent_id = dep_id from sys_dep where dep_code = @regionCode and delete_mark = 0
declare depcursor cursor local for select dep_id, dep_name, dep_alias, dep_level, dep_code from sys_dep where dep_parent = @parent_id and delete_mark = 0

open depcursor

fetch next from depcursor into @childid, @childname, @childalias, @childlevel, @childcode

while @@FETCH_STATUS = 0
begin
	if @childlevel > 1
	begin
		with temp as
		(
			select dep_id,dep_name,dep_level from sys_dep where dep_parent = @childid
			union all
			select d.dep_id,d.dep_name,d.dep_level from temp inner join sys_dep d
			on temp.dep_id=d.dep_parent
		)
		insert into #grandchild_tmp select dep_id,dep_name,dep_level from temp where dep_level = @lowestlevel order by dep_level
		
		select @childcount = COUNT(*) from #grandchild_tmp
		
		if @childcount > 0
		begin
			select @grandchildrenid = stuff((select ',' + convert(varchar(10), dep_id) from #grandchild_tmp for xml path('')), 1, 1, '')
		
			insert into #stat_tmp exec CountLevel1DepUrinalysis @grandchildrenid,@starttime,@endtime
		end
		else
		begin
			if @childalias is not null and @childalias != ''
			begin
				set @name = @childalias
			end
			else
			begin
				set @name = @childname
			end
			insert into #stat_tmp values(@name, 0, 0, 0)
		end
	end
	else
	begin
		insert into #stat_tmp exec CountLevel1DepUrinalysis @childid,@starttime,@endtime
	end
	
	if @childalias is not null and @childalias != ''
	begin
		set @name = @childalias
	end
	else
	begin
		set @name = @childname
	end
	insert into #result_tmp select @name, @childcode, SUM(total), SUM(checked), SUM(unchecked) from #stat_tmp
	
	delete from #stat_tmp
	
	delete from #grandchild_tmp
	
	fetch next from depcursor into @childid, @childname, @childalias, @childlevel, @childcode
end
select * from #result_tmp

drop table #stat_tmp
drop table #result_tmp
drop table #grandchild_tmp
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CountActiveUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[CountActiveUser]
GO
create proc [dbo].[CountActiveUser]
@depparent varchar(100), @starttime date, @endtime date, @rolelevel1 int
as
begin
	create table #stat_result
	(
		name varchar(100),
		code varchar(100),
		community_count int,
		total_user_count int,
		active_user_count int
	)
	create table #dep_temp
	(
		dep_id int,
		dep_level int
	)
	create table #user_temp
	(
		user_id int
	)
	declare @parent_id int
	declare @dep_id int
	declare @dep_count int
	declare @dep_alias varchar(100)
	declare @dep_name varchar(100)
	declare @dep_code varchar(100)
	declare @name varchar(100)
	declare @community_count varchar(100)
	declare @total_user_count int
	declare @active_user_count int
	declare @lowestlevel int = 1
	
	select @parent_id = dep_id from sys_dep where dep_code = @depparent and delete_mark = 0
	declare depCursor cursor for select dep_id, dep_name, dep_alias, dep_code from sys_dep where dep_parent = @parent_id and delete_mark = 0
	
	open depCursor
	
	fetch next from depCursor
	into @dep_id,@dep_name,@dep_alias, @dep_code
	
	while @@FETCH_STATUS = 0
	begin
		if @dep_alias is not null and @dep_alias != ''
		begin
			set @name = @dep_alias
		end
		else
		begin
			set @name = @dep_name
		end
		
		
		--先查询出该部门下所有社区ID
		;with d as
		(
			select dep_id, dep_level from sys_dep where dep_id = @dep_id
			union all
			select a.dep_id, a.dep_level from sys_dep a,d where a.dep_parent = d.dep_id
		)
		insert into #dep_temp select dep_id, dep_level from d where dep_level <= @lowestlevel
		
		select @community_count = count(*) from #dep_temp where dep_level = @lowestlevel
		
		--查询社区中所有重点人员ID
		insert into #user_temp select u.user_id from sys_user u, sys_user_dep_relationship t1, #dep_temp d, sys_user_role_relationship t2, sys_role r
		where u.user_id = t1.user_id and t1.dep_id = d.dep_id and u.user_id = t2.user_id and r.role_id = t2.role_id and u.delete_mark = 0 and r.role_level = @rolelevel1
		
		select @total_user_count = count(*) from #user_temp
		
		--查询活跃用户数量(近一个月以内有过签到操作的人员视作活跃用户)
		select @active_user_count = count(*) from (select user_id1, COUNT(user_id1) sign_count from user_sign us, #user_temp ut where us.user_id1 = ut.user_id and us.delete_mark = 0 and us.sign_time2 > @starttime and us.sign_time2 < @endtime
		group by user_id1) t
		
		insert into #stat_result values(@name, @dep_code, @community_count, @total_user_count, @active_user_count)
		
		delete from #dep_temp
		delete from #user_temp
		fetch next from depCursor
		into @dep_id,@dep_name,@dep_alias, @dep_code
	end
	
	close depCursor
	deallocate depCursor
	
	select * from #stat_result
end

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[f_StringSplit]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[f_StringSplit]
GO
CREATE FUNCTION [dbo].[f_StringSplit]
(
    @Text NVARCHAR(MAX),
    @Sign NVARCHAR(MAX)
)
RETURNS
@ResultTable TABLE
(
    ID INT IDENTITY(1,1) PRIMARY KEY,
    TextValue NVARCHAR(1024)
)
AS
BEGIN
    DECLARE @StartIndex INT
    DECLARE @FindIndex  INT
    DECLARE @Content VARCHAR(4000)

    -- 和函数CHARINDEX有关CHARINDEX
    SET @StartIndex = 1
    SET @FindIndex=0


    WHILE(@StartIndex<=LEN(@Text))
    BEGIN
        SET @FindIndex=CHARINDEX(@Sign, @Text, @StartIndex)
        IF(@FindIndex=0 OR @FindIndex IS NULL)
        BEGIN
            -- 查找完毕
            SET @FindIndex=LEN(@Text)+1
        END

        SET @Content=LTRIM(RTRIM(SUBSTRING(@Text, @StartIndex, @FindIndex-@StartIndex)))

        -- 下次查找的位置
        SET @StartIndex=@FindIndex+1

        -- 插入结果 
        IF(LEN(@Content)>0)
        BEGIN
            INSERT INTO @ResultTable(TextValue) VALUES (@Content) 
        END
    END
    RETURN
END
GO

INSERT INTO [dbo].[sys_user]([user_name],[user_mobile],[user_password],[user_photo],[create_time],[delete_mark])
VALUES('管理员','administrator123','1','/upload/default_via.png',GETDATE(),0)

INSERT INTO [dbo].[sys_role]([role_name],[role_code],[role_level],[role_order],[create_time],[delete_mark])
VALUES('管理员','gly',999,0,GETDATE(),0),('民警','mj',10,0,GETDATE(),0),('社工','wgy',2,0,GETDATE(),0),('重点人员','zdry',1,0,GETDATE(),0)

INSERT INTO [dbo].[sys_user_role_relationship]([user_id],[role_id],[ur_order],[create_time],[delete_mark])
VALUES(1,1,0,getdate(),0)

INSERT INTO [dbo].[app_version_records]([version_code], [version_description], [create_time])
VALUES('1','APP上线初始版本', getdate())

insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark)
values('新闻管理', '', '/menu/newsManager.png','root', 0, 'web', 1, '', GETDATE(), 0)
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark)
values('签到管理', '', '/menu/signManager.png','root', 0, 'web', 2, '', GETDATE(), 0)
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark)
values('尿检管理', '', '/menu/urinalysisManager.png','root', 0, 'web', 3, '', GETDATE(), 0)
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark)
values('系统大屏', '', '/menu/chartScreen.png','root', 0, 'web', 4, '', GETDATE(), 0)
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark)
values('系统管理', '', '/menu/systemManager.png','root', 0, 'web', 6, '', GETDATE(), 0)
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark)
values('预警管理', '', '/menu/warningManager.png', 'root', 0, 'web', 5, '', GETDATE(), 0)
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('新闻快报', 'xwkb', '/menu/news.png','menu', 1, 'web', 1, '', GETDATE(), 0, '/views/news', '/a/news')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('社区新鲜事', 'sqxxs', '/menu/community.png','menu', 1, 'web', 4, '', GETDATE(), 0, '/views/community', '/a/community')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('签到查询', 'qdcx', '/menu/sign-in.png','menu', 2, 'web', 1, '', GETDATE(), 0, '/views/sign', '/a/sign')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('人员轨迹', 'rygj', '/menu/path.png','menu', 2, 'web', 2, '', GETDATE(), 0, '/views/track', '/a/track')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('尿检查询', 'njcx', '/menu/chemistry.png', 'menu', 3, 'web', 1, '', GETDATE(), 0, '/views/urinalysis', '/a/urinalysis')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('大屏报表', 'dpbb', '/menu/report-form.png', 'menu', 4, 'web', 1, '', GETDATE(), 0, '/views/report', '/a/report')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('部门/人员管理', 'bmrygl', '/menu/department.png','menu', 5, 'web', 1, '', GETDATE(), 0, '/views/department-user', '/a/department-user')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('角色管理', 'jsgl', '/menu/role.png','menu', 5, 'web', 2, '', GETDATE(), 0, '/views/role', '/a/role')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('教育视频', 'jysp', '/menu/video.png','menu', 1, 'web', 2, '', GETDATE(), 0, '/views/edu-video', '/a/edu-video')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('帮扶记录', 'bfjl', '/menu/hands-helping.png','menu', 1, 'web', 3, '', GETDATE(), 0, '/views/help-record', '/a/help-record')
insert into sys_menu(menu_name, menu_code, menu_icon, menu_type, menu_parent, menu_system, menu_order, menu_config, create_time, delete_mark, menu_url, menu_path)
values('电子围栏', 'dzwl', '/menu/fence.png', 'menu', 6, 'web', 1, '', GETDATE(), 0, '/views/ele-fence', '/a/ele-fence')

insert into sys_role_menu_relationship 
values
(1, 1, 0, GETDATE(), 0),(1, 2, 0, GETDATE(), 0),(1, 3, 0, GETDATE(), 0),
(1, 4, 0, GETDATE(), 0),(1, 5, 0, GETDATE(), 0),(1, 6, 0, GETDATE(), 0),
(1, 7, 0, GETDATE(), 0),(1, 8, 0, GETDATE(), 0),(1, 9, 0, GETDATE(), 0),
(1, 10, 0, GETDATE(), 0),(1, 11, 0, GETDATE(), 0),(1, 12, 0, GETDATE(), 0),
(1, 13, 0, GETDATE(), 0),(1, 14, 0, GETDATE(), 0),(1, 15, 0, GETDATE(), 0),
(1, 16, 0, GETDATE(), 0),(1, 17, 0, GETDATE(), 0)

--填写行政区编码和部门名称
--insert into sys_dep(dep_code, dep_name, dep_alias, dep_order, dep_parent, create_time, delete_mark)
--values('420000', '湖北省', '湖北省', 1, 0, GETDATE(), 0)