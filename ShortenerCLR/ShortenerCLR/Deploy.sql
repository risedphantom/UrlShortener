sp_configure 'clr enabled', 1
go
reconfigure
go

use TestDB
go

if object_id('GetShortUrl') is not null 
	drop function dbo.GetShortUrl
go

if object_id('GetLongUrl') is not null 
	drop function dbo.GetLongUrl
go

if exists(	select	top 1 1 
			from	sys.assemblies
			where	name = 'ShortenerCLR')
	drop assembly [ShortenerCLR]
go

create assembly [ShortenerCLR]
from '<binary data>'
with permission_set = unsafe
go

create function dbo.GetShortUrl(@longUrl nvarchar(max)) returns nvarchar(max) 
as external name ShortenerCLR.CouchBase.GetShortUrl;
go

create function dbo.GetLongUrl(@shortUrl nvarchar(max)) returns nvarchar(max) 
as external name ShortenerCLR.CouchBase.GetLongUrl;
go