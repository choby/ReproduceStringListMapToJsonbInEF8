# ReproduceStringListMapToJsonbInEF8

run `Evo.RAM.HttpApi.Host`, and access `https://localhost:44342/api/app/data-permission`  and `https://localhost:44342/api/app/role`

Both DataPermission and Role have a `List<string>` field named Tags, but only DataPermission works, and Tags in a Role need to be set to `IEnumerable<string>`

```sql

drop table if exists "EvoDataPermissions";

/*==============================================================*/
/* Table: "EvoDataPermissions"                                  */
/*==============================================================*/
create table "EvoDataPermissions" (
   "Id"                 uuid                 not null,
   "Name"               varchar(64)          not null,
   "Code"               varchar(64)          not null,
   "ParentId"           uuid                 null,
   "IncludeAllUsers"    bool                 null,
   "IsEnabled"          bool                 null,
   "Tags"               jsonb                null,
   "Sort"               int                  null,
   "Brands"             jsonb                null,
   "Users"              jsonb                null,
   "ExtraProperties"    jsonb                null,
   "CreationTime"       TIMESTAMP            not null,
   "CreatorName"        varchar(24)          null,
   "CreatorId"          uuid                 null,
   "DeleterId"          uuid                 null,
   "DeleterName"        varchar(24)          null,
   "DeletionTime"       timestamp            null,
   "IsDeleted"          BOOL                 not null default false,
   "LastModificationTime" TIMESTAMP            null,
   "LastModifierName"   varchar(24)          null,
   "LastModifierId"     uuid                 null,
   "TenantId"           uuid                 null,
   "ConcurrencyStamp"   varchar(40)          null,
   constraint PK_EVODATAPERMISSIONS primary key ("Id", "Name", "Code")
);

comment on table "EvoDataPermissions" is
'数据权限';

comment on column "EvoDataPermissions"."IncludeAllUsers" is
'加载所有用户';

comment on column "EvoDataPermissions"."IsEnabled" is
'停用启用';

comment on column "EvoDataPermissions"."Brands" is
'绑定的品牌 ，有序';

comment on column "EvoDataPermissions"."Users" is
'绑定的用户，有序';


```

```sql
drop table if exists "EvoRoles";

/*==============================================================*/
/* Table: "EvoRoles"                                            */
/*==============================================================*/
create table "EvoRoles" (
   "Id"                 uuid                 not null,
   "Name"               varchar(24)          not null,
   "Code"               varchar(12)          null,
   "Type"               int2                 null,
   "IsEnabled"          bool                 null default true,
   "Tags"               jsonb                null,
   "Sort"               float                null,
   "ExtraProperties"    jsonb                null,
   "Description"        varchar(100)         null,
   "CreationTime"       TIMESTAMP            not null,
   "CreatorName"        varchar(24)          null,
   "CreatorId"          uuid                 null,
   "DeleterId"          uuid                 null,
   "DeleterName"        varchar(24)          null,
   "DeletionTime"       timestamp            null,
   "IsDeleted"          BOOL                 not null default false,
   "LastModificationTime" TIMESTAMP            null,
   "LastModifierName"   varchar(24)          null,
   "LastModifierId"     uuid                 null,
   "TenantId"           uuid                 null,
   "ConcurrencyStamp"   varchar(40)          not null,
   constraint PK_EVOROLES primary key ("Id")
);

```
